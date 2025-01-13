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
                Vector3 Rotation = __result.transform.rotation.eulerAngles;
                float randomYRotation = Random.Range(0, 360);
                __result.transform.Rotate(Rotation.x, randomYRotation, Rotation.z);
            }
        }
    }
}