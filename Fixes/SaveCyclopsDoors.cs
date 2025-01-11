using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
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
        private static void CloseDoor(Openable openable)
        {
            openable.isOpen = false;
            Vector3 rot = openable.transform.localEulerAngles;
            if (closedDoorsYAngle.TryGetValue(openable.name, out float y))
            {
                openable.transform.localEulerAngles = new Vector3(rot.x, y, rot.z);
            }
        }

        [HarmonyPatch(typeof(Openable), nameof(Openable.OnHandClick)), HarmonyPostfix]
        public static void Openable_OnHandClick(Openable __instance)
        {
            if (!Main.Config.SaveClosedCyclopsDoors)
            {
                return;
            }

            var name = __instance.name;

            // Save when opening
            if (!__instance.isOpen && !__instance.IsSealed())
            {
                Main.SaveData.CyclopsClosedDoors.Add(name);
            }

            // Don't save when closing
            else
            {
                Main.SaveData.CyclopsClosedDoors.Remove(name);
            }
        }

        [HarmonyPatch(typeof(Openable), nameof(Openable.Start)), HarmonyPrefix]
        public static void Openable_Start(Openable __instance)
        {
            if (!Main.Config.SaveClosedCyclopsDoors)
            {
                return;
            }

            // Close if saved as closed
            if (closedDoorsYAngle.ContainsKey(__instance.name) &&
                Main.SaveData.CyclopsClosedDoors.Contains(__instance.name))
            {
                CloseDoor(__instance);
            }
        }
    }
}