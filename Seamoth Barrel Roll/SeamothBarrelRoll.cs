using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Seamoth_Barrel_Roll
{
    [HarmonyPatch]
    internal class SeamothBarrelRoll
    {
        // Track active rolling states
        private static Dictionary<SeaMoth, RollState> activeRolls = new Dictionary<SeaMoth, RollState>();
        private class RollState
        {
            public float currentRollForce = 0f;
            public float targetRollForce = 0f;     // Target force to reach
            public bool isRolling = false;
            public bool wasRolling = false;        // For sound transitions
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
        public static bool Vehicle_StabilizeRoll(Vehicle __instance)
        {
            // No effect in normal mode
            if (!Main.Config.EnableFeature || Main.Config.StabilizationMode == StabilizationMode.Normal)
            {
                return true;
            }

            // Check if disabled
            if (Main.Config.StabilizationMode == StabilizationMode.Disabled)
            {
                return false;
            }

            // Check power requirement
            if (Main.Config.StabilizationRequiresPower && !HasPower(__instance))
            {
                return false;
            }

            // Stabilize only when empty
            if (__instance is SeaMoth seamoth && Main.Config.StabilizationMode == StabilizationMode.OnlyWhenEmpty)
            {
                return !seamoth.GetPilotingMode();
            }

            return true;
        }

        // Rolling physics and effects
        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.FixedUpdate)), HarmonyPostfix]
        public static void Vehicle_FixedUpdate(Vehicle __instance)
        {
            if (!Main.Config.EnableFeature)
            {
                return;
            }

            // Check if piloting
            if (!(__instance is SeaMoth seamoth) || !seamoth.GetPilotingMode())
            {
                return;
            }

            // Check power requirement
            if (Main.Config.RollingRequiresPower && !HasPower(seamoth))
            {
                return;
            }

            // Check airborne rolling
            if (!Main.Config.AllowAirborneRolling && seamoth.transform.position.y > Ocean.GetOceanLevel())
            {
                return;
            }

            // Check power status
            PowerRelay powerRelay = __instance.GetComponent<PowerRelay>();
            if (powerRelay != null && powerRelay.GetPowerStatus() == PowerSystem.Status.Offline)
            {
                return;
            }

            // Init roll state
            if (!activeRolls.ContainsKey(seamoth))
            {
                activeRolls[seamoth] = new RollState();
            }

            RollState state = activeRolls[seamoth];
            bool hasInput = Input.GetKey(Main.Config.RollLeftKey) || Input.GetKey(Main.Config.RollRightKey);

            // Set force
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

            // Interpolate current force towards target
            if (state.currentRollForce < state.targetRollForce)
            {
                state.currentRollForce = Mathf.Min(state.currentRollForce + Main.Config.RollAcceleration * Time.fixedDeltaTime, state.targetRollForce);
            }
            else if (state.currentRollForce > state.targetRollForce)
            {
                state.currentRollForce = Mathf.Max(state.currentRollForce - Main.Config.RollAcceleration * Time.fixedDeltaTime, state.targetRollForce);
            }

            // Apply torque if enough force
            if (Mathf.Abs(state.currentRollForce) > 0.01f)
            {
                seamoth.useRigidbody.AddTorque(seamoth.transform.forward * state.currentRollForce * Time.fixedDeltaTime,
                    ForceMode.VelocityChange);
            }

            // Engine sound transitions
            if (state.isRolling != state.wasRolling)
            {
                if (!state.isRolling && seamoth.engineSound != null)
                {
                    seamoth.engineSound.AccelerateInput(1f);
                }
                state.wasRolling = state.isRolling;
            }

            // Effects based on roll intensity
            if (state.isRolling)
            {
                // Engine sound
                if (seamoth.engineSound != null)
                {
                    float speedRatio = Mathf.Abs(state.currentRollForce) / Main.Config.RollForce;
                    seamoth.engineSound.AccelerateInput(1f + speedRatio * 0.5f);
                }

                // Bubble emission
                if (seamoth.bubbles != null)
                {
                    var emission = seamoth.bubbles.emission;
                    float bubbleIntensity = Mathf.Lerp(20f, 50f,
                        Mathf.Abs(state.currentRollForce) / Main.Config.RollForce);
                    emission.rateOverTime = bubbleIntensity;
                }
            }
        }

        // Cleanup and stabilization on exit
        [HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.OnPilotModeEnd)), HarmonyPostfix]
        public static void SeaMoth_OnPilotModeEnd(SeaMoth __instance)
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

            // Stabilization when empty
            if (Main.Config.StabilizationMode == StabilizationMode.OnlyWhenEmpty)
            {
                // Check power requirement
                if (Main.Config.StabilizationRequiresPower && !HasPower(__instance))
                {
                    activeRolls.Remove(__instance);
                    return;
                }

                // Convert angles to -180/180
                float zAngle = __instance.transform.eulerAngles.z;
                if (zAngle > 180f) zAngle -= 360f;

                if (Mathf.Abs(zAngle) > 2f)
                {
                    __instance.useRigidbody.AddTorque(-__instance.transform.forward * Mathf.Sign(zAngle),
                        ForceMode.VelocityChange);
                }
            }

            // Clean uo roll state
            activeRolls.Remove(__instance);
        }
    }
}