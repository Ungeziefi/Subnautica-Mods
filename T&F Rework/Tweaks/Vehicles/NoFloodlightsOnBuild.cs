using HarmonyLib;

namespace Ungeziefi.Tweaks;

[HarmonyPatch]
public class NoFloodlightsOnBuild
{
    [HarmonyPatch(typeof(CyclopsLightingPanel), nameof(CyclopsLightingPanel.SubConstructionComplete))]
    [HarmonyPrefix]
    private static bool CyclopsLightingPanel_SubConstructionComplete()
    {
        return !Main.Config.NoFloodlightsOnBuild;
    }
}