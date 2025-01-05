using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(ExosuitDrillArm))]
    public class KeepDrillParticlesOnLoad
    {
        [HarmonyPatch(nameof(ExosuitDrillArm.OnHit)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> OnHit(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            if (!Main.Config.KeepDrillParticlesOnLoad)
            {
                return instructions;
            }

            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Ldloca_S),  // loading emission module local var
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ParticleSystem.EmissionModule), "get_enabled")),
                new CodeMatch(OpCodes.Brtrue)
            );

            // Move back to before the brtrue and insert the new check
            matcher.Insert(
                new CodeInstruction(OpCodes.Ldarg_0),  // Load ExosuitDrillArm instance (this)
                Transpilers.EmitDelegate(CheckParticleSystem)
            );

            //foreach (var item in matcher.InstructionEnumeration())
            //{
            //    Main.Logger.LogInfo($"{item.opcode} {item.operand}");
            //}

            return matcher.InstructionEnumeration();
        }

        // Ensures that the particle system is only skipped if it is null, alive, and its emission is enabled
        private static bool CheckParticleSystem(bool existingCondition, ExosuitDrillArm instance)
        {
            var fxPS = instance.fxControl.emitters[0].fxPS;
            if (fxPS == null)
            {
                return true; // Skip Play if fxPS is null
            }

            return fxPS.IsAlive() && fxPS.emission.enabled;
        }
    }
}