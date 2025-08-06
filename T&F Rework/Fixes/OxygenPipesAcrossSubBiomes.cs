using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    internal class OxygenPipesAcrossSubBiomes
    {
        [HarmonyPatch(typeof(AtmosphereVolume), nameof(AtmosphereVolume.Start)), HarmonyPostfix]
        public static void AtmosphereVolume_Start(DisplayManager __instance)
        {
            if (Main.Config.OxygenPipesAcrossSubBiomes)
            {
                int noRayCastLayer = LayerMask.NameToLayer("Ignore Raycast");
                __instance.gameObject.layer = noRayCastLayer;
            }
        }
    }
}
