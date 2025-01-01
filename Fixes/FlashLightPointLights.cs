using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    // Removed point lights from the flashlight to prevent illuminating behind the player
    [HarmonyPatch(typeof(FlashLight))]
    public class FixFlashLightPointLights
    {
        [HarmonyPatch(nameof(FlashLight.Start)), HarmonyPrefix]
        public static void Start(FlashLight __instance)
        {
            if (!Main.FixesConfig.NoFlashlightPointLights)
            {
                return;
            }

            var lights = __instance.GetComponentsInChildren<Light>(true);
            for (int i = lights.Length - 1; i >= 0; i--)
            {
                if (lights[i].type == LightType.Point)
                    lights[i].enabled = false;
            }
        }
    }
}