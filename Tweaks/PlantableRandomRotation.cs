﻿using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch(typeof(Plantable))]
    public class TweakPlantableRandomRotation
    {
        [HarmonyPatch(nameof(Plantable.Spawn)), HarmonyPostfix]
        public static void Spawn(ref GameObject __result)
        {
            if (Main.TweaksConfig.PlantRotationRandomizer)
            {
                float randomYRotation = Random.Range(0, 360);
                __result.transform.Rotate(0, randomYRotation, 0);
            }
            // Main.Logger.LogInfo($"Plant rotation is {__result.transform.eulerAngles}");
        }
    }
}