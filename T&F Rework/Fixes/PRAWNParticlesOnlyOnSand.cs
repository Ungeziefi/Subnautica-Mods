using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class PRAWNParticlesOnlyOnSand
    {
        #region ApplyJumpForce
        private static bool ShouldPlayJumpFX_ApplyJumpForce(bool onGround, Exosuit exosuit)
        {
            if (!onGround || !exosuit.IsUnderwater()) return false; // Keep original result

            if (Physics.Raycast(new Ray(exosuit.transform.position, Vector3.down), out RaycastHit hitInfo, 10f))
            {
                var terrain = hitInfo.collider.GetComponent<TerrainChunkPieceCollider>();
                return terrain != null; // True if terrain was hit
            }

            return false; // No terrain found
        }

        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.ApplyJumpForce)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Exosuit_ApplyJumpForce(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var matcher = new CodeMatcher(instructions, generator);

            if (!Main.Config.PRAWNParticlesOnlyOnSand) return instructions;

            // Find onGround check
            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Vehicle), "onGround")));

            // Insert check
            matcher.Advance(1).Insert(
                new CodeInstruction(OpCodes.Ldarg_0),  // Load Exosuit
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PRAWNParticlesOnlyOnSand), nameof(ShouldPlayJumpFX_ApplyJumpForce)))
            );

            return matcher.InstructionEnumeration();
        }
        #endregion

        #region OnLand
        private static bool ShouldPlayJumpFX_OnLand(Exosuit exosuit)
        {
            if (!exosuit.IsUnderwater()) return false;

            if (Physics.Raycast(new Ray(exosuit.transform.position, Vector3.down), out RaycastHit hitInfo, 10f))
            {
                var terrain = hitInfo.collider.GetComponent<TerrainChunkPieceCollider>();
                return terrain != null; // True if terrain was hit
            }

            return false; // No terrain found
        }

        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.OnLand)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Exosuit_OnLand(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var matcher = new CodeMatcher(instructions, generator);

            if (!Main.Config.PRAWNParticlesOnlyOnSand) return instructions;

            // Create skip label at the return instruction
            var skipLabel = generator.DefineLabel();
            matcher.End().Labels.Add(skipLabel);

            // Insert check at the start
            matcher.Start().Insert(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PRAWNParticlesOnlyOnSand), nameof(ShouldPlayJumpFX_OnLand))),
                new CodeInstruction(OpCodes.Brfalse, skipLabel)
            );

            return matcher.InstructionEnumeration();
        }
        #endregion
    }
}