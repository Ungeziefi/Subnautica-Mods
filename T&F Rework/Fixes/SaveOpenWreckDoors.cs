using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(BulkheadDoor))]
    public class SaveOpenWreckDoors
    {
        private static string GetDoorCoords(Vector3Int position)
        {
            // "Harmony non-ref patch parameter position.x modified. This assignment have no effect."
            // Safe to ignore because we're only reading
#pragma warning disable Harmony003
            return $"{position.x},{position.y},{position.z}";
#pragma warning restore Harmony003
        }

        [HarmonyPatch(nameof(BulkheadDoor.OnHandClick)), HarmonyPostfix]
        public static void OnHandClick(BulkheadDoor __instance)
        {
            var pos = new Vector3Int(
                Mathf.RoundToInt(__instance.transform.position.x),
                Mathf.RoundToInt(__instance.transform.position.y),
                Mathf.RoundToInt(__instance.transform.position.z)
            );
            string doorCoords = GetDoorCoords(pos);

            // Save if opening, remove if closing
            if (!__instance.opened)
                Main.SaveData.OpenWreckDoors.Add(doorCoords);
            else
                Main.SaveData.OpenWreckDoors.Remove(doorCoords);
        }

        [HarmonyPatch(nameof(BulkheadDoor.Awake)), HarmonyPrefix]
        public static void Awake(BulkheadDoor __instance)
        {
            if (!Main.Config.SaveOpenWreckDoors) return;

            var pos = new Vector3Int(
                Mathf.RoundToInt(__instance.transform.position.x),
                Mathf.RoundToInt(__instance.transform.position.y),
                Mathf.RoundToInt(__instance.transform.position.z)
            );
            string doorKey = GetDoorCoords(pos);

            // Check if this door was saved as open
            if (Main.SaveData.OpenWreckDoors.Contains(doorKey))
                __instance.initiallyOpen = true;
        }
    }
}