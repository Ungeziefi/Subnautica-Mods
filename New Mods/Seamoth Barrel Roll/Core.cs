using HarmonyLib;
using Nautilus.FMod;
using Nautilus.Utility;
using UnityEngine;
using System.Collections.Generic;

namespace Ungeziefi.Seamoth_Barrel_Roll
{
    [HarmonyPatch]
    public partial class SeamothBarrelRoll
    {
        private static bool registeredSound = false;
        private static readonly Dictionary<SeaMoth, float> currentRollForces = new();
        public static bool HasPower(Vehicle vehicle) => vehicle.GetComponent<EnergyMixin>().charge > 0f;
        private static bool ShouldProcessRolling(Vehicle vehicle, out SeaMoth seamoth)
        {
            seamoth = vehicle as SeaMoth;

            bool isPausedOrLoading = WaitScreen.IsWaiting
                || Cursor.visible
                || UWE.FreezeTime.HasFreezers()
                || Player.main.GetPDA().isOpen;

            return seamoth != null &&
                   seamoth.GetPilotingMode() &&
                   Main.Config.EnableFeature &&
                   !isPausedOrLoading &&
                   (!Main.Config.RollingRequiresPower || HasPower(seamoth)) &&
                   (Main.Config.AllowAirborneRolling || !IsAirborne(seamoth));
        }

        private static bool IsAirborne(SeaMoth seamoth)
        {
            return seamoth.transform.position.y > Ocean.GetOceanLevel() ||
                   seamoth.precursorOutOfWater;
        }

        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.FixedUpdate)), HarmonyPostfix]
        public static void Vehicle_FixedUpdate(Vehicle __instance)
        {
            if (!ShouldProcessRolling(__instance, out SeaMoth sm)) return;

            bool left = GameInput.GetButtonHeld(Main.RollLeftButton);
            bool right = GameInput.GetButtonHeld(Main.RollRightButton);
            bool isRolling = left || right;

            currentRollForces.TryGetValue(sm, out float currentForce);
            float targetForce = left && right ? currentForce : left ? Main.Config.RollForce : right ? -Main.Config.RollForce : 0f;

            currentForce = Main.Config.RollAcceleration == 0f ? targetForce :
                Mathf.MoveTowards(currentForce, targetForce, Main.Config.RollAcceleration * Time.fixedDeltaTime);

            currentRollForces[sm] = currentForce;

            if (isRolling)
            {
                sm.useRigidbody.AddTorque(sm.transform.forward * currentForce * Time.fixedDeltaTime, ForceMode.VelocityChange);

                float intensity = Mathf.Abs(currentForce) / Main.Config.RollForce;

                if (sm.engineSound) sm.engineSound.AccelerateInput(1f + intensity * 0.5f);
                if (sm.bubbles)
                {
                    var emission = sm.bubbles.emission;
                    emission.rateOverTime = Mathf.Lerp(20f, 50f, intensity);
                }
            }
            else if (sm.engineSound)
            {
                sm.engineSound.AccelerateInput(1f);
            }
        }

        #region Star Fox Sound
        [HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.Start)), HarmonyPostfix]
        public static void SeaMoth_Start()
        {
            if (registeredSound) return;

            new FModSoundBuilder(new ModFolderSoundSource("Sounds"))
                .CreateNewEvent("DoABarrelRoll", AudioUtils.BusPaths.UnderwaterAmbient)
                .SetMode2D()
                .SetSound("DoABarrelRoll")
                .Register();

            registeredSound = true;
        }

        [HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.Update)), HarmonyPostfix]
        public static void SeaMoth_Update(Vehicle __instance)
        {
            if (__instance is not SeaMoth sm || !Main.Config.EnableFeature || !Main.Config.StarFoxSound || !sm.GetPilotingMode()) return;

            if (GameInput.GetButtonDown(Main.RollLeftButton) || GameInput.GetButtonDown(Main.RollRightButton))
            {
                FMODUWE.PlayOneShot(AudioUtils.GetFmodAsset("DoABarrelRoll"), Player.main.transform.position);
            }
        }
        #endregion
    }
}