using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoPlantWavingIndoors
    {
        private static void DisableWavingShader(Component component)
        {
            foreach (var material in component.GetComponentsInChildren<MeshRenderer>().SelectMany(mr => mr.materials))
            {
                if (material.IsKeywordEnabled("UWE_WAVING"))
                {
                    material.DisableKeyword("UWE_WAVING");
                }
            }
        }

        [HarmonyPatch(typeof(LargeWorldEntity), nameof(LargeWorldEntity.Awake)), HarmonyPrefix]
        public static void LargeWorldEntity_Awake(LargeWorldEntity __instance)
        {
            if (!Main.Config.NoPlantWavingIndoors) return;

            if (__instance.gameObject.GetComponentInParent<Base>(true))
            {
                DisableWavingShader(__instance);
            }
        }


        // To-Do: When the plant is fully grown, neither GrowingPlant nor LargeWorldEntity work until a game restart
        [HarmonyPatch(typeof(GrowingPlant), nameof(GrowingPlant.Start)), HarmonyPrefix]
        public static void GrowingPlant_Start(GrowingPlant __instance)
        {
            if (!Main.Config.NoPlantWavingIndoors) return;

            if (__instance.gameObject.GetComponentInParent<Base>(true))
            {
                DisableWavingShader(__instance);
            }
        }
    }
}