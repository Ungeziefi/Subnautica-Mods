using System.Collections.Generic;
using HarmonyLib;

namespace Ungeziefi.Fixes
{
    // Prevent interaction with cut-open doors
    [HarmonyPatch(typeof(StarshipDoor))]
    public class FixStarshipDoorInteraction
    {
        private static readonly HashSet<StarshipDoor> cutOpenedDoors = new HashSet<StarshipDoor>();

        [HarmonyPatch(nameof(StarshipDoor.OnHandHover))]
        public static bool Prefix(StarshipDoor __instance)
        {
            if (cutOpenedDoors.Contains(__instance))
            {
                return false;
            }

            var laserCutObject = __instance.GetComponent<LaserCutObject>();
            if (laserCutObject != null && laserCutObject.isCutOpen)
            {
                cutOpenedDoors.Add(__instance);
                return false;
            }

            return true;
        }
    }
}