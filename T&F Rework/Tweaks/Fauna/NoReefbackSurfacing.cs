using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class NoReefbackSurfacing
    {
        private const float MIN_REEFBACK_DEPTH = -15f;

        [HarmonyPatch(typeof(SwimBehaviour), nameof(SwimBehaviour.SwimToInternal)), HarmonyPrefix]
        public static void SwimBehaviour_SwimToInternal(SwimBehaviour __instance, ref Vector3 targetPosition)
        {
            if (!Main.Config.NoReefbackSurfacing) return;

            TechType techType = CraftData.GetTechType(__instance.gameObject);
            if (techType == TechType.Reefback && targetPosition.y > MIN_REEFBACK_DEPTH)
            {
                targetPosition.y = MIN_REEFBACK_DEPTH;
            }
        }
    }
}