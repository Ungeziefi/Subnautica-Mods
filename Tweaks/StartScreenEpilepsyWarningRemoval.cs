using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch(typeof(StartScreen))]
    public class TweakStartScreenEpilepsyWarningRemoval
    {
        [HarmonyPatch(nameof(StartScreen.TryToShowDisclaimer)), HarmonyPrefix]
        public static bool TryToShowDisclaimer(StartScreen __instance)
        {
            if (Main.TweaksConfig.SkipEpilepsyWarning)
            {
                // Main.Logger.LogInfo("Epilepsy warning skipped");
                return false;
            }
            return true;
        }
    }
}