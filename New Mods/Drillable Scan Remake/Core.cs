// To-Do: Fix Kyanite and Language patching

using System.Collections.Generic;
using HarmonyLib;

namespace Ungeziefi.Drillable_Scan_Remake
{
    [HarmonyPatch]
    public class DrillableScanRemake
    {
        private static readonly Dictionary<string, TechType> drillableClassIdToNormalMap = new()
        {
            { "793b4079-ef3b-43da-9fc7-3ec5cbc3ae19", TechType.Salt },
            { "b3db72b6-f0cf-4234-be74-d98bd4c49797", TechType.Quartz },
            { "601ee500-1744-4697-8279-59ef35160edb", TechType.Copper },
            { "9f855246-76c4-438b-8e4d-9cd6d7ce4224", TechType.Titanium },
            { "1efa1a20-3a39-4f56-ace0-154211d6af12", TechType.Lead },
            { "026d91e2-430b-4c6d-8bd4-b51e270d5eed", TechType.Silver },
            { "e7c097ac-e7be-4808-aaaa-70178d96f68b", TechType.Diamond },
            { "a05fe1c9-ae0d-43db-a12c-865992808cb2", TechType.Gold },
            { "f67c158c-3b83-473c-ad52-93fd2eeef66b", TechType.Magnetite },
            { "846c3df6-ffbf-4206-b591-72f5ba11ed40", TechType.Lithium },
            { "06ada673-7d2b-454f-ae11-951d628e64a7", TechType.MercuryOre },
            { "fb5de2b6-1fe8-44fc-a555-dc0a09dc292a", TechType.Uranium },
            { "9c8f56e6-3380-42e4-a758-e8d733b5ddec", TechType.Nickel },
            { "697beac5-e39a-4809-854d-9163da9f997e", TechType.Sulphur },
            { "4f441e53-7a9a-44dc-83a4-b1791dc88ffd", TechType.Kyanite },
            { "853a9c5b-aba3-4d6b-a547-34553aa73fa9", TechType.Kyanite },
            { "109bbd29-c445-4ad8-a4bf-be7bc6d421d6", TechType.AluminumOxide }
        };

        // Append "Drillable"
        //[HarmonyPatch(typeof(Language), nameof(Language.Get)), HarmonyPostfix]
        //private static void Language_Get(ref string __result, string key)
        //{
        //    if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(__result))
        //        return;

        //    if (key.IndexOf("drillable", StringComparison.OrdinalIgnoreCase) >= 0)
        //    {
        //        __result += " (Drillable)";
        //    }
        //}

        // Override TechType
        [HarmonyPatch(typeof(ResourceTracker), nameof(ResourceTracker.Start)), HarmonyPrefix]
        private static bool ResourceTracker_Start(ResourceTracker __instance)
        {
            var classId = CraftData.GetClassIdForTechType(CraftData.GetTechType(__instance.gameObject));
            if (!string.IsNullOrEmpty(classId) && drillableClassIdToNormalMap.ContainsKey(classId))
            {
                __instance.overrideTechType = TechType.None;
            }
            return true;
        }

        // Set icon back to the normal TechType
        [HarmonyPatch(typeof(uGUI_MapRoomResourceNode), nameof(uGUI_MapRoomResourceNode.SetTechType)), HarmonyPostfix]
        private static void MapRoomResourceNode_SetTechType(uGUI_MapRoomResourceNode __instance, TechType techType)
        {
            var classId = CraftData.GetClassIdForTechType(techType);
            if (!string.IsNullOrEmpty(classId) && drillableClassIdToNormalMap.TryGetValue(classId, out TechType normalType))
            {
                __instance.icon.sprite = SpriteManager.Get(normalType);
                __instance.icon.enabled = true;
            }
        }

        // Set icon back to the normal TechType
        [HarmonyPatch(typeof(uGUI_MapRoomScanner), nameof(uGUI_MapRoomScanner.UpdateGUIState)), HarmonyPostfix]
        private static void MapRoomScanner_UpdateGUIState(uGUI_MapRoomScanner __instance)
        {
            TechType activeTechType = __instance.mapRoom.GetActiveTechType();
            var classId = CraftData.GetClassIdForTechType(activeTechType);
            if (!string.IsNullOrEmpty(classId) && drillableClassIdToNormalMap.TryGetValue(classId, out TechType normalType))
            {
                __instance.scanningIcon.sprite = SpriteManager.Get(normalType);
                __instance.scanningIcon.enabled = true;
            }
        }
    }
}