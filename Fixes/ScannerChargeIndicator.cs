using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class ScannerChargeIndicator
    {
        [HarmonyPatch(typeof(ScannerTool), nameof(ScannerTool.Update)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ScannerTool_Update(IEnumerable<CodeInstruction> instructions)
        {
            if (!Main.Config.ScannerChargeIndicator) return instructions;

            // Find the 2nd SetTextRaw
            var matcher = new CodeMatcher(instructions);
            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(HandReticle), "SetTextRaw")));
            matcher.MatchForward(false,
                new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(HandReticle), "SetTextRaw")));

            // Remove callvirt, ldsfld, ldc.i4.3, and ldsfld
            matcher.RemoveInstructions(4);

            return matcher.InstructionEnumeration();
        }
    }
}