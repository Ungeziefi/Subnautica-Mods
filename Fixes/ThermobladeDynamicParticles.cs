using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(VFXLateTimeParticles))]
    public class ThermobladeDynamicParticles
    {
        public static ParticleSystem[] heatBladeParticles;

        [HarmonyPatch(nameof(VFXLateTimeParticles.Play)), HarmonyPostfix]
        public static void Play(VFXLateTimeParticles __instance)
        {
            if (!Main.Config.ThermobladeDynamicParticles || __instance.name != "xHeatBlade_Bubbles(Clone)")
            {
                return;
            }

            heatBladeParticles = __instance.psChildren;
            FixHeatBlade();
        }

        public static void FixHeatBlade()
        {
            if (heatBladeParticles == null ||
                heatBladeParticles.Length != 3 ||
                heatBladeParticles[0] == null ||
                heatBladeParticles[0].gameObject == null ||
                !heatBladeParticles[0].gameObject.activeInHierarchy)
            {
                return;
            }

            bool underwater = Player.main.isUnderwater.value;
            // Enable bubbles with refraction when underwater, only smoke when above water
            heatBladeParticles[0].EnableEmission(underwater); // xHeatBlade_Bubbles(Clone)
            heatBladeParticles[2].EnableEmission(underwater); // xRefract
            heatBladeParticles[1].EnableEmission(!underwater); // xSmk

        }

        public static void OnPlayerUnderwaterChanged(Utils.MonitoredValue<bool> isUnderwaterForSwimming)
        {
            FixHeatBlade();
        }
    }
}