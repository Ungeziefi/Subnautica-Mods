using HarmonyLib;
using Nautilus.FMod;
using UnityEngine;

namespace Ungeziefi.Seamoth_Barrel_Roll
{
    [HarmonyPatch]
    public partial class SeamothBarrelRoll
    {
        private static bool registeredSound = false;

        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.FixedUpdate)), HarmonyPostfix]
        public static void Vehicle_FixedUpdate(Vehicle __instance)
        {
            if (!ShouldProcessRolling(__instance, out SeaMoth seamoth)) return;

            var state = GetOrCreateRollState(seamoth);
            UpdateRollInput(state);
            ApplyRollPhysics(seamoth, state);
            UpdateRollEffects(seamoth, state);
        }

        private static bool ShouldProcessRolling(Vehicle vehicle, out SeaMoth seamoth)
        {
            seamoth = null;

            if (!Main.Config.EnableFeature)
                return false;

            seamoth = vehicle as SeaMoth;
            if (seamoth == null || !seamoth.GetPilotingMode())
                return false;

            if (Main.Config.RollingRequiresPower && !HasPower(seamoth))
                return false;

            if (!Main.Config.AllowAirborneRolling && IsAirborne(seamoth))
                return false;

            return true;
        }

        private static bool IsAirborne(SeaMoth seamoth)
        {
            return seamoth.transform.position.y > Ocean.GetOceanLevel() ||
                   seamoth.precursorOutOfWater ||
                   seamoth.onGround;
        }

        private static RollState GetOrCreateRollState(SeaMoth seamoth)
        {
            if (!activeRolls.TryGetValue(seamoth, out var state))
            {
                state = new RollState();
                activeRolls[seamoth] = state;
            }
            return state;
        }

        private static void UpdateRollInput(RollState state)
        {
            bool rollLeft = GameInput.GetButtonHeld(Main.RollLeftButton);
            bool rollRight = GameInput.GetButtonHeld(Main.RollRightButton);

            state.isRolling = rollLeft || rollRight;
            state.targetRollForce = rollLeft ? Main.Config.RollForce :
                                   rollRight ? -Main.Config.RollForce :
                                   0f;

            // Interpolate towards target
            state.currentRollForce = Mathf.MoveTowards(
                state.currentRollForce,
                state.targetRollForce,
                Main.Config.RollAcceleration * Time.fixedDeltaTime);
        }

        private static void ApplyRollPhysics(SeaMoth seamoth, RollState state)
        {
            if (Mathf.Abs(state.currentRollForce) > 0.01f)
            {
                seamoth.useRigidbody.AddTorque(
                    seamoth.transform.forward * state.currentRollForce * Time.fixedDeltaTime,
                    ForceMode.VelocityChange);
            }
        }

        private static void UpdateRollEffects(SeaMoth seamoth, RollState state)
        {
            UpdateEngineSound(seamoth, state);
            UpdateBubbleEffects(seamoth, state);
        }

        private static void UpdateEngineSound(SeaMoth seamoth, RollState state)
        {
            if (seamoth.engineSound == null) return;

            // Handle state transitions
            if (state.isRolling != state.wasRolling)
            {
                if (!state.isRolling)
                {
                    seamoth.engineSound.AccelerateInput(1f);
                }
                state.wasRolling = state.isRolling;
            }

            // Adjust sound during roll
            if (state.isRolling)
            {
                float speedRatio = Mathf.Abs(state.currentRollForce) / Main.Config.RollForce;
                seamoth.engineSound.AccelerateInput(1f + speedRatio * 0.5f);
            }
        }

        private static void UpdateBubbleEffects(SeaMoth seamoth, RollState state)
        {
            if (!state.isRolling || seamoth.bubbles == null) return;

            var emission = seamoth.bubbles.emission;
            float rollIntensity = Mathf.Abs(state.currentRollForce) / Main.Config.RollForce;
            emission.rateOverTime = Mathf.Lerp(20f, 50f, rollIntensity);
        }

        #region Star Fox Sound
        [HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.Start)), HarmonyPostfix]
        public static void SeaMoth_Start()
        {
            if (registeredSound) return;

            CustomSoundSourceBase soundSource = new ModFolderSoundSource("SoundsFolder");
            new FModSoundBuilder(soundSource)
                .CreateNewEvent("DoABarrelRoll", Nautilus.Utility.AudioUtils.BusPaths.UnderwaterAmbient)
                .SetMode2D()
                .SetSound("DoABarrelRoll")
                .Register();

            registeredSound = true;
        }

        [HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.Update)), HarmonyPostfix]
        public static void SeaMoth_Update(Vehicle __instance)
        {
            if (!ShouldPlayBarrelRollSound(__instance)) return;

            if (GameInput.GetButtonDown(Main.RollLeftButton) ||
                GameInput.GetButtonDown(Main.RollRightButton))
            {
                Utils.PlayFMODAsset(
                    Nautilus.Utility.AudioUtils.GetFmodAsset("DoABarrelRoll"),
                    Player.main.transform.position);
            }
        }

        private static bool ShouldPlayBarrelRollSound(Vehicle vehicle)
        {
            return Main.Config.EnableFeature &&
                   Main.Config.StarFoxSound &&
                   vehicle is SeaMoth seamoth &&
                   seamoth.GetPilotingMode();
        }
        #endregion
    }
}