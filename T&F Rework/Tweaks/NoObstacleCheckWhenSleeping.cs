using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class NoObstacleCheckWhenSleeping
    {
        [HarmonyPatch(typeof(Bed), nameof(Bed.CheckForSpace)), HarmonyPostfix]
        public static void Bed_CheckForSpace(ref bool __result)
        {
            if (Main.Config.NoObstacleCheckWhenSitting)
            {
                __result = true;
            }
        }
    }
}