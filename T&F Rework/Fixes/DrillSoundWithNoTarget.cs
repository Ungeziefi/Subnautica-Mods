using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class DrillSoundWithNoTarget
    {
        [HarmonyPatch(typeof(ExosuitDrillArm), nameof(ExosuitDrillArm.StopEffects)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ExosuitDrillArm_StopEffects(IEnumerable<CodeInstruction> instructions)
        {
            if (!Main.Config.DrillSoundWithNoTarget) return instructions;

            // Find this.loop.Stop(STOP_MODE.ALLOWFADEOUT)
            var matcher = new CodeMatcher(instructions);
            matcher.MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(ExosuitDrillArm), "loop")),
                new CodeMatch(OpCodes.Ldc_I4_0),
                new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(FMOD_CustomEmitter), "Stop")));

            // Replace with NOPs, can't remove because of 2 branches to ldarg_0 (41)
            matcher.SetOpcodeAndAdvance(OpCodes.Nop);
            matcher.SetOpcodeAndAdvance(OpCodes.Nop);
            matcher.SetOpcodeAndAdvance(OpCodes.Nop);
            matcher.SetOpcodeAndAdvance(OpCodes.Nop);

            return matcher.InstructionEnumeration();
        }

        // Sound needs to be stopped manually
        // When deactivated
        [HarmonyPatch(typeof(ExosuitDrillArm), "IExosuitArm.OnUseUp"), HarmonyPostfix]
        public static void IExosuitArm_OnUseUp(ExosuitDrillArm __instance)
        {
            if (Main.Config.DrillSoundWithNoTarget) __instance.loop.Stop();
        }

        // When reset
        [HarmonyPatch(typeof(ExosuitDrillArm), "IExosuitArm.ResetArm"), HarmonyPostfix]
        public static void IExosuitArm_ResetArm(ExosuitDrillArm __instance)
        {
            if (Main.Config.DrillSoundWithNoTarget) __instance.loop.Stop();
        }
    }
}
