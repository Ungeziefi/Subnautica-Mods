using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class TreadersCanAttack
    {
        [HarmonyPatch(typeof(SeaTreaderMeleeAttack), nameof(SeaTreaderMeleeAttack.GetCanAttack)), HarmonyPostfix]
        static void SeaTreaderMeleeAttack_GetCanAttack(SeaTreaderMeleeAttack __instance, GameObject otherGameObject, ref bool __result)
        {
            if (!Main.Config.TreadersCanAttack) return;

            // Removed onSurface condition
            __result = !__instance.frozen &&
                      !__instance.treader.cinematicMode &&
                      Time.time > __instance.lastAttackTime + __instance.attackInterval &&
                      __instance.GetCanHit(otherGameObject);
        }
    }
}