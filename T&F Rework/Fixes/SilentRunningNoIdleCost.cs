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
                // Store original power cost
                if (originalPowerCost == -1f)
                {
                    originalPowerCost = subRoot.silentRunningPowerCost;
                }
            }
        }

        [HarmonyPatch(typeof(CyclopsSilentRunningAbilityButton), nameof(CyclopsSilentRunningAbilityButton.SilentRunningIteration)), HarmonyPrefix]
        public static void CyclopsSilentRunningAbilityButton_SilentRunningIteration(CyclopsSilentRunningAbilityButton __instance)
        {
            if (!Main.Config.SilentRunningNoIdleCost) return;

            // Cost 0 when engine off
            if (__instance.subRoot.noiseManager.noiseScalar == 0f)
            {
                __instance.subRoot.silentRunningPowerCost = 0f;
            }
            else // Original cost when engine on
            {
                __instance.subRoot.silentRunningPowerCost = originalPowerCost;
            }
        }
    }
}