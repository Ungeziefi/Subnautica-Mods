using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Cockpit_Free_Look
{
    [HarmonyPatch]
    public partial class CockpitFreeLook
    {
        [HarmonyPatch(typeof(Player), nameof(Player.Update)), HarmonyPostfix]
        public static void Player_Update()
        {
            if (!ShouldProcessFreeLook(out Vehicle vehicle, out bool isExosuit)) return;

            if (Main.Config.HoldKeyMode)
            {
                HandleHoldMode(vehicle, isExosuit);
            }
            else
            {
                HandleToggleMode(vehicle, isExosuit);
            }
        }

        private static bool ShouldProcessFreeLook(out Vehicle vehicle, out bool isExosuit)
        {
            vehicle = null;
            isExosuit = false;

            if (!Main.Config.SeamothEnableFeature && !Main.Config.PRAWNEnableFeature)
                return false;

            Player player = Player.main;
            if (player == null || player.mode != Player.Mode.LockedPiloting || Cursor.visible)
                return false;

            vehicle = player.currentMountedVehicle;
            if (vehicle == null)
                return false;

            isExosuit = vehicle is Exosuit;
            bool isValidVehicle = (isExosuit && Main.Config.PRAWNEnableFeature) ||
                                  (!isExosuit && Main.Config.SeamothEnableFeature);

            return isValidVehicle;
        }

        private static void HandleHoldMode(Vehicle vehicle, bool isExosuit)
        {
            bool isButtonHeld = GameInput.GetButtonHeld(Main.FreeLookButton);

            if (isButtonHeld && !isLooking)
            {
                StartFreeLook(vehicle, isExosuit);
            }
            else if (!isButtonHeld && isLooking)
            {
                EndFreeLook();
            }
        }

        private static void HandleToggleMode(Vehicle vehicle, bool isExosuit)
        {
            bool isButtonPressed = GameInput.GetButtonDown(Main.FreeLookButton);

            if (isButtonPressed)
            {
                if (!isLooking)
                {
                    StartFreeLook(vehicle, isExosuit);
                }
                else
                {
                    EndFreeLook();
                }
            }
        }

        private static void StartFreeLook(Vehicle vehicle, bool isExosuit)
        {
            isLooking = true;
            isReturning = false;
            originalRotation = mainCamera.transform.localRotation;
            currentRotation = Vector2.zero;

            if (isExosuit)
            {
                DisableExosuitArms(vehicle as Exosuit);
            }
        }

        private static void EndFreeLook()
        {
            isLooking = false;
            isReturning = true;
            returnTime = 0f;
        }
    }
}