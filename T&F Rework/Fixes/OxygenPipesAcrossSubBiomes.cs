using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class OxygenPipesAcrossSubBiomes
    {
        // Makes the oxygen pipes collision check ignore colliders that wouldn't affect the player
        [HarmonyPatch(typeof(OxygenPipe), nameof(OxygenPipe.IsInSight)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (!Main.Config.OxygenPipesAcrossSubBiomes) return instructions;

            var matcher = new CodeMatcher(instructions);

            //Find where it loads 0 onto the stack, this is converted to an enum later considered QueryTriggerInteraction.UseGlobal
            matcher.MatchForward(true, new CodeMatch(ci => ci.opcode == OpCodes.Ldc_I4_0));

            if (!matcher.IsValid)
            {
                Main.Logger.LogWarning($"Skipping the {nameof(OxygenPipesAcrossSubBiomes)} transpiler patch! Another mod might be modifying OxygenPipe.IsInSight.");
                return matcher.Instructions();
            }

            //Set to 1 so the enum resolves to QueryTriggerInteraction.Ignore.
            matcher.Set(OpCodes.Ldc_I4_1, null);

            return matcher.InstructionEnumeration();
        }
    }
}
