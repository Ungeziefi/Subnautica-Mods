using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Seamoth_Barrel_Roll
{
    [HarmonyPatch]
    internal class SeamothBarrelRoll
    {
        #region State and Power Check
        private static Dictionary<SeaMoth, RollState> activeRolls = new Dictionary<SeaMoth, RollState>();

        private class RollState
        {
            public float currentRollForce, targetRollForce;
            public bool isRolling, wasRolling; // State tracking for sound effects
        }

        private static bool HasPower(Vehicle vehicle) =>
            vehicle?.GetComponent<EnergyMixin>()?.charge > 0f;
        #endregion

        #region Stabilization
        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.StabilizeRoll)), HarmonyPrefix]
        public static bool Vehicle_StabilizeRoll(Vehicle __instance)
        {
            if (!Main.Config.EnableFeature ||
                Main.Config.StabilizationMode == StabilizationMode.Normal)
                return true;  // Use normal stabilization

            if (Main.Config.StabilizationMode == StabilizationMode.Disabled)
                return false;  // Disable stabilization

            if (Main.Config.StabilizationRequiresPower && !HasPower(__instance))
                return false;  // No stabilization without power

            // Only stabilize when empty
            if (__instance is SeaMoth seamoth && Main.Config.StabilizationMode == StabilizationMode.OnlyWhenEmpty)
            {
                return !seamoth.GetPilotingMode(); // Only when not piloting
            }
            else
            {
                return true; // Otherwise always stabilize
            }
        }
        #endregion

        #region Physics
        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.FixedUpdate)), HarmonyPostfix]
        public static void Vehicle_FixedUpdate(Vehicle __instance)
        {
            if (!Main.Config.EnableFeature ||
                !(__instance is SeaMoth seamoth) ||
                !seamoth.GetPilotingMode() ||
                (Main.Config.RollingRequiresPower && !HasPower(seamoth)) || // Power
                (!Main.Config.AllowAirborneRolling && seamoth.transform.position.y > Ocean.GetOceanLevel())) // Airborne rolling
                return;

            // Get or create roll state
            var state = activeRolls.ContainsKey(seamoth) ? activeRolls[seamoth] : activeRolls[seamoth] = new RollState();

            // Force from input
            bool rollLeft = Input.GetKey(Main.Config.RollLeftKey);
            bool rollRight = Input.GetKey(Main.Config.RollRightKey);
            state.isRolling = rollLeft || rollRight;
            state.targetRollForce = rollLeft ? Main.Config.RollForce : // Left is positive
                                  rollRight ? -Main.Config.RollForce : // Right is negative
                                  0f; // No input

            // Interpolate towards target
            state.currentRollForce = Mathf.MoveTowards(state.currentRollForce, state.targetRollForce,
                                                     Main.Config.RollAcceleration * Time.fixedDeltaTime);

            // Roll when enough force
            if (Mathf.Abs(state.currentRollForce) > 0.01f)
                seamoth.useRigidbody.AddTorque(seamoth.transform.forward * state.currentRollForce * Time.fixedDeltaTime,
                    ForceMode.VelocityChange);

            // Engine sound transitions
            if (state.isRolling != state.wasRolling)
            {
                if (!state.isRolling && seamoth.engineSound != null)
                    seamoth.engineSound.AccelerateInput(1f);  // Reset engine sound when stopping roll
                state.wasRolling = state.isRolling;
            }

            // Rolling effects
            if (state.isRolling)
            {
                // Adjust engine sound based on roll intensity
                if (seamoth.engineSound != null)
                {
                    float speedRatio = Mathf.Abs(state.currentRollForce) / Main.Config.RollForce;
                    seamoth.engineSound.AccelerateInput(1f + speedRatio * 0.5f);
                }

                // Adjust bubble effects based on roll intensity
                if (seamoth.bubbles != null)
                {
                    var emission = seamoth.bubbles.emission;
                    emission.rateOverTime = Mathf.Lerp(20f, 50f,
                        Mathf.Abs(state.currentRollForce) / Main.Config.RollForce);
                }
            }
        }
        #endregion

        #region Cleanup
        [HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.OnPilotModeEnd)), HarmonyPostfix]
        public static void SeaMoth_OnPilotModeEnd(SeaMoth __instance)
        {
            if (!Main.Config.EnableFeature || !activeRolls.ContainsKey(__instance))
                return;

            // Reset engine sound
            if (__instance.engineSound != null)
                __instance.engineSound.AccelerateInput(1f);

            // Apply stabilization when empty if configured
            if (Main.Config.StabilizationMode == StabilizationMode.OnlyWhenEmpty &&
                (!Main.Config.StabilizationRequiresPower || HasPower(__instance)))
            {
                // Convert rotation to -180/180 range
                float zAngle = __instance.transform.eulerAngles.z;
                if (zAngle > 180f) zAngle -= 360f;

                if (Mathf.Abs(zAngle) > 2f)
                {
                    __instance.useRigidbody.AddTorque(-__instance.transform.forward * Mathf.Sign(zAngle),
                        ForceMode.VelocityChange);
                }
            }

            // Remove tracking state
            activeRolls.Remove(__instance);
        }
        #endregion
    }
}