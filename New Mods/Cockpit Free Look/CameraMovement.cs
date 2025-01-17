using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Cockpit_Free_Look
{
    [HarmonyPatch]
    public partial class CockpitFreeLook
    {
        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.Update)), HarmonyPostfix]
        public static void Vehicle_Update(Vehicle __instance)
        {
            bool isExosuit = __instance is Exosuit;
            bool isValidVehicle = (isExosuit && Main.Config.PRAWNEnableFeature) ||
                                (!isExosuit && Main.Config.SeamothEnableFeature);
            if (!isValidVehicle) return;

            // Check for vehicle
            if (__instance != Player.main?.currentMountedVehicle) return;

            // Get camera
            if (mainCamera == null)
                mainCamera = MainCamera.camera;
            if (mainCamera == null) return;

            Transform cameraTransform = mainCamera.transform;

            // Return to centre
            if (isReturning)
            {
                returnTime += Time.deltaTime;
                float t = returnTime / Main.Config.FreeLookReturnDuration;
                t = Mathf.Sin(t * Mathf.PI * 0.5f); // Smooth easing

                cameraTransform.localRotation = Quaternion.Slerp(
                    cameraTransform.localRotation,
                    originalRotation,
                    t
                );

                // Return complete
                if (returnTime >= Main.Config.FreeLookReturnDuration)
                {
                    isReturning = false;
                    cameraTransform.localRotation = originalRotation;
                    currentRotation = Vector2.zero;

                    if (isExosuit)
                    {
                        EnableExosuitArms(__instance as Exosuit);
                    }
                }
                return;
            }

            if (!isLooking || Cursor.visible) return;

            // Calculate look rotation
            Vector2 lookDelta = GameInput.GetLookDelta() * Main.Config.FreeLookSensitivity;

            // Apply vehicle-specific rotations
            if (isExosuit)
            {
                // PRAWN - horizontal only
                float newY = currentRotation.y + lookDelta.x;
                currentRotation.y = Mathf.Clamp(newY, -Main.Config.PRAWNAngleLimit, Main.Config.PRAWNAngleLimit);

                Quaternion yawRotation = Quaternion.Euler(0f, currentRotation.y, 0f);
                cameraTransform.localRotation = originalRotation * yawRotation;
            }
            else
            {
                // Seamoth - horizontal and vertical with tilt
                float newX = currentRotation.x - lookDelta.y;
                float newY = currentRotation.y + lookDelta.x;

                currentRotation.x = Mathf.Clamp(newX, -Main.Config.SeamothVerticalLimit, Main.Config.SeamothVerticalLimit);
                currentRotation.y = Mathf.Clamp(newY, -Main.Config.SeamothHorizontalLimit, Main.Config.SeamothHorizontalLimit);

                // Calculate all rotations
                Quaternion yawRotation = Quaternion.Euler(0f, currentRotation.y, 0f);
                Quaternion pitchRotation = Quaternion.Euler(currentRotation.x, 0f, 0f);

                // Add tilt
                float tiltAmount = Mathf.Sin(currentRotation.y * Mathf.Deg2Rad) * (Main.Config.CameraTiltAngle * Mathf.Deg2Rad);
                Quaternion tiltRotation = Quaternion.Euler(0f, 0f, -tiltAmount * Mathf.Rad2Deg);

                cameraTransform.localRotation = originalRotation * yawRotation * pitchRotation * tiltRotation;
            }
        }
    }
}