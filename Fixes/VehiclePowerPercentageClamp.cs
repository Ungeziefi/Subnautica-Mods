using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(CyclopsHelmHUDManager))]
    public static class VehiclePowerPercentageClamp
    {
        [HarmonyPatch(nameof(CyclopsHelmHUDManager.Update)), HarmonyPostfix]
        static void Postfix(CyclopsHelmHUDManager __instance)
        {
            if (Main.Config.VehiclePowerPercentageClamp)
            {
                __instance.lastPowerPctUsedForString = Mathf.Clamp(__instance.lastPowerPctUsedForString, 0, 100);
                __instance.powerText.text = string.Format("{0}%", __instance.lastPowerPctUsedForString);
            }
        }
    }
}