using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(BulkheadDoor))]
    public class SaveOpenWreckDoors
    {
        private static string GetDoorCoords(Vector3Int position)
        {
            return $"{position.x},{position.y},{position.z}";
        }

        [HarmonyPatch(nameof(BulkheadDoor.OnHandClick))]
        public static void Postfix(BulkheadDoor __instance)
        {
            var pos = new Vector3Int(
                Mathf.RoundToInt(__instance.transform.position.x),
                Mathf.RoundToInt(__instance.transform.position.y),
                Mathf.RoundToInt(__instance.transform.position.z)
            );

            string doorCoords = GetDoorCoords(pos);

            if (!__instance.opened)
            {
                // Open the door
                Main.SaveData.OpenWreckDoors[doorCoords] = true;
            }
            else
            {
                // Close the door
                Main.SaveData.OpenWreckDoors.Remove(doorCoords);
            }
        }

        [HarmonyPatch(nameof(BulkheadDoor.Awake))]
        public static void Prefix(BulkheadDoor __instance)
        {
            if (!Main.Config.SaveOpenWreckDoors)
            {
                return;
            }

            var pos = new Vector3Int(
                Mathf.RoundToInt(__instance.transform.position.x),
                Mathf.RoundToInt(__instance.transform.position.y),
                Mathf.RoundToInt(__instance.transform.position.z)
            );

            string doorKey = GetDoorCoords(pos);

            // Check if this door was saved as open
            if (Main.SaveData.OpenWreckDoors.TryGetValue(doorKey, out bool isOpen) && isOpen)
            {
                __instance.initiallyOpen = true;
            }
        }
    }
}