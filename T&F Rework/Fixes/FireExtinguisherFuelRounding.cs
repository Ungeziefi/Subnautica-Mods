using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class FireExtinguisherFuelRounding
    {
        [HarmonyPatch(typeof(FireExtinguisher), nameof(FireExtinguisher.GetFuelValueText)), HarmonyTranspiler]
        static IEnumerable<CodeInstruction> FireExtinguisher_GetFuelValueText(IEnumerable<CodeInstruction> instructions)
        {
            if (!Main.Config.FireExtinguisherFuelRounding) return instructions;

            var matcher = new CodeMatcher(instructions);

            matcher.MatchForward(false,
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Mathf), nameof(Mathf.FloorToInt)))
                )
                .SetOperandAndAdvance(AccessTools.Method(typeof(Mathf), nameof(Mathf.RoundToInt)));

            return matcher.InstructionEnumeration();
        }
    }
}