using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    internal class PlantGrowthTimeMultiplier
    {
        [HarmonyPatch(typeof(GrowingPlant), nameof(GrowingPlant.GetGrowthDuration)), HarmonyPostfix]
        public static void GrowingPlant_GetGrowthDuration(ref float __result)
        {
            if (Main.Config.PlantGrowthTimeMultiplier == 1 || NoCostConsoleCommand.main.fastGrowCheat)
            {
                // Main.Logger.LogInfo($"Kept original plant growth time at: {__result}");
                return;
            }

            __result *= Main.Config.PlantGrowthTimeMultiplier;
            // Main.Logger.LogInfo($"Plant growth duration is now: {__result}");
        }
    }
}