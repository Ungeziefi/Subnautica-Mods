using HarmonyLib;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoPlantWavingIndoors
    {
        private static bool ShouldDisableWaving(Component component) =>
            Main.Config.NoPlantWavingIndoors && component.gameObject.GetComponentInParent<Base>(true);

        private static void DisableWavingShader(Component component)
        {
            component.GetComponentsInChildren<MeshRenderer>()
                .SelectMany(mr => mr.materials)
                .Where(m => m.IsKeywordEnabled("UWE_WAVING"))
                .ToList()
                .ForEach(m => m.DisableKeyword("UWE_WAVING"));
        }

        [HarmonyPatch(typeof(LargeWorldEntity), nameof(LargeWorldEntity.Awake))]
        [HarmonyPatch(typeof(GrowingPlant), nameof(GrowingPlant.Start))]
        [HarmonyPrefix]
        public static void DisableWavingPrefix(Component __instance)
        {
            if (ShouldDisableWaving(__instance))
                DisableWavingShader(__instance);
        }

        [HarmonyPatch(typeof(GrowingPlant), nameof(GrowingPlant.SpawnGrownModelAsync))]
        public static class GrowingPlant_SpawnGrownModelAsync_Patch
        {
            public static void Postfix(GrowingPlant __instance, ref IEnumerator __result)
            {
                if (!ShouldDisableWaving(__instance)) return;
                __result = new IEnumerator[] { __result, HandleGrownModel(__instance) }.GetEnumerator();
            }

            private static IEnumerator HandleGrownModel(GrowingPlant instance)
            {
                if (instance.seed == null)
                    yield break;

                if (instance.seed.currentPlanter == null)
                    yield break;

                if (instance.seed.currentPlanter.grownPlantsRoot is Transform root)
                {
                    var activeChild = root.Cast<Transform>()
                        .FirstOrDefault(child => child.gameObject.activeSelf);

                    if (activeChild != null)
                        DisableWavingShader(activeChild);
                }
            }
        }
    }
}