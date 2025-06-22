using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class NoPDADelay
    {
        [HarmonyPatch(typeof(PDA), nameof(PDA.Open)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PDA_Open(IEnumerable<CodeInstruction> instructions)
        {
            if (!Main.Config.NoPDADelay) return instructions;

            var matcher = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(i => i.opcode == OpCodes.Callvirt),
                    new CodeMatch(i => i.opcode == OpCodes.Ldarg_0),
                    new CodeMatch(i => i.opcode == OpCodes.Ldfld),
                    new CodeMatch(OpCodes.Ldc_R4, 0.5f)
                )
                .SetOperandAndAdvance(0f);

            return matcher.InstructionEnumeration();
        }

        [HarmonyPatch(typeof(PDA), nameof(PDA.Close)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PDA_Close(IEnumerable<CodeInstruction> instructions)
        {
            if (!Main.Config.NoPDADelay) return instructions;

            var matcher = new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(i => i.opcode == OpCodes.Ldarg_0),
                    new CodeMatch(i => i.opcode == OpCodes.Ldfld),
                    new CodeMatch(OpCodes.Ldc_R4, 0.3f)
                )
                .SetOperandAndAdvance(0f);

            return matcher.InstructionEnumeration();
        }
    }
}