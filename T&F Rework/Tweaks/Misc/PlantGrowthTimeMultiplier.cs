using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class PlantGrowthTimeMultiplier
    {
        [HarmonyPatch(typeof(GrowingPlant), nameof(GrowingPlant.GetGrowthDuration)), HarmonyPostfix]
        public static void GrowingPlant_GetGrowthDuration(ref float __result)
        {
            if (Main.Config.PlantGrowthTimeMultiplier == 1 || NoCostConsoleCommand.main.fastGrowCheat) return;

            __result *= Main.Config.PlantGrowthTimeMultiplier;
        }
    }
}