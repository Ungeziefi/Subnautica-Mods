using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Ungeziefi.Fixes.Equipment;

[HarmonyPatch]
public class AirBladderRequiresPower
{
    private static bool CanBreathe()
    {
        if (Player.main.CanBreathe()) return true;

        return false;
    }

    [HarmonyPatch(typeof(AirBladder), nameof(AirBladder.UpdateInflateState))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> AirBladder_UpdateInflateState(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);

        if (!Main.Config.AirBladderRequiresPower) return instructions;

        matcher.MatchForward(true,
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(AirBladder), "get_isUnderwater")),
            new CodeMatch(OpCodes.Brtrue));

        var skipLabel = (Label)matcher.Operand;

        matcher.Advance(1);

        matcher.InsertAndAdvance(Transpilers.EmitDelegate(CanBreathe));
        matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Brfalse_S, skipLabel));

        return matcher.InstructionEnumeration();
    }
}