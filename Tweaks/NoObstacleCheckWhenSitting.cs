using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch(typeof(Bench))]
    public class NoObstacleCheckWhenSitting
    {
        [HarmonyPatch(nameof(Bench.CanSit)), HarmonyPostfix]
        public static void CanSit(ref bool __result)
        {
            if (Main.Config.NoObstacleCheckWhenSitting)
            {
                __result = true;
            }
        }
    }
}