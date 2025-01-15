using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class EncyclopediaScrollFix
    {
        [HarmonyPatch(typeof(uGUI_EncyclopediaTab), nameof(uGUI_EncyclopediaTab.DisplayEntry)), HarmonyPostfix]
        public static void uGUI_EncyclopediaTab_DisplayEntry(uGUI_EncyclopediaTab __instance)
        {
            if (Main.Config.ResetDatabankScroll)
            {
                __instance.contentScrollRect.verticalNormalizedPosition = 1f;
            }
        }
    }
}