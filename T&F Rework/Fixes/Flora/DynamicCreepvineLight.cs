using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes.Flora;

[HarmonyPatch]
public class DynamicCreepvineLight
{
    [HarmonyPatch(typeof(FruitPlant), nameof(FruitPlant.Update))]
    [HarmonyPostfix]
    public static void FruitPlant_Update(FruitPlant __instance)
    {
        if (!Main.Config.DynamicCreepvineLight) return;

        if (__instance == null) return;
        if (CraftData.GetTechType(__instance.gameObject) != TechType.Creepvine) return;

        var light = __instance.GetComponentInChildren<Light>();
        if (!light) return;

        var activeFruits = 0;
        foreach (var fruit in __instance.fruits)
            if (fruit && fruit.gameObject.activeInHierarchy)
                activeFruits++;

        // Set light intensity based on the number of active fruits
        light.intensity = (float)activeFruits / __instance.fruits.Length;
    }
}