﻿using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch(typeof(StartScreen))]
    public class SkipEpilepsyWarning
    {
        [HarmonyPatch(nameof(StartScreen.TryToShowDisclaimer)), HarmonyPrefix]
        public static bool TryToShowDisclaimer(StartScreen __instance)
        {
            if (Main.TweaksConfig.SkipEpilepsyWarning)
            {
                return false;
            }
            return true;
        }
    }
}