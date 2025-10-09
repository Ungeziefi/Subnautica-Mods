using HarmonyLib;
using System;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class LeviathansDontAttackLandTargets
    {
        [HarmonyPatch(typeof(AggressiveWhenSeeTarget), nameof(AggressiveWhenSeeTarget.IsTargetValid)), HarmonyPostfix]
        [HarmonyPatch(new Type[] { typeof(GameObject) })]
        public static void AggressiveWhenSeeTarget_IsTargetValid(AggressiveWhenSeeTarget __instance, ref bool __result, GameObject target)
        {
            if (!Main.Config.LeviathansDontAttackLandTargets) return;

            // Skip if already invalid
            if (!__result) return;

            if (CreatureData.GetBehaviourType(__instance.myTechType) == BehaviourType.Leviathan &&
                target.transform.position.y > Ocean.GetOceanLevel())
            {
                __result = false;
            }
        }
    }
}