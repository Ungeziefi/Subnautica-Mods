using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(ScannerTool))]
    public class FixScannerToolChargeLevel
    {
        [HarmonyPatch(nameof(ScannerTool.Update)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Update(IEnumerable<CodeInstruction> instructions)
        {
            if (!Main.FixesConfig.ScannerChargeIndicator)
            {
                // Return the original instructions
                return instructions;
            }

            var matcher = new CodeMatcher(instructions);

            // Find the first instance of SetTextRaw and move to the end of the match
            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(HandReticle), "SetTextRaw")));

            // Find the second instance of SetTextRaw and move to the start of the match
            matcher.MatchForward(false,
                new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(HandReticle), "SetTextRaw")));

            // Remove the instructions for the second SetTextRaw call (callvirt, ldsfld, ldc.i4.3, and ldsfld)
            matcher.RemoveInstructions(4);

            return matcher.InstructionEnumeration();
        }
    }
}