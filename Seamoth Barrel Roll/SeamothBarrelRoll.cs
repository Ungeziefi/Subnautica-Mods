using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Seamoth_Barrel_Roll
{
    [HarmonyPatch(typeof(SeaMoth))]
    internal class SeamothBarrelRoll
    {
        // Tracks active rolling states for each Seamoth instance
        private static Dictionary<SeaMoth, RollState> activeRolls = new Dictionary<SeaMoth, RollState>();
        private class RollState
        {
            public float currentRollForce = 0f;
            public float targetRollForce = 0f;     // Target force to reach (smoothly transitions)
            public bool isRolling = false;
            public bool wasRolling = false;        // (for sound transitions)
        }

        private static bool HasPower(Vehicle vehicle)
        {
            if (vehicle == null)
            {
                return false;
            }

            var energyMixin = vehicle.GetComponent<EnergyMixin>();
            return energyMixin != null && energyMixin.charge > 0f;
        }

        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.StabilizeRoll)), HarmonyPrefix]
        public static bool StabilizeRoll(Vehicle __instance)
        {
            // If set to normal mode, don't interfere with the default stabilization
            if (!Main.Config.EnableFeature || Main.Config.StabilizationMode == StabilizationMode.Normal)
            {
                return true;
            }

            // Check if stabilization is disabled
            if (Main.Config.StabilizationMode == StabilizationMode.Disabled)
            {
                return false;
            }

            // Check if stabilization requires power
            if (Main.Config.StabilizationRequiresPower && !HasPower(__instance))
            {
                return false;
            }

            // Allow stabilization only when the Seamoth isn't being piloted
            if (__instance is SeaMoth seamoth && Main.Config.StabilizationMode == StabilizationMode.OnlyWhenEmpty)
            {
                return !seamoth.GetPilotingMode();
            }

            return true;
        }

        // Rolling physics and effects
        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.FixedUpdate)), HarmonyPostfix]
        public static void FixedUpdate(Vehicle __instance)
        {
            if (!Main.Config.EnableFeature)
            {
                return;
            }

            // Check if the Seamoth is being piloted
            if (!(__instance is SeaMoth seamoth) || !seamoth.GetPilotingMode())
            {
                return;
            }

            // Check if rolling requires power
            if (Main.Config.RollingRequiresPower && !HasPower(seamoth))
            {
                return;
            }

            // Check if rolling in the air is allowed
            if (!Main.Config.AllowAirborneRolling && seamoth.transform.position.y > Ocean.GetOceanLevel())
            {
                return;
            }

            // Check if the Seamoth is powered
            PowerRelay powerRelay = __instance.GetComponent<PowerRelay>();
            if (powerRelay != null && powerRelay.GetPowerStatus() == PowerSystem.Status.Offline)
            {
                return;
            }

            // Initialize rolling state if it doesn't exist yet
            if (!activeRolls.ContainsKey(seamoth))
            {
                activeRolls[seamoth] = new RollState();
            }

            RollState state = activeRolls[seamoth];
            bool hasInput = Input.GetKey(Main.Config.RollLeftKey) || Input.GetKey(Main.Config.RollRightKey);

            // Set target force based on input direction
            if (Input.GetKey(Main.Config.RollLeftKey))
            {
                state.targetRollForce = Main.Config.RollForce;
                state.isRolling = true;
            }
            else if (Input.GetKey(Main.Config.RollRightKey))
            {
                state.targetRollForce = -Main.Config.RollForce;
                state.isRolling = true;
            }
            else
            {
                state.targetRollForce = 0f;
                state.isRolling = false;
            }

            // Interpolate the current force towards the target force
            if (state.currentRollForce < state.targetRollForce)
            {
                state.currentRollForce = Mathf.Min(state.currentRollForce + Main.Config.RollAcceleration * Time.fixedDeltaTime, state.targetRollForce);
            }
            else if (state.currentRollForce > state.targetRollForce)
            {
                state.currentRollForce = Mathf.Max(state.currentRollForce - Main.Config.RollAcceleration * Time.fixedDeltaTime, state.targetRollForce);
            }

            // Apply the rolling torque if there's enough force
            if (Mathf.Abs(state.currentRollForce) > 0.01f)
            {
                seamoth.useRigidbody.AddTorque(seamoth.transform.forward * state.currentRollForce * Time.fixedDeltaTime,
                    ForceMode.VelocityChange);
            }

            // Handle engine sound transitions
            if (state.isRolling != state.wasRolling)
            {
                if (!state.isRolling && seamoth.engineSound != null)
                {
                    seamoth.engineSound.AccelerateInput(1f);
                }
                state.wasRolling = state.isRolling;
            }

            // Update sound and visual effects during roll
            if (state.isRolling)
            {
                // Increase engine sound pitch based on roll intensity
                if (seamoth.engineSound != null)
                {
                    float speedRatio = Mathf.Abs(state.currentRollForce) / Main.Config.RollForce;
                    seamoth.engineSound.AccelerateInput(1f + speedRatio * 0.5f);
                }

                // Increase bubble emission based on roll intensity
                if (seamoth.bubbles != null)
                {
                    var emission = seamoth.bubbles.emission;
                    float bubbleIntensity = Mathf.Lerp(20f, 50f,
                        Mathf.Abs(state.currentRollForce) / Main.Config.RollForce);
                    emission.rateOverTime = bubbleIntensity;
                }
            }
        }

        // Cleanup and stabilization when exiting the Seamoth
        [HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.OnPilotModeEnd)), HarmonyPostfix]
        public static void OnPilotModeEnd(SeaMoth __instance)
        {
            if (!Main.Config.EnableFeature || !activeRolls.ContainsKey(__instance))
            {
                return;
            }

            // Reset engine sound
            if (__instance.engineSound != null)
            {
                __instance.engineSound.AccelerateInput(1f);
            }

            // Apply immediate stabilization force when exiting if in OnlyWhenEmpty mode
            if (Main.Config.StabilizationMode == StabilizationMode.OnlyWhenEmpty)
            {
                // Check if stabilization requires power
                if (Main.Config.StabilizationRequiresPower && !HasPower(__instance))
                {
                    activeRolls.Remove(__instance);
                    return;
                }

                // Convert from Euler angles to -180 to 180 range
                float zAngle = __instance.transform.eulerAngles.z;
                if (zAngle > 180f) zAngle -= 360f;

                if (Mathf.Abs(zAngle) > 2f)
                {
                    __instance.useRigidbody.AddTorque(-__instance.transform.forward * Mathf.Sign(zAngle),
                        ForceMode.VelocityChange);
                }
            }

            // Clean up the rolling state
            activeRolls.Remove(__instance);
        }
    }
}