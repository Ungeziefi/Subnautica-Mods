using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class PRAWNSuitLightFollowsCamera
    {
        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.Start)), HarmonyPostfix]
        public static void Exosuit_Start(Exosuit __instance)
        {
            if (Main.Config.PRAWNSuitLightFollowsCamera)
            {
                (__instance.leftArmAttach.transform.Find("lights_parent") ?? __instance.transform.Find("lights_parent"))?.SetParent(__instance.leftArmAttach);
            }
        }
    }
}