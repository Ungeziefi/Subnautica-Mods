using System.Collections.Generic;
using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoDepositPopIn
    {
        // List of all drillable resource types
        private static readonly HashSet<TechType> drillables = new HashSet<TechType>
        {
            TechType.DrillableAluminiumOxide,
            TechType.DrillableCopper,
            TechType.DrillableDiamond,
            TechType.DrillableGold,
            TechType.DrillableKyanite,
            TechType.DrillableLead,
            TechType.DrillableLithium,
            TechType.DrillableMagnetite,
            TechType.DrillableMercury,
            TechType.DrillableNickel,
            TechType.DrillableQuartz,
            TechType.DrillableSalt,
            TechType.DrillableSilver,
            TechType.DrillableSulphur,
            TechType.DrillableTitanium,
            TechType.DrillableUranium
        };

        [HarmonyPatch(typeof(LargeWorldEntity), nameof(LargeWorldEntity.Awake)), HarmonyPostfix]
        private static void LargeWorldEntity_Awake(LargeWorldEntity __instance)
        {
            if (Main.Config.NoDepositPopIn == ViewDistanceOption.Disabled) return;

            TechType techType = CraftData.GetTechType(__instance.gameObject);
            if (drillables.Contains(techType))
            {
                // Map enum to cell level
                var cellLevel = Main.Config.NoDepositPopIn switch
                {
                    ViewDistanceOption.Medium => LargeWorldEntity.CellLevel.Medium,
                    ViewDistanceOption.Far => LargeWorldEntity.CellLevel.Far,
                    ViewDistanceOption.VeryFar => LargeWorldEntity.CellLevel.VeryFar,
                    _ => LargeWorldEntity.CellLevel.Near // Default
                };

                __instance.cellLevel = cellLevel;
            }
        }
    }
}