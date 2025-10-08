using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class PRAWNSuitArmsNeedPower
    {
        static Vector3 aimTargetLeftPos;
        static Vector3 aimTargetRightPos;

        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.Update)), HarmonyPrefix]
        public static void Exosuit_Update_Prefix(Exosuit __instance)
        {
            if (Main.Config.PRAWNSuitArmsNeedPower && !__instance.IsPowered())
            {
                aimTargetLeftPos = __instance.aimTargetLeft.position;
                aimTargetRightPos = __instance.aimTargetRight.position;
            }
        }

        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.Update)), HarmonyPostfix]
        public static void Exosuit_Update_Postfix(Exosuit __instance)
        {
            if (Main.Config.PRAWNSuitArmsNeedPower && !__instance.IsPowered())
            {
                __instance.aimTargetLeft.position = aimTargetLeftPos;
                __instance.aimTargetRight.position = aimTargetRightPos;
            }
        }
    }
}