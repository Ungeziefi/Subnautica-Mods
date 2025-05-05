using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class SeaglideLightAngle
    {
        private static float lastAppliedAngle = float.MinValue;

        [HarmonyPatch(typeof(Seaglide), nameof(Seaglide.OnDraw)), HarmonyPostfix]
        public static void Seaglide_OnDraw(Seaglide __instance)
        {
            if (!Main.Config.SLAEnableFeature || __instance.toggleLights == null)
                return;

            Transform lightsParent = __instance.gameObject.transform.Find("lights_parent");
            if (lightsParent != null)
            {
                // Only apply changes if config value has changed
                if (lastAppliedAngle != Main.Config.LightAngle)
                {
                    lightsParent.localRotation = Quaternion.Euler(-Main.Config.LightAngle, 0f, 0f);

                    lastAppliedAngle = Main.Config.LightAngle;
                }
            }
        }
    }
}