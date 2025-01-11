using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class SilentRunningNoIdleCost
    {
        public static float originalPowerCost = -1f;

        [HarmonyPatch(typeof(PowerRelay), nameof(PowerRelay.ModifyPower)), HarmonyPostfix]
        public static void PowerRelay_ModifyPower(PowerRelay __instance, float amount, float modified)
        {
            var subRoot = __instance.GetComponent<SubRoot>();
            if (subRoot != null && subRoot.isCyclops)
            {
                // Main.Logger.LogInfo($"Cyclops power modification: {amount}, modified: {modified}");

                // Store original power cost
                if (originalPowerCost == -1f)
                {
                    originalPowerCost = subRoot.silentRunningPowerCost;
                    // Main.Logger.LogInfo($"Original power cost is: {originalPowerCost}");
                }
            }
        }

        [HarmonyPatch(typeof(CyclopsSilentRunningAbilityButton), nameof(CyclopsSilentRunningAbilityButton.SilentRunningIteration)), HarmonyPrefix]
        public static void CyclopsSilentRunningAbilityButton_SilentRunningIteration(CyclopsSilentRunningAbilityButton __instance)
        {
            if (!Main.Config.SilentRunningNoIdleCost)
            {
                return;
            }

            // Cost 0 when engine off
            if (__instance.subRoot.noiseManager?.noiseScalar == 0f)
            {
                __instance.subRoot.silentRunningPowerCost = 0f;
                // Main.Logger.LogInfo($"Current power cost is {__instance.subRoot.silentRunningPowerCost}");
            }

            // Original cost when engine on
            else
            {
                __instance.subRoot.silentRunningPowerCost = originalPowerCost;
                // Main.Logger.LogInfo($"Current power cost is {__instance.subRoot.silentRunningPowerCost}");
            }
        }
    }
}