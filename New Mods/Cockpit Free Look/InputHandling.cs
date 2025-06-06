using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Cockpit_Free_Look
{
    [HarmonyPatch]
    public partial class CockpitFreeLook
    {
        [HarmonyPatch(typeof(Player), nameof(Player.UpdateRotation)), HarmonyPostfix]
        public static void Player_UpdateRotation()
        {
            if (!Main.Config.SeamothEnableFeature && !Main.Config.PRAWNEnableFeature) return;

            Player player = Player.main;
            if (player == null || player.mode != Player.Mode.LockedPiloting) return;

            if (Cursor.visible) return;

            // Get current vehicle
            Vehicle vehicle = player.currentMountedVehicle;
            if (vehicle == null) return;

            // Check vehicle and config
            bool isExosuit = vehicle is Exosuit;
            bool isValidVehicle = (isExosuit && Main.Config.PRAWNEnableFeature) || (!isExosuit && Main.Config.SeamothEnableFeature);
            if (!isValidVehicle) return;

            // Check for key press
            bool isAnyKeyPressed = GameInput.GetKey(Main.Config.FreeLookKey) ||
                                 GameInput.GetKey(Main.Config.SecondaryFreeLookKey);

            if (Main.Config.HoldKeyMode)
            {
                // Hold mode
                if (isAnyKeyPressed && !isLooking)
                {
                    // Start free look when key is held
                    isLooking = true;
                    isReturning = false;
                    originalRotation = mainCamera.transform.localRotation;
                    currentRotation = Vector2.zero;

                    if (isExosuit)
                    {
                        DisableExosuitArms(vehicle as Exosuit);
                    }
                }
                else if (!isAnyKeyPressed && isLooking)
                {
                    // End free look when key is released
                    isLooking = false;
                    isReturning = true;
                    returnTime = 0f;
                }
            }
            else
            {
                // Toggle mode
                if (isAnyKeyPressed && !wasKeyPressed)
                {
                    if (!isLooking)
                    {
                        // Start
                        isLooking = true;
                        isReturning = false;
                        originalRotation = mainCamera.transform.localRotation;
                        currentRotation = Vector2.zero;

                        if (isExosuit)
                        {
                            DisableExosuitArms(vehicle as Exosuit);
                        }
                    }
                    else
                    {
                        // End
                        isLooking = false;
                        isReturning = true;
                        returnTime = 0f;
                    }
                }
            }
            wasKeyPressed = isAnyKeyPressed;
        }
    }
}