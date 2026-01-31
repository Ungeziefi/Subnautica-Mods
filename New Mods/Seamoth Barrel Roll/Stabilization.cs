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
                    bool isRolling = GameInput.GetButtonDown(Main.RollLeftButton) || GameInput.GetButtonDown(Main.RollRightButton);
                    return !isRolling;
                }

                // Only stabilize when not moving and not rolling
                if (Main.Config.StabilizationMode == StabilizationMode.OnlyWhenIdle)
                {
                    bool isRolling = GameInput.GetButtonDown(Main.RollLeftButton) || GameInput.GetButtonDown(Main.RollRightButton);
                    bool isMoving = GameInput.GetButtonDown(GameInput.Button.MoveForward) ||
                                    GameInput.GetButtonDown(GameInput.Button.MoveBackward) ||
                                    GameInput.GetButtonDown(GameInput.Button.MoveLeft) ||
                                    GameInput.GetButtonDown(GameInput.Button.MoveRight) ||
                                    GameInput.GetButtonDown(GameInput.Button.MoveUp) ||
                                    GameInput.GetButtonDown(GameInput.Button.MoveDown);
                    return !isRolling && !isMoving;
                }
            }

            return true;
        }
    }
}