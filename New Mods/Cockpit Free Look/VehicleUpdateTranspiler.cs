using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Ungeziefi.Cockpit_Free_Look
{
    [HarmonyPatch]
    public partial class CockpitFreeLook
    {
        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.Update)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Vehicle_Update(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            // Line 777
            var matcher = new CodeMatcher(instructions, generator);
            matcher.MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Vehicle), "GetPilotingMode")));

            // Add label to the branch that skips the check
            var endLabel = matcher.Advance(2).Instruction.operand;

            // Add condition
            matcher.Start().Insert(
                new CodeInstruction(OpCodes.Ldarg_0),
                Transpilers.EmitDelegate(IsInFreeLook),
                new CodeInstruction(OpCodes.Brtrue, endLabel) // Skip block if true
            );

            return matcher.InstructionEnumeration();
        }
    }
}