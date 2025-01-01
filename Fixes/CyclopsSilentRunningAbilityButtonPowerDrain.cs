using HarmonyLib;

namespace Ungeziefi.Fixes
{
    // Print the current power drain of the Cyclops
    //[HarmonyPatch(typeof(PowerRelay))]
    //public class DebugCyclopsPowerDrain
    //{
    //    [HarmonyPatch(nameof(PowerRelay.ModifyPower)), HarmonyPostfix]
    //    public static void Postfix(PowerRelay __instance, float amount, float modified)
    //    {
    //        var subRoot = __instance.GetComponent<SubRoot>();
    //        if (subRoot != null && subRoot.isCyclops)
    //        {
    //            Main.Logger.LogInfo($"Cyclops power modification: {amount}, modified: {modified}");
    //        }
    //    }
    //}

    // Prevent power drain when using the silent mode of the Cyclops
    [HarmonyPatch(typeof(CyclopsSilentRunningAbilityButton))]
    public class FixCyclopsSilentRunningAbilityButtonPowerDrain
    {
        [HarmonyPatch(nameof(CyclopsSilentRunningAbilityButton.SilentRunningIteration)), HarmonyPrefix]
        public static void SilentRunningIteration(CyclopsSilentRunningAbilityButton __instance)
        {
            if (!Main.FixesConfig.SilentRunningNoIdleCost)
            {
                return;
            }

            // Set silentRunningPowerCost to 0 when the engine is off
            if (__instance.subRoot.noiseManager?.noiseScalar == 0f)
            {
                __instance.subRoot.silentRunningPowerCost = 0f;
                // Main.Logger.LogInfo($"Current power cost is {__instance.subRoot.silentRunningPowerCost}");
            }
            else
            {
                // Restore the original power cost if the engine is on
                __instance.subRoot.silentRunningPowerCost = 5f;
                // Main.Logger.LogInfo($"Current power cost is {__instance.subRoot.silentRunningPowerCost}");
            }
        }
    }
}