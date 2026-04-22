using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Ungeziefi.Tweaks.Misc;

[HarmonyPatch]
public class DisableSwimWarning
{
    public static bool ShouldDisableSwimWarning()
    {
        if (Main.Config.DisableSwimWarning) return true;

        return false;
    }

    [HarmonyPatch(typeof(HintSwimToSurface), nameof(HintSwimToSurface.ShouldShowWarning))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> HintSwimToSurface_ShouldShowWarning(
        IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);

        if (!Main.Config.DisableSwimWarning) return instructions;

        matcher.MatchForward(true,
            new CodeMatch(OpCodes.Ldc_I4),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(GameModeUtils),
                nameof(GameModeUtils.IsOptionActive),
                new[] { typeof(GameModeOption) })),
            new CodeMatch(OpCodes.Brfalse));

        var skipLabel = (Label)matcher.Operand;

        matcher.Advance(1);

        matcher.InsertAndAdvance(Transpilers.EmitDelegate(ShouldDisableSwimWarning));
        matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Brtrue_S, skipLabel));

        return matcher.InstructionEnumeration();
    }
}