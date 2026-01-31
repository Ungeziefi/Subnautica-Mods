using HarmonyLib;

namespace Ungeziefi.Seamoth_Barrel_Roll
{
    [HarmonyPatch]
    public partial class SeamothBarrelRoll
    {
        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.StabilizeRoll)), HarmonyPrefix]
        public static bool Vehicle_StabilizeRoll(Vehicle __instance)
        {
            if (__instance is SeaMoth seamoth)
            {
                if (!Main.Config.EnableFeature || Main.Config.StabilizationMode == StabilizationMode.Normal)
                    return true;  // Use normal stabilization

                if (Main.Config.StabilizationMode == StabilizationMode.Disabled)
                    return false;  // Disable stabilization

                if (Main.Config.StabilizationRequiresPower && !HasPower(__instance))
                    return false;  // No stabilization without power

                // Only when not piloting
                if (Main.Config.StabilizationMode == StabilizationMode.OnlyWhenUnpiloted)
                {
                    return !seamoth.GetPilotingMode();
                }

                // Only stabilize when not rolling
                if (Main.Config.StabilizationMode == StabilizationMode.OnlyWhenNotRolling)
                {
                    bool isRolling = GameInput.GetButtonHeld(Main.RollLeftButton) || GameInput.GetButtonHeld(Main.RollRightButton);
                    return !isRolling;
                }

                // Only stabilize when not moving and not rolling
                if (Main.Config.StabilizationMode == StabilizationMode.OnlyWhenIdle)
                {
                    bool isRolling = GameInput.GetButtonHeld(Main.RollLeftButton) || GameInput.GetButtonHeld(Main.RollRightButton);
                    bool isMoving = GameInput.GetButtonHeld(GameInput.Button.MoveForward) ||
                                    GameInput.GetButtonHeld(GameInput.Button.MoveBackward) ||
                                    GameInput.GetButtonHeld(GameInput.Button.MoveLeft) ||
                                    GameInput.GetButtonHeld(GameInput.Button.MoveRight) ||
                                    GameInput.GetButtonHeld(GameInput.Button.MoveUp) ||
                                    GameInput.GetButtonHeld(GameInput.Button.MoveDown);
                    return !isRolling && !isMoving;
                }
            }

            return true;
        }
    }
}