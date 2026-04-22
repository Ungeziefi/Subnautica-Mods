using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Ungeziefi.Fixes.Equipment;

[HarmonyPatch]
public class AirBladderRequiresPower
{
    public static bool ShouldRefillOxygen()
    {
        var player = Player.main;

        if (player.IsInBase())
        {
            var sub = player.currentSub;
            if (sub != null && sub.powerRelay.GetPowerStatus() == PowerSystem.Status.Offline)
                return false;
        }

        return true;
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

        matcher.InsertAndAdvance(Transpilers.EmitDelegate(ShouldRefillOxygen));
        matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Brfalse_S, skipLabel));

        return matcher.InstructionEnumeration();
    }
}