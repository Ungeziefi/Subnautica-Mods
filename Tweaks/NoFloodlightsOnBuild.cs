using System.Collections.Generic;
using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch(typeof(CyclopsLightingPanel))]
    public class NoFloodlightsOnBuild
    {
        [HarmonyPatch(nameof(CyclopsLightingPanel.SubConstructionComplete)), HarmonyPrefix]
        static bool SubConstructionComplete(CyclopsLightingPanel __instance)
        {
            if (Main.Config.NoFloodlightsOnBuild)
            {
                return false;
            }
            return true;
        }
    }
}