using HarmonyLib;

namespace Ungeziefi.Fixes
{
    // Print the current power drain of the Cyclops
    //[HarmonyPatch(typeof(PowerRelay))]
    //public class DebugCyclopsPowerDrain
    //{
    //    [HarmonyPatch(nameof(PowerRelay.ModifyPower))]
    //    public static void Postfix(PowerRelay __instance, float amount, float modified)
    //    {
    //        if (__instance.GetComponent<SubRoot>() is SubRoot sub && sub.isCyclops)
    //        {
    //            Main.Logger.LogInfo($"Cyclops power modification: {amount}, modified: {modified}");
    //        }
    //    }
    //}

    // Prevent power drain when using the silent mode of the submarine
    [HarmonyPatch(typeof(CyclopsSilentRunningAbilityButton))]
    public class FixCyclopsSilentRunningAbilityButtonPowerDrain
    {
        [HarmonyPatch(nameof(CyclopsSilentRunningAbilityButton.SilentRunningIteration))]
        public static bool Prefix(CyclopsSilentRunningAbilityButton __instance)
        {
            // Don't consume power when the engine is off
            if (Player.main.currentSub != null && Player.main.currentSub.noiseManager != null && Player.main.currentSub.noiseManager.noiseScalar == 0f)
            {
                return false;
            }

            // Consume energy for silent running mode
            if (__instance.subRoot.powerRelay.ConsumeEnergy(__instance.subRoot.silentRunningPowerCost, out float amountConsumed))
            {
                // Power consumption successful, continue silent running
                return false;
            }

            // Turn off silent running if power consumption fails
            __instance.TurnOffSilentRunning();
            return false;
        }
    }
}