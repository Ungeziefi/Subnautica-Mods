using System.Collections.Generic;
using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(StarshipDoor))]
    public class NoPromptOnCutDoors
    {
        private static readonly HashSet<StarshipDoor> cutOpenedDoors = new HashSet<StarshipDoor>();
        private static bool IsDoorCutOpen(StarshipDoor door)
        {
            var laserCutObject = door.GetComponent<LaserCutObject>();
            return laserCutObject != null && laserCutObject.isCutOpen;
        }

        [HarmonyPatch(nameof(StarshipDoor.OnHandHover)), HarmonyPrefix]
        public static bool OnHandHover(StarshipDoor __instance)
        {
            if (!Main.Config.NoPromptOnCutDoors)
            {
                return true;
            }

            if (IsDoorCutOpen(__instance))
            {
                cutOpenedDoors.Add(__instance);
                return false;
            }

            return !cutOpenedDoors.Contains(__instance);
        }
    }
}