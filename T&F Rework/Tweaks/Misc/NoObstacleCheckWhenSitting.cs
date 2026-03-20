using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class NoObstacleCheckWhenSitting
    {
        [HarmonyPatch(typeof(Bench), nameof(Bench.CanSit)), HarmonyPostfix]
        public static void Bench_CanSit(ref bool __result)
        {
            if (Main.Config.NoObstacleCheckWhenSitting)
            {
                __result = true;
            }
        }
    }
}