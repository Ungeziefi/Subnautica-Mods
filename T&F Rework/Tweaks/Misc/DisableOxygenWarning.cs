using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Ungeziefi.Tweaks.Misc;

[HarmonyPatch]
public class DisableOxygenWarning
{
    public static bool ShouldDisableOxygenWarning()
    {
        if (Main.Config.DisableOxygenWarning) return true;

        return false;
    }

    [HarmonyPatch(typeof(LowOxygenAlert), nameof(LowOxygenAlert.Update))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> LowOxygenAlert_Update(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);

        if (!Main.Config.DisableOxygenWarning) return instructions;

        matcher.MatchForward(true,
            new CodeMatch(OpCodes.Ldc_I4),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(GameModeUtils),
                nameof(GameModeUtils.IsOptionActive),
                new[] { typeof(GameModeOption) })),
            new CodeMatch(OpCodes.Brtrue));

        var skipLabel = (Label)matcher.Operand;

        matcher.Advance(1);

        matcher.InsertAndAdvance(Transpilers.EmitDelegate(ShouldDisableOxygenWarning));
        matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Brtrue_S, skipLabel));

        return matcher.InstructionEnumeration();
    }
}