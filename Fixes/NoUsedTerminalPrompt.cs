using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(StoryHandTarget))]
    public class NoUsedTerminalPrompt
    {
        [HarmonyPatch(nameof(StoryHandTarget.OnHandHover)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> OnHandHover(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> instrList = instructions.ToList();

            Label continueLabel = generator.DefineLabel();

            // Add IsTerminalUsed at the start then branch to the continue label if false
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return Transpilers.EmitDelegate(IsTerminalUsed);
            yield return new CodeInstruction(OpCodes.Brfalse_S, continueLabel);
            yield return new CodeInstruction(OpCodes.Ret);

            // Add the label to the first instruction
            instrList[0].labels.Add(continueLabel);

            foreach (var instruction in instrList)
            {
                yield return instruction;
            }
        }

        private static bool IsTerminalUsed(StoryHandTarget instance)
        {
            var Terminal = instance.GetComponent<PrecursorComputerTerminal>();
            bool isUsed = Terminal != null && Terminal.used;
            // Main.Logger.LogInfo($"IsTerminalUsed: {isUsed}");
            return isUsed;
        }
    }
}