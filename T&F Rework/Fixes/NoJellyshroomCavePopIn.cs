using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoJellyshroomCavePopIn
    {
        [HarmonyPatch(typeof(LargeWorldEntity), nameof(LargeWorldEntity.Awake)), HarmonyPostfix]
        private static void LargeWorldEntity_Awake(LargeWorldEntity __instance)
        {
            if (Main.Config.NoJellyshroomCavePopIn == "Disabled") return;

            if (__instance.name.StartsWith("Crab_snake_mushrooms") ||
                __instance.name.StartsWith("coral_reef_Stalactite"))
            {
                __instance.cellLevel = Main.Config.NoJellyshroomCavePopIn switch
                {
                    "Medium" => LargeWorldEntity.CellLevel.Medium,
                    "Far" => LargeWorldEntity.CellLevel.Far,
                    "VeryFar" => LargeWorldEntity.CellLevel.VeryFar,
                    _ => LargeWorldEntity.CellLevel.Near // Default
                };
            }
        }
    }
}