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

            // Open the door
            if (!__instance.opened)
            {
                Main.SaveData.OpenWreckDoors.Add(doorCoords);
            }

            // Close the door
            else
            {
                // No point in saving it as closed because it's the default state
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
            if (Main.SaveData.OpenWreckDoors.Contains(doorKey))
            {
                __instance.initiallyOpen = true;
            }
        }
    }
}