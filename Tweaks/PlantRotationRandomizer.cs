using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class PlantRotationRandomizer
    {
        [HarmonyPatch(typeof(Plantable), nameof(Plantable.Spawn)), HarmonyPostfix]
        public static void Plantable_Spawn(ref GameObject __result)
        {
            if (Main.Config.PlantRotationRandomizer)
            {
                float randomYRotation = Random.Range(0, 360);
                __result.transform.Rotate(0, randomYRotation, 0);
            }
        }
    }
}