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
            if (!Main.Config.PlantRotationRandomizer) return;

            var euler = __result.transform.eulerAngles;
            euler.y = Random.Range(0f, 360f);
            __result.transform.eulerAngles = euler;
        }
    }
}