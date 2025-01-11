using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class KeepDrillParticlesOnLoad
    {
        // Particle system is only skipped if null, alive, and if its emission is enabled
        private static bool CheckParticleSystem(bool existingCondition, ExosuitDrillArm instance)
        {
            var fxPS = instance.fxControl.emitters[0].fxPS;
            if (fxPS == null)
            {
                return true; // Skip Play if fxPS is null
            }

            return fxPS.IsAlive() && fxPS.emission.enabled;
        }

        [HarmonyPatch(typeof(ExosuitDrillArm), nameof(ExosuitDrillArm.OnHit)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ExosuitDrillArm_OnHit(IEnumerable<CodeInstruction> instructions)
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

            // Move back to before Brtrue and insert check
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
    }
}