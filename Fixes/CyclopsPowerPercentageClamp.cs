using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class CyclopsPowerPercentageClamp
    {
        [HarmonyPatch(typeof(CyclopsHelmHUDManager), nameof(CyclopsHelmHUDManager.Update)), HarmonyPostfix]
        static void CyclopsHelmHUDManager_Update(CyclopsHelmHUDManager __instance)
        {
            if (!Main.Config.CyclopsPowerPercentageClamp) return;

            __instance.lastPowerPctUsedForString = Mathf.Clamp(__instance.lastPowerPctUsedForString, 0, 100);
            __instance.powerText.text = string.Format("{0}%", __instance.lastPowerPctUsedForString);
        }
    }
}