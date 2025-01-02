using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(Openable))]
    public class SaveCyclopsClosedDoors
    {
        private static readonly Dictionary<string, float> closedDoorsYAngle = new Dictionary<string, float>
        {
            { "submarine_hatch_01", 270f },
            { "submarine_hatch_02", 180f },
            { "submarine_hatch_02 1", 180f },
            { "submarine_hatch_02 3", 180f },
            { "submarine_hatch_02 4", 0f },
            { "submarine_hatch_02 7", 0f }
        };

        [HarmonyPatch(nameof(Openable.OnHandClick)), HarmonyPostfix]
        public static void OnHandClick(Openable __instance)
        {
            var name = __instance.name;

            // Open the door
            if (!__instance.isOpen && !__instance.IsSealed())
            {
                Main.SaveData.CyclopsClosedDoors.Add(name);
            }
            // Close the door
            else
            {
                // No point in saving it as closed because it's the default state
                Main.SaveData.CyclopsClosedDoors.Remove(name);
            }
        }

        private static void CloseDoor(Openable openable)
        {
            openable.isOpen = false;
            Vector3 rot = openable.transform.localEulerAngles;
            if (closedDoorsYAngle.TryGetValue(openable.name, out float y))
            {
                openable.transform.localEulerAngles = new Vector3(rot.x, y, rot.z);
            }
        }

        [HarmonyPatch(nameof(Openable.Start)), HarmonyPrefix]
        public static void Start(Openable __instance)
        {
            if (!Main.PersistenceConfig.SaveClosedCyclopsDoors)
            {
                return;
            }

            // Check if this door has a defined closing angle and if it's in the closed doors list
            if (closedDoorsYAngle.ContainsKey(__instance.name) &&
                Main.SaveData.CyclopsClosedDoors.Contains(__instance.name))
            {
                CloseDoor(__instance);
            }
        }
    }
}