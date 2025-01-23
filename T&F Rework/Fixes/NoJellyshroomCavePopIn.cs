using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoJellyshroomCavePopIn
    {
        [HarmonyPatch(typeof(LargeWorldEntity), nameof(LargeWorldEntity.Awake)), HarmonyPostfix]
        private static void LargeWorldEntity_Awake(LargeWorldEntity __instance)
        {
            if (Main.Config.NoJellyshroomCavePopIn == ViewDistanceOption.Disabled) return;

            if (__instance.name.StartsWith("Crab_snake_mushrooms") ||
                __instance.name.StartsWith("coral_reef_Stalactite"))
            {
                var cellLevel = Main.Config.NoJellyshroomCavePopIn switch
                {
                    ViewDistanceOption.Medium => LargeWorldEntity.CellLevel.Medium,
                    ViewDistanceOption.Far => LargeWorldEntity.CellLevel.Far,
                    ViewDistanceOption.VeryFar => LargeWorldEntity.CellLevel.VeryFar,
                    _ => LargeWorldEntity.CellLevel.Near // Default fallback
                };

                __instance.cellLevel = cellLevel;
            }
        }
    }
}