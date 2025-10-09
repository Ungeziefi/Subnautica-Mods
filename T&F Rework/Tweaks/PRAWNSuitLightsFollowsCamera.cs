using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class PRAWNSuitLightsFollowsCamera
    {
        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.Start)), HarmonyPostfix]
        public static void Exosuit_Start(Exosuit __instance)
        {
            if (Main.Config.PRAWNSuitLightsFollowCamera)
            {
                PRAWNSuitLightsToggle.GetLightsTransform(__instance).SetParent(__instance.leftArmAttach);
            }
        }
    }
}