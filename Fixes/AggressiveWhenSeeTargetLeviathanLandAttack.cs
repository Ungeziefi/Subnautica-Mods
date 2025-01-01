using System;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{ 
    [HarmonyPatch(typeof(AggressiveWhenSeeTarget))]
    public class FixAggressiveWhenSeeTargetLeviathanLandAttack
    {
        [HarmonyPatch(nameof(AggressiveWhenSeeTarget.IsTargetValid)), HarmonyPostfix]
        [HarmonyPatch(new Type[] { typeof(GameObject) })]
        public static void IsTargetValid(AggressiveWhenSeeTarget __instance, ref bool __result, GameObject target)
        {
            if (!Main.FixesConfig.LeviathansDontAttackLandTargets)
            {
                return;
            }

            if (CreatureData.GetBehaviourType(__instance.myTechType) == BehaviourType.Leviathan && target.transform.position.y > Ocean.GetOceanLevel())
            {
                __result = false;
                // Main.Logger.LogInfo("Leviathan target is on land, attack aborted.");
            }

            else
            {
                __result = true;
                // Main.Logger.LogInfo("Leviathan target is valid.");
            }
        }
    }
}