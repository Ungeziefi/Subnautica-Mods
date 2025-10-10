using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
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
            if (fxPS == null) return true; // Skip Play if fxPS is null

            return fxPS.IsAlive() && fxPS.emission.enabled;
        }

        [HarmonyPatch(typeof(ExosuitDrillArm), nameof(ExosuitDrillArm.OnHit)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ExosuitDrillArm_OnHit(IEnumerable<CodeInstruction> instructions)
        {
            if (!Main.Config.KeepDrillParticlesOnLoad) return instructions;

            // Find the check for particle system
            var matcher = new CodeMatcher(instructions);
            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Ldloca_S),  // loading emission module local var
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ParticleSystem.EmissionModule), "get_enabled")),
                new CodeMatch(OpCodes.Brtrue)
            );

            // Insert check right before the Brtrue
            matcher.Insert(
                new CodeInstruction(OpCodes.Ldarg_0),  // Load ExosuitDrillArm instance (this)
                Transpilers.EmitDelegate(CheckParticleSystem)
            );

            return matcher.InstructionEnumeration();
        }
    }
}