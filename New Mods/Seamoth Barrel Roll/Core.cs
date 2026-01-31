using HarmonyLib;
using System.Collections.Generic;

namespace Ungeziefi.Seamoth_Barrel_Roll
{
    public partial class SeamothBarrelRoll
    {
        private static readonly Dictionary<SeaMoth, RollState> activeRolls = new();

        public class RollState
        {
            public float currentRollForce, targetRollForce;
            public bool isRolling, wasRolling; // State tracking for sound effects
        }

        public static bool HasPower(Vehicle vehicle) =>
            vehicle.GetComponent<EnergyMixin>().charge > 0f;

        // Cleanup on exiting pilot mode
        [HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.OnPilotModeEnd)), HarmonyPostfix]
        public static void SeaMoth_OnPilotModeEnd(SeaMoth __instance)
        {
            if (!Main.Config.EnableFeature || !activeRolls.ContainsKey(__instance))
                return;

            // Reset engine sound
            __instance.engineSound.AccelerateInput(1f);

            // Remove tracking state
            activeRolls.Remove(__instance);
        }
    }
}