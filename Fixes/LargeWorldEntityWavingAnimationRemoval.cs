using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    // Removed waving animations from indoor plants
    [HarmonyPatch(typeof(LargeWorldEntity))]
    public class FixLargeWorldEntityWavingAnimationRemoval
    {
        private static readonly HashSet<TechType> techTypesToRemoveWavingShader = new HashSet<TechType>
            {
                TechType.BulboTree,
                TechType.PurpleVasePlant,
                TechType.OrangePetalsPlant,
                TechType.PinkMushroom,
                TechType.PurpleRattle,
                TechType.PinkFlower
            };

        private static void DisableWavingShader(Component component)
        {
            foreach (var material in component.GetComponentsInChildren<MeshRenderer>().SelectMany(mr => mr.materials))
            {
                material.DisableKeyword("UWE_WAVING");
            }
        }

        [HarmonyPatch(nameof(LargeWorldEntity.Awake)), HarmonyPrefix]
        public static void Awake(LargeWorldEntity __instance)
        {
            if (Main.FixesConfig.NoPlantAnimationsIndoors)
            {
                var tt = CraftData.GetTechType(__instance.gameObject);
                if (techTypesToRemoveWavingShader.Contains(tt) && __instance.gameObject.GetComponentInParent<Base>(true))
                {
                    DisableWavingShader(__instance);
                }
            }
        }
    }
}