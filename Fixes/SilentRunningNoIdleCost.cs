﻿using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(PowerRelay))]
    public class SilentRunningNoIdleCost_PowerRelay
    {
        public static float originalPowerCost = -1f;

        [HarmonyPatch(nameof(PowerRelay.ModifyPower)), HarmonyPostfix]
        public static void Postfix(PowerRelay __instance, float amount, float modified)
        {
            var subRoot = __instance.GetComponent<SubRoot>();
            if (subRoot != null && subRoot.isCyclops)
            {
                // Main.Logger.LogInfo($"Cyclops power modification: {amount}, modified: {modified}");

                // Store the original power cost when it's initialized
                if (originalPowerCost == -1f)
                {
                    originalPowerCost = subRoot.silentRunningPowerCost;
                    // Main.Logger.LogInfo($"Original power cost is: {originalPowerCost}");
                }
            }
        }
    }

    [HarmonyPatch(typeof(CyclopsSilentRunningAbilityButton))]
    public class SilentRunningNoIdleCost
    {
        [HarmonyPatch(nameof(CyclopsSilentRunningAbilityButton.SilentRunningIteration)), HarmonyPrefix]
        public static void SilentRunningIteration(CyclopsSilentRunningAbilityButton __instance)
        {
            if (!Main.Config.SilentRunningNoIdleCost)
            {
                return;
            }

            // Set silentRunningPowerCost to 0 when the engine is off
            if (__instance.subRoot.noiseManager?.noiseScalar == 0f)
            {
                __instance.subRoot.silentRunningPowerCost = 0f;
                // Main.Logger.LogInfo($"Current power cost is {__instance.subRoot.silentRunningPowerCost}");
            }

            // Restore the original power cost if the engine is on
            else
            {
                __instance.subRoot.silentRunningPowerCost = SilentRunningNoIdleCost_PowerRelay.originalPowerCost;
                // Main.Logger.LogInfo($"Current power cost is {__instance.subRoot.silentRunningPowerCost}");
            }
        }
    }
}