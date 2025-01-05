using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(ExosuitDrillArm))]
    public class DrillSoundWithNoTarget
    {
        [HarmonyPatch(nameof(ExosuitDrillArm.StopEffects)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> StopEffects(IEnumerable<CodeInstruction> instructions)
        {
            if (!Main.Config.DrillSoundWithNoTarget)
            {
                return instructions;
            }

            var matcher = new CodeMatcher(instructions);

            // Find this.loop.Stop(STOP_MODE.ALLOWFADEOUT)
            matcher.MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ExosuitDrillArm), "loop")),
                new CodeMatch(OpCodes.Ldc_I4_0),
                new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(FMOD_CustomEmitter), "Stop")));

            // Replace those 4 instructions with NOPs, can't remove them because there are 2 branches going to ldarg_0 (41)
            matcher.SetOpcodeAndAdvance(OpCodes.Nop);
            matcher.SetOpcodeAndAdvance(OpCodes.Nop);
            matcher.SetOpcodeAndAdvance(OpCodes.Nop);
            matcher.SetOpcodeAndAdvance(OpCodes.Nop);

            return matcher.InstructionEnumeration();
        }

        // Need to stop the sound manually when the drill is deactivated
        [HarmonyPatch("IExosuitArm.OnUseUp"), HarmonyPostfix]
        public static void OnUseUp(ExosuitDrillArm __instance)
        {
            if (Main.Config.DrillSoundWithNoTarget)
            {
                __instance.loop.Stop();
            }
        }
    }
}