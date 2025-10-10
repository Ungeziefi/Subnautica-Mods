using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class PRAWNSuitLightsFollowsCamera
    {
        private static Transform GetLightsTransform(Exosuit exosuit)
        {
            return exosuit?.leftArmAttach?.transform?.Find("lights_parent")
                   ?? exosuit?.transform?.Find("lights_parent");
        }

        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.Start)), HarmonyPostfix]
        public static void Exosuit_Start(Exosuit __instance)
        {
            if (Main.Config.PRAWNSuitLightsFollowCamera)
            {
                GetLightsTransform(__instance).SetParent(__instance.leftArmAttach);
            }
        }
    }
}