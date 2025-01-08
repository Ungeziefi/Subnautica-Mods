using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

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

        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.StabilizeRoll)), HarmonyPrefix]
        public static bool StabilizeRoll(Vehicle __instance)
        {
            if (!Main.Config.EnableFeature || Main.Config.StabilizationMode == StabilizationMode.Normal)
            {
                return true;
            }

            if (Main.Config.StabilizationMode == StabilizationMode.Disabled)
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

            if (!(__instance is SeaMoth seamoth) || !seamoth.GetPilotingMode())
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
                // Calculate the current roll angle
                float zAngle = __instance.transform.eulerAngles.z;
                if (zAngle > 180f) zAngle -= 360f;  // Convert to -180 to 180 range

                // Apply stabilization if tilted more than 2 degrees
                if (Mathf.Abs(zAngle) > 2f)
                {
                    float stabilizeForce = Mathf.Sign(zAngle);
                    __instance.useRigidbody.AddTorque(-__instance.transform.forward * stabilizeForce,
                        ForceMode.VelocityChange);
                }
            }

            // Clean up the rolling state
            activeRolls.Remove(__instance);
        }
    }
}