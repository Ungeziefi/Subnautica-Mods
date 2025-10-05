using HarmonyLib;
using Nautilus.FMod;
using UnityEngine;

namespace Ungeziefi.Seamoth_Barrel_Roll
{
    [HarmonyPatch]
    public partial class SeamothBarrelRoll
    {
        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.FixedUpdate)), HarmonyPostfix]
        public static void Vehicle_FixedUpdate(Vehicle __instance)
        {
            if (!Main.Config.EnableFeature ||
                !(__instance is SeaMoth seamoth) ||
                !seamoth.GetPilotingMode() ||
                (Main.Config.RollingRequiresPower && !HasPower(seamoth)) || // Power
                (!Main.Config.AllowAirborneRolling && (seamoth.transform.position.y > Ocean.GetOceanLevel() || // Above water
                                                        seamoth.precursorOutOfWater || // In the QEP moon pool
                                                        seamoth.onGround))) // Just in case
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

        #region Star Fox Sound
        // Register sound event
        [HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.Start)), HarmonyPostfix]
        public static void SeaMoth_Start(Vehicle __instance)
        {
            // Should register regardless to allow toggling without restart
            // Actually not sure but I'm too lazy to test
            // I'm probably right anyway...

            CustomSoundSourceBase soundSource = new ModFolderSoundSource("SoundsFolder");
            FModSoundBuilder builder = new FModSoundBuilder(soundSource);
            builder.CreateNewEvent("DoABarrelRoll", Nautilus.Utility.AudioUtils.BusPaths.UnderwaterAmbient)
                .SetMode2D()
                .SetSound("DoABarrelRoll")
                .Register();
        }

        // Play sound
        [HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.Update)), HarmonyPostfix]
        public static void SeaMoth_Update(Vehicle __instance)
        {
            if (!Main.Config.EnableFeature || !Main.Config.StarFoxSound) return;

            if (__instance is SeaMoth seamoth && seamoth.GetPilotingMode() &&
                (Input.GetKeyDown(Main.Config.RollLeftKey) || Input.GetKeyDown(Main.Config.RollRightKey)))
            {
                Utils.PlayFMODAsset(Nautilus.Utility.AudioUtils.GetFmodAsset("DoABarrelRoll"), Player.main.transform.position);
            }
        }
        #endregion
    }
}