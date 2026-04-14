using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class NoScannerIconIfScanned
    {
        public static bool IsScanTargetBlueprintKnown()
        {
            PDAScanner.ScanTarget scanTarget = PDAScanner.scanTarget;
            if (!scanTarget.isValid)
                return false;

            TechType blueprint = PDAScanner.GetEntryUnlockable(scanTarget.techType, out _);
            return KnownTech.Contains(blueprint);
        }

        [HarmonyPatch(typeof(GUIHand), nameof(GUIHand.OnUpdate)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> GUIHand_OnUpdate(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            if (!Main.Config.NoScannerIconIfScanned) return instructions;

            var matcher = new CodeMatcher(instructions, generator);

            matcher.MatchForward(false,
                    new CodeMatch(OpCodes.Ldsfld, AccessTools.Field(typeof(uGUI_ScannerIcon), nameof(uGUI_ScannerIcon.main))),
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(uGUI_ScannerIcon), nameof(uGUI_ScannerIcon.Show)))
                );

            var skipLabel = generator.DefineLabel();
            var skipInstruction = matcher.InstructionAt(2);
            skipInstruction.labels.Add(skipLabel);

            matcher.Insert(
                new CodeInstruction(Transpilers.EmitDelegate(IsScanTargetBlueprintKnown)),
                new CodeInstruction(OpCodes.Brtrue_S, skipLabel)
            );

            return matcher.InstructionEnumeration();
        }
    }
}