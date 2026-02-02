using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class OxygenPipesAcrossSubBiomes
    {
        /// <summary>
        /// This ensures that the oxygen pipe placement check hits only colliders that the player would also collide with,
        /// ignoring anything that is just a trigger.
        /// </summary>
        [HarmonyPatch(typeof(OxygenPipe), nameof(OxygenPipe.IsInSight)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (!Main.Config.OxygenPipesAcrossSubBiomes) return instructions;
            
            
            CodeMatcher matcher = new CodeMatcher(instructions);

            //Find where it loads 0 onto the stack, this is converted to an enum later considered QueryTriggerInteraction.UseGlobal
            matcher.MatchForward(true, new CodeMatch(ci => ci.opcode == OpCodes.Ldc_I4_0));

            if (!matcher.IsValid)
            {
                Main.Logger.LogWarning($"Skipping {nameof(OxygenPipesAcrossSubBiomes)} Transpiler! Likely another mod is modifying OxygenPipe.IsInSight");
                return matcher.Instructions();
            }
			
            //Set to 1 so the enum resolves to QueryTriggerInteraction.Ignore.
            matcher.Set(OpCodes.Ldc_I4_1, null);
		
            return matcher.InstructionEnumeration();
        }
    }
}
