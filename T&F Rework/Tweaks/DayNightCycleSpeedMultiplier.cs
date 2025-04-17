using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch(typeof(DayNightCycle))]
    internal class DayNightCycleSpeedMultiplier
    {
        private static bool skipTimeModeStopped;

        private static void SetSpeed(DayNightCycle instance)
        {
            float multiplier = Main.Config.DayNightCycleSpeedMultiplier;
            if (multiplier != 1f)
            {
                instance._dayNightSpeed *= multiplier;
            }
        }

        // Set initial speed
        [HarmonyPatch(nameof(DayNightCycle.Awake)), HarmonyPostfix]
        static void DayNightCycle_Awake(DayNightCycle __instance) => SetSpeed(__instance);

        // Check if time skip is ending
        [HarmonyPatch(nameof(DayNightCycle.Update)), HarmonyPrefix]
        static void DayNightCycle_UpdatePrefix(DayNightCycle __instance)
        {
            if (!__instance.debugFreeze && __instance.skipTimeMode)
            {
                double timePassed = __instance.timePassedAsDouble + __instance.deltaTime;
                if (timePassed >= __instance.skipModeEndTime)
                    skipTimeModeStopped = true;
            }
        }

        // Reset speed after time skip ends
        [HarmonyPatch(nameof(DayNightCycle.Update)), HarmonyPostfix]
        static void DayNightCycle_UpdatePostfix(DayNightCycle __instance)
        {
            if (skipTimeModeStopped)
            {
                skipTimeModeStopped = false;
                SetSpeed(__instance);
            }
        }

        // Reset speed after game resume
        [HarmonyPatch(nameof(DayNightCycle.Resume)), HarmonyPostfix]
        static void DayNightCycle_Resume(DayNightCycle __instance) => SetSpeed(__instance);

        // Reset speed after day command
        [HarmonyPatch(nameof(DayNightCycle.OnConsoleCommand_day)), HarmonyPostfix]
        static void DayNightCycle_OnConsoleCommand_day(DayNightCycle __instance) => SetSpeed(__instance);

        // Reset speed after night command
        [HarmonyPatch(nameof(DayNightCycle.OnConsoleCommand_night)), HarmonyPostfix]
        static void DayNightCycle_OnConsoleCommand_night(DayNightCycle __instance) => SetSpeed(__instance);

        // Reset speed after daynight command
        [HarmonyPatch(nameof(DayNightCycle.OnConsoleCommand_daynight)), HarmonyPostfix]
        static void DayNightCycle_OnConsoleCommand_daynight(DayNightCycle __instance) => SetSpeed(__instance);

        // Reset speed after skip time mode ends
        [HarmonyPatch(nameof(DayNightCycle.StopSkipTimeMode)), HarmonyPostfix]
        static void DayNightCycle_StopSkipTimeMode(DayNightCycle __instance) => SetSpeed(__instance);
    }
}
