using System.Collections.Generic;
using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(StarshipDoor))]
    public class NoPromptOnCutDoors
    {
        private static readonly HashSet<StarshipDoor> cutOpenedDoors = new HashSet<StarshipDoor>();

        [HarmonyPatch(nameof(StarshipDoor.OnHandHover)), HarmonyPrefix]
        public static bool OnHandHover(StarshipDoor __instance)
        {
            if (!Main.Config.NoPromptOnCutDoors)
            {
                return true;
            }

            var laserCutObject = __instance.GetComponent<LaserCutObject>();
            if (laserCutObject != null && laserCutObject.isCutOpen)
            {
                cutOpenedDoors.Add(__instance);
            }

            if (cutOpenedDoors.Contains(__instance))
            {
                return false;
            }

            return true;
        }
    }
}