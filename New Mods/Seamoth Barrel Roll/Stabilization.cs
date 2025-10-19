using HarmonyLib;

namespace Ungeziefi.Seamoth_Barrel_Roll
{
    [HarmonyPatch]
    public partial class SeamothBarrelRoll
    {
        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.StabilizeRoll)), HarmonyPrefix]
        public static bool Vehicle_StabilizeRoll(Vehicle __instance)
        {
            if (!Main.Config.EnableFeature ||
                Main.Config.StabilizationMode == StabilizationMode.Normal)
                return true;  // Use normal stabilization

            if (Main.Config.StabilizationMode == StabilizationMode.Disabled)
                return false;  // Disable stabilization

            if (Main.Config.StabilizationRequiresPower && !HasPower(__instance))
                return false;  // No stabilization without power

            if (__instance is SeaMoth seamoth)
            {
                // Only stabilize when empty
                if (Main.Config.StabilizationMode == StabilizationMode.OnlyWhenEmpty)
                {
                    return !seamoth.GetPilotingMode(); // Only when not piloting
                }
                // Only stabilize when not actively rolling
                else if (Main.Config.StabilizationMode == StabilizationMode.OnlyWhenIdle)
                {
                    bool isRolling = GameInput.GetButtonDown(Main.RollLeftButton) || GameInput.GetButtonDown(Main.RollRightButton);
                    return !isRolling;
                }
            }

            return true; // Otherwise always stabilize
        }
    }
}