using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoUsedTerminalPrompt
    {
        private static bool IsTerminalUsed(StoryHandTarget instance)
        {
            var Terminal = instance.GetComponent<PrecursorComputerTerminal>();
            bool isUsed = Terminal != null && Terminal.used;
            // Main.Logger.LogInfo($"IsTerminalUsed: {isUsed}");
            return isUsed;
        }

        [HarmonyPatch(typeof(StoryHandTarget), nameof(StoryHandTarget.OnHandHover)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> StoryHandTarget_OnHandHover(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> instrList = instructions.ToList();

            Label continueLabel = generator.DefineLabel();

            // Continue if not used
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return Transpilers.EmitDelegate(IsTerminalUsed);
            yield return new CodeInstruction(OpCodes.Brfalse_S, continueLabel);
            yield return new CodeInstruction(OpCodes.Ret);

            // Add label to first instruction
            instrList[0].labels.Add(continueLabel);

            foreach (var instruction in instrList)
            {
                yield return instruction;
            }
        }
    }
}