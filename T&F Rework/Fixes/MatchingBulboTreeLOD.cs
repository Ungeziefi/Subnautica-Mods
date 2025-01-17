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
            if (!Main.Config.MatchingBulboTreeLOD) return;

            if (__instance?.gameObject?.name?.StartsWith("land_plant_middle_01") == true)
            {
                var lodGroup = __instance.gameObject.GetComponent<LODGroup>();
                if (lodGroup != null)
                {
                    // Disable LOD system
                    lodGroup.enabled = false;

                    // Disable lower quality renderers
                    var renderers = __instance.gameObject.GetComponentsInChildren<Renderer>();
                    for (int i = 1; i < renderers.Length; i++)
                    {
                        renderers[i].enabled = false;
                    }
                }
            }
        }
    }
}