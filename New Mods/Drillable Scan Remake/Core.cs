using HarmonyLib;
using System;
using System.Collections.Generic;

namespace Ungeziefi.Drillable_Scan_Remake
{
    [HarmonyPatch]
    public class DrillableScanRemake
    {
        private static readonly Dictionary<TechType, TechType> drillableToNormalMap = new()
        {
            { TechType.DrillableSalt, TechType.Salt },
            { TechType.DrillableQuartz, TechType.Quartz },
            { TechType.DrillableCopper, TechType.Copper },
            { TechType.DrillableTitanium, TechType.Titanium },
            { TechType.DrillableLead, TechType.Lead },
            { TechType.DrillableSilver, TechType.Silver },
            { TechType.DrillableDiamond, TechType.Diamond },
            { TechType.DrillableGold, TechType.Gold },
            { TechType.DrillableMagnetite, TechType.Magnetite },
            { TechType.DrillableLithium, TechType.Lithium },
            { TechType.DrillableMercury, TechType.MercuryOre },
            { TechType.DrillableUranium, TechType.Uranium },
            { TechType.DrillableNickel, TechType.Nickel },
            { TechType.DrillableSulphur, TechType.Sulphur },
            { TechType.DrillableKyanite, TechType.Kyanite },
            { TechType.DrillableAluminiumOxide, TechType.AluminumOxide }
        };

        // Append "Drillable"
        [HarmonyPatch(typeof(Language), nameof(Language.Get)), HarmonyPrefix]
        private static bool Language_Get(Language __instance, ref string __result, string key)
        {
            // Fix NREs when changing hotkeys
            if (__instance == null || string.IsNullOrEmpty(key))
                return true;

            if (key.IndexOf("drillable", StringComparison.OrdinalIgnoreCase) >= 0 &&
                __instance.TryGet(key, out __result))
            {
                __result += " (Drillable)";
                return false;
            }
            return true;
        }

        // Override TechType
        [HarmonyPatch(typeof(ResourceTracker), nameof(ResourceTracker.Start)), HarmonyPrefix]
        private static bool ResourceTracker_Start(ResourceTracker __instance)
        {
            // From 70 to 84 of the enum
            var techType = CraftData.GetTechType(__instance.gameObject);
            if (techType >= TechType.DrillableSalt && techType <= TechType.DrillableAluminiumOxide)
            {
                __instance.overrideTechType = TechType.None;
            }
            return true;
        }

        // Set icon
        [HarmonyPatch(typeof(uGUI_MapRoomResourceNode), nameof(uGUI_MapRoomResourceNode.SetTechType)), HarmonyPostfix]
        private static void MapRoomResourceNode_SetTechType(uGUI_MapRoomResourceNode __instance, TechType techType)
        {
            if (drillableToNormalMap.TryGetValue(techType, out TechType normalType))
            {
                __instance.icon.sprite = SpriteManager.Get(normalType);
                __instance.icon.enabled = true;
            }
        }

        // Set icon
        [HarmonyPatch(typeof(uGUI_MapRoomScanner), nameof(uGUI_MapRoomScanner.UpdateGUIState)), HarmonyPostfix]
        private static void MapRoomScanner_UpdateGUIState(uGUI_MapRoomScanner __instance)
        {
            TechType activeTechType = __instance.mapRoom.GetActiveTechType();
            if (drillableToNormalMap.TryGetValue(activeTechType, out TechType normalType))
            {
                __instance.scanningIcon.sprite = SpriteManager.Get(normalType);
                __instance.scanningIcon.enabled = true;
            }
        }
    }
}