using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(FlashLight))]
    public class NoFlashlightPointLights
    {
        [HarmonyPatch(nameof(FlashLight.Start)), HarmonyPrefix]
        public static void Start(FlashLight __instance)
        {
            if (!Main.Config.NoFlashlightPointLights)
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