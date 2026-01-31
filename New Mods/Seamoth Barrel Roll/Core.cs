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

        // Cleanup on exit
        [HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.OnPilotModeEnd)), HarmonyPostfix]
        public static void SeaMoth_OnPilotModeEnd(SeaMoth __instance)
        {
            if (activeRolls.ContainsKey(__instance))
            {
                __instance.engineSound.AccelerateInput(1f); // Reset engine sound
                activeRolls.Remove(__instance); // Remove tracking state
            }
        }
    }
}