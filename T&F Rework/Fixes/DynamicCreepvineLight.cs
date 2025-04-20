using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class DynamicCreepvineLight
    {
        [HarmonyPatch(typeof(FruitPlant), nameof(FruitPlant.Update)), HarmonyPostfix]
        public static void FruitPlant_Update(FruitPlant __instance)
        {
            if (!Main.Config.DynamicCreepvineLight) return;

            if (__instance == null) return;
            if (CraftData.GetTechType(__instance.gameObject) != TechType.Creepvine) return;

            Light light = __instance.GetComponentInChildren<Light>();
            if (!light) return;

            int activeFruits = 0;
            foreach (PickPrefab fruit in __instance.fruits)
            {
                if (fruit && fruit.gameObject.activeInHierarchy)
                    activeFruits++;
            }

            // Set light intensity based on the number of active fruits
            light.intensity = (float)activeFruits / __instance.fruits.Length;
        }
    }
}