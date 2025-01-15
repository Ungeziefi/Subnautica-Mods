using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoFlashlightPointLights
    {
        [HarmonyPatch(typeof(FlashLight), nameof(FlashLight.Start)), HarmonyPrefix]
        public static void FlashLight_Start(FlashLight __instance)
        {
            if (!Main.Config.NoFlashlightPointLights) return;

            // Get all point lights and disable them
            var lights = __instance.GetComponentsInChildren<Light>(true);
            for (int i = lights.Length - 1; i >= 0; i--)
            {
                if (lights[i].type == LightType.Point)
                {
                    lights[i].enabled = false;
                }
            }
        }
    }
}