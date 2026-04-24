using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Ungeziefi.Custom_Sunbeam_Countdown;

[HarmonyPatch]
public class CountdownDuration
{
    [HarmonyPatch(typeof(StoryGoalCustomEventHandler), nameof(StoryGoalCustomEventHandler.endTime), MethodType.Getter)]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> StoryGoalCustomEventHandler_endTime(
        IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);

        matcher.MatchForward(false, new CodeMatch(OpCodes.Ldc_R4))
            .RemoveInstruction()
            .Insert(
                Transpilers.EmitDelegate(() => Main.Config.CountdownDuration * 60)
            );

        return matcher.InstructionEnumeration();
    }
}