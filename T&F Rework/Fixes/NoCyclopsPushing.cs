using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoCyclopsPushing
    {
        [HarmonyPatch(typeof(MeleeAttack), nameof(MeleeAttack.CanDealDamageTo)), HarmonyPostfix]
        public static void Postfix(MeleeAttack __instance, GameObject target, ref bool __result)
        {
            if (!Main.Config.NoCyclopsPushing) return;

            if (!__result && target.GetComponent<SubControl>() != null)
            {
                __result = __instance.canBiteCyclops;
            }
        }
    } 
}