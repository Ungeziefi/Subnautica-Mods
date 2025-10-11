using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoPromptOnCutDoors
    {
        private static bool IsDoorCutOpen(StarshipDoor door)
        {
            var laserCutObject = door.GetComponent<LaserCutObject>();
            return laserCutObject != null && laserCutObject.isCutOpen;
        }

        [HarmonyPatch(typeof(StarshipDoor), nameof(StarshipDoor.OnHandHover)), HarmonyPrefix]
        public static bool StarshipDoor_OnHandHover(StarshipDoor __instance)
        {
            if (!Main.Config.NoPromptOnCutDoors) return true;

            return !IsDoorCutOpen(__instance);
        }
    }
}