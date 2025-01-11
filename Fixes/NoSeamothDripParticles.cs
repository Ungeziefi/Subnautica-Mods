using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoSeamothDripParticles
    {
        [HarmonyPatch(typeof(VFXSeamothDamages), nameof(VFXSeamothDamages.UpdateParticles)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> VFXSeamothDamages_UpdateParticles(IEnumerable<CodeInstruction> instructions)
        {
            if (!Main.Config.NoSeamothDripParticles)
            {
                return instructions;
            }

            var matcher = new CodeMatcher(instructions);


            matcher.MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(VFXSeamothDamages), "dripsParticles")),
                new CodeMatch(OpCodes.Ldloc_0),
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(VFXSeamothDamages), "UpdatePArticlesEmission")));

            matcher.RemoveInstructions(5);

            //foreach (var item in matcher.InstructionEnumeration())
            //{
            //    Main.Logger.LogInfo($"{item.opcode} {item.operand}");
            //}

            return matcher.InstructionEnumeration();
        }
    }
}