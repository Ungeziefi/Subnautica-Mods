using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    // Save the state of bulkhead doors
    [HarmonyPatch(typeof(BulkheadDoor))]
    public class FixBulkheadDoorState
    {
        private static readonly Dictionary<string, HashSet<Vector3Int>> openedWreckDoors = new Dictionary<string, HashSet<Vector3Int>>();

        [HarmonyPatch(nameof(BulkheadDoor.OnHandClick))]
        public static void Postfix(BulkheadDoor __instance)
        {
            var pos = new Vector3Int((int)__instance.transform.position.x, (int)__instance.transform.position.y, (int)__instance.transform.position.z);
            var slot = SaveLoadManager.main.currentSlot;
            if (!openedWreckDoors.ContainsKey(slot))
            {
                openedWreckDoors[slot] = new HashSet<Vector3Int>();
            }

            if (__instance.opened)
            {
                openedWreckDoors[slot].Remove(pos);
                // Main.Logger.LogInfo($"Door at {pos} in slot {slot} closed and state saved.");
            }

            else
            {
                openedWreckDoors[slot].Add(pos);
                // Main.Logger.LogInfo($"Door at {pos} in slot {slot} opened and state saved.");
            }
        }

        [HarmonyPatch(nameof(BulkheadDoor.Awake))]
        public static void Prefix(BulkheadDoor __instance)
        {
            var pos = new Vector3Int((int)__instance.transform.position.x, (int)__instance.transform.position.y, (int)__instance.transform.position.z);
            var slot = SaveLoadManager.main.currentSlot;
            if (openedWreckDoors.ContainsKey(slot) && openedWreckDoors[slot].Contains(pos))
            {
                __instance.initiallyOpen = true;
            }
        }
    }
}