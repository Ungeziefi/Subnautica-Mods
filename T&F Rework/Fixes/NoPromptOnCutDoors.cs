using HarmonyLib;
using System.Collections.Generic;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoPromptOnCutDoors
    {
        // Track cut open doors
        private static readonly HashSet<StarshipDoor> cutOpenedDoors = new HashSet<StarshipDoor>();

        // Checks if door has been cut open
        private static bool IsDoorCutOpen(StarshipDoor door)
        {
            var laserCutObject = door.GetComponent<LaserCutObject>();
            return laserCutObject != null && laserCutObject.isCutOpen;
        }

        [HarmonyPatch(typeof(StarshipDoor), nameof(StarshipDoor.OnHandHover)), HarmonyPrefix]
        public static bool StarshipDoor_OnHandHover(StarshipDoor __instance)
        {
            if (!Main.Config.NoPromptOnCutDoors) return true;

            // Add to the set and stop hover text if cut open
            if (IsDoorCutOpen(__instance))
            {
                cutOpenedDoors.Add(__instance);
                return false;
            }

            // Allow hover text if not cut open
            return !cutOpenedDoors.Contains(__instance);
        }
    }
}