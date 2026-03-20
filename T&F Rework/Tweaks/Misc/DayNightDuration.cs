using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch(typeof(DayNightCycle))]
    public class DayNightDuration
    {
        private const float kVanillaCycleLengthSeconds = 1200f;

        public static float GetCycleLengthSeconds()
        {
            float totalMinutes = Main.Config.DayDurationMinutes + Main.Config.NightDurationMinutes;
            return totalMinutes * 60f;
        }

        private static float CalculateDayNightSpeed()
        {
            return kVanillaCycleLengthSeconds / GetCycleLengthSeconds();
        }

        private static float SetDayNightDuration(DayNightCycle instance)
        {
            float dayMinutes = Main.Config.DayDurationMinutes;
            float nightMinutes = Main.Config.NightDurationMinutes;
            float totalMinutes = dayMinutes + nightMinutes;
            float dayFraction = dayMinutes / totalMinutes;

            instance.sunRiseTime = (1f - dayFraction) / 2f;
            instance.sunSetTime = instance.sunRiseTime + dayFraction;

            float speed = CalculateDayNightSpeed();
            instance._dayNightSpeed = speed;
            return speed;
        }

        [HarmonyPatch(nameof(DayNightCycle.Awake)), HarmonyPostfix]
        static void DayNightCycle_Awake(DayNightCycle __instance) => SetDayNightDuration(__instance);

        [HarmonyPatch(nameof(DayNightCycle.Resume)), HarmonyPostfix]
        static void DayNightCycle_Resume(DayNightCycle __instance) => SetDayNightDuration(__instance);

        [HarmonyPatch(nameof(DayNightCycle.OnConsoleCommand_day)), HarmonyPostfix]
        static void DayNightCycle_OnConsoleCommand_day(DayNightCycle __instance) => SetDayNightDuration(__instance);

        [HarmonyPatch(nameof(DayNightCycle.OnConsoleCommand_night)), HarmonyPostfix]
        static void DayNightCycle_OnConsoleCommand_night(DayNightCycle __instance) => SetDayNightDuration(__instance);

        [HarmonyPatch(nameof(DayNightCycle.OnConsoleCommand_daynight)), HarmonyPostfix]
        static void DayNightCycle_OnConsoleCommand_daynight(DayNightCycle __instance) => SetDayNightDuration(__instance);

        [HarmonyPatch(typeof(DayNightCycle), nameof(DayNightCycle.StopSkipTimeMode)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> DayNightCycle_StopSkipTimeMode(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchForward(false,
                    new CodeMatch(i => i.opcode == OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldc_R4, 1f),
                    new CodeMatch(OpCodes.Stfld)
                )
                .Advance(1)
                .RemoveInstruction()
                .Insert(
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DayNightDuration), nameof(CalculateDayNightSpeed)))
                );

            return matcher.InstructionEnumeration();
        }

        [HarmonyPatch(typeof(DayNightCycle), nameof(DayNightCycle.Resume)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> DayNightCycle_Resume_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchForward(false,
                    new CodeMatch(i => i.opcode == OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldc_R4, 1f),
                    new CodeMatch(OpCodes.Stfld)
                )
                .Advance(1)
                .RemoveInstruction()
                .Insert(
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DayNightDuration), nameof(CalculateDayNightSpeed)))
                );

            return matcher.InstructionEnumeration();
        }

        [HarmonyPatch(typeof(DayNightCycle), nameof(DayNightCycle.Update)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> DayNightCycle_Update(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchForward(false,
                    new CodeMatch(i => i.opcode == OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldc_R4, 1f),
                    new CodeMatch(OpCodes.Stfld)
                )
                .Advance(1)
                .RemoveInstruction()
                .Insert(
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DayNightDuration), nameof(CalculateDayNightSpeed)))
                );

            return matcher.InstructionEnumeration();
        }
    }
}