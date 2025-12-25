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
    }
}