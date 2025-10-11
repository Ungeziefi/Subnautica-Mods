using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class PassiveEngineOverheating
    {
        [HarmonyPatch(typeof(SubFire), nameof(SubFire.EngineOverheatSimulation)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SubFire_EngineOverheatSimulation(IEnumerable<CodeInstruction> instructions)
        {
            if (!Main.Config.PassiveEngineOverheating) return instructions;

            var matcher = new CodeMatcher(instructions);
            matcher.MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(SubFire), "subControl")),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(SubControl), "appliedThrottle")),
                new CodeMatch(OpCodes.Brfalse)
            );

            // Replace with NOPs
            matcher.SetOpcodeAndAdvance(OpCodes.Nop);
            matcher.SetOpcodeAndAdvance(OpCodes.Nop);
            matcher.SetOpcodeAndAdvance(OpCodes.Nop);
            matcher.SetOpcodeAndAdvance(OpCodes.Nop);

            foreach (var item in matcher.InstructionEnumeration())
            {
                Main.Logger.LogInfo($"{item.opcode} {item.operand}");
            }

            return matcher.InstructionEnumeration();
        }
    }
}