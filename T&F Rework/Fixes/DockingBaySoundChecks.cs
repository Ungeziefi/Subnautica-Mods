using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class DockingBaySoundChecks
    {
        #region Open SFX
        [HarmonyPatch(typeof(VehicleDockingBay), nameof(VehicleDockingBay.LaunchbayAreaEnter)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> VehicleDockingBay_LaunchbayAreaEnter(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            if (!Main.Config.DockingBaySoundChecks) return instructions;

            // Find Play call
            var matcher = new CodeMatcher(instructions, generator);
            matcher.MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0), // Load "this"
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(VehicleDockingBay), "bayDoorsOpenSFX")),
                new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(FMOD_CustomEmitter), nameof(FMOD_CustomEmitter.Play))));

            // Skip label
            var skipLabel = generator.DefineLabel();
            var skipInstruction = matcher.InstructionAt(3);
            skipInstruction.labels.Add(skipLabel);

            // Insert check
            matcher.Insert(
                new CodeInstruction(OpCodes.Ldarg_0),                 // Load "this"
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(VehicleDockingBay), nameof(VehicleDockingBay.GetDockedVehicle))),
                new CodeInstruction(OpCodes.Ldnull),                  // Load null for comparison
                new CodeInstruction(OpCodes.Ceq),                     // Compare if GetDockedVehicle() == null
                new CodeInstruction(OpCodes.Brfalse_S, skipLabel)     // Skip if occupied
            );

            return matcher.InstructionEnumeration();
        }
        #endregion

        #region Close SFX
        [HarmonyPatch(typeof(VehicleDockingBay), nameof(VehicleDockingBay.LaunchbayAreaExit)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> VehicleDockingBay_LaunchbayAreaExit(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            if (!Main.Config.DockingBaySoundChecks) return instructions;

            // Find Play call
            var matcher = new CodeMatcher(instructions, generator);
            matcher.MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0), // Load "this"
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(VehicleDockingBay), "bayDoorsCloseSFX")),
                new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(FMOD_CustomEmitter), nameof(FMOD_CustomEmitter.Play))));

            // Skip label
            var skipLabel = generator.DefineLabel();
            var skipInstruction = matcher.InstructionAt(3);
            skipInstruction.labels.Add(skipLabel);

            // Insert checks
            matcher.Insert(
                new CodeInstruction(OpCodes.Ldarg_0),                 // Load "this"
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(VehicleDockingBay), nameof(VehicleDockingBay.GetDockedVehicle))),
                new CodeInstruction(OpCodes.Ldnull),                  // Load null for comparison
                new CodeInstruction(OpCodes.Ceq),                     // Compare if GetDockedVehicle() == null
                new CodeInstruction(OpCodes.Brfalse_S, skipLabel),    // Skip if occupied

                // Probably redundant
                new CodeInstruction(OpCodes.Ldarg_0),                 // Load "this"
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(VehicleDockingBay), nameof(VehicleDockingBay.IsPowered))),
                new CodeInstruction(OpCodes.Brfalse_S, skipLabel)     // Skip if not powered
            );

            return matcher.InstructionEnumeration();
            #endregion
        }
    }
}