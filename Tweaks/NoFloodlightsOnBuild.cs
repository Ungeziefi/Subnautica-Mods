using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class NoFloodlightsOnBuild
    {
        [HarmonyPatch(typeof(CyclopsLightingPanel), nameof(CyclopsLightingPanel.SubConstructionComplete)), HarmonyPrefix]
        static bool CyclopsLightingPanel_SubConstructionComplete(CyclopsLightingPanel __instance) => !Main.Config.NoFloodlightsOnBuild;
    }
}