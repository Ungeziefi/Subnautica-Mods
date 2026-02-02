using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    internal class OxygenPipesAcrossSubBiomes
    {
        [HarmonyPatch(typeof(AtmosphereVolume), nameof(AtmosphereVolume.Start)), HarmonyPostfix]
        public static void AtmosphereVolume_Start(AtmosphereVolume __instance)
        {
            if (!Main.Config.OxygenPipesAcrossSubBiomes) return;

            // Prevent turning every creature orange in the PCF prison biomes
            HashSet<string> excludedBiomes = new() {
                "Prison_Moonpool",
                "Prison_Antechamber",
                "Prison_UpperRoom",
                "Prison_Aquarium_Mid",
                "Prison_Aquarium_Upper",
                "Prison_Aquarium",
                "Prison_Aquarium_Cave"
                };

            if (excludedBiomes.Contains(__instance.overrideBiome)) return;

            int noRayCastLayer = LayerMask.NameToLayer("Ignore Raycast");
            __instance.gameObject.layer = noRayCastLayer;
        }
    }
}
