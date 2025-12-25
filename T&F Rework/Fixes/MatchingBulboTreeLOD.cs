using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class MatchingBulboTreeLOD
    {
        [HarmonyPatch(typeof(LargeWorldEntity), nameof(LargeWorldEntity.Awake)), HarmonyPostfix]
        private static void LargeWorldEntity_Awake(LargeWorldEntity __instance)
        {
            if (!Main.Config.MatchingBulboTreeLOD)
                return;

            if (__instance == null || __instance.gameObject == null)
                return;

            if (!__instance.gameObject.name.StartsWith("land_plant_middle_01"))
                return;

            LODGroup lodGroup = __instance.gameObject.GetComponent<LODGroup>();
            if (lodGroup != null) lodGroup.enabled = false;
        }
    }
}