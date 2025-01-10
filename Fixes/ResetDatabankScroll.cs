using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(uGUI_EncyclopediaTab))]
    public class EncyclopediaScrollFix
    {
        [HarmonyPatch(nameof(uGUI_EncyclopediaTab.DisplayEntry)), HarmonyPostfix]
        public static void DisplayEntry(uGUI_EncyclopediaTab __instance)
        {
            if (Main.Config.ResetDatabankScroll)
            {
                __instance.contentScrollRect.verticalNormalizedPosition = 1f;
            }
        }
    }
}