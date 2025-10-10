using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class PRAWNSuitClawDamage
    {
        [HarmonyPatch(typeof(ExosuitClawArm), nameof(ExosuitClawArm.OnHit)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ExosuitClawArm_OnHit(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            if (Main.Config.PRAWNSuitClawDamage == 50f) return instructions;

            matcher.MatchForward(false,
                    new CodeMatch(i => i.opcode == OpCodes.Ldloc_S),
                    new CodeMatch(OpCodes.Ldc_R4, 50f),
                    new CodeMatch(i => i.opcode == OpCodes.Ldloc_1),
                    new CodeMatch(i => i.opcode == OpCodes.Ldc_I4_0),
                    new CodeMatch(i => i.opcode == OpCodes.Ldnull)
                )
                .Advance(1)
                .RemoveInstruction()
                .Insert(
                    Transpilers.EmitDelegate(() => Main.Config.PRAWNSuitClawDamage)
                );

            return matcher.InstructionEnumeration();
        }
    }
}