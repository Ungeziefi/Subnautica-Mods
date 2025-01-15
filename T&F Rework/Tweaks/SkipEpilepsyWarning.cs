using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class SkipEpilepsyWarning
    {
        [HarmonyPatch(typeof(StartScreen), nameof(StartScreen.TryToShowDisclaimer)), HarmonyPrefix]
        public static bool StartScreen_TryToShowDisclaimer(StartScreen __instance) => !Main.Config.SkipEpilepsyWarning;
    }
}