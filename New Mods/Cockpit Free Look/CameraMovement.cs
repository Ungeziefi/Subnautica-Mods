using HarmonyLib;
using UnityEngine;
using UWE;

namespace Ungeziefi.Cockpit_Free_Look;

[HarmonyPatch]
public partial class CockpitFreeLook
{
    [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.Update))]
    [HarmonyPostfix]
    public static void Vehicle_Update(Vehicle __instance)
    {
        var isExosuit = __instance is Exosuit;
        var isValidVehicle = (isExosuit && Main.Config.PRAWNEnableFeature) ||
                             (!isExosuit && Main.Config.SeamothEnableFeature);
        if (!isValidVehicle) return;

        // Check for vehicle
        if (__instance != Player.main.currentMountedVehicle) return;

        var cameraTransform = MainCamera.camera.transform;

        // Return to centre
        if (isReturning)
        {
            returnTime += Time.deltaTime;
            var t = returnTime / Main.Config.FreeLookReturnDuration;
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

                if (isExosuit) EnableExosuitArms(__instance as Exosuit);
            }

            return;
        }

        var isPausedOrLoading = WaitScreen.IsWaiting
                                || Cursor.visible
                                || FreezeTime.HasFreezers()
                                || Player.main.GetPDA().isOpen;

        if (!isLooking || isPausedOrLoading) return;

        // Calculate look rotation
        var lookDelta = GameInput.GetLookDelta() * Main.Config.FreeLookSensitivity;

        // Apply vehicle-specific rotations
        if (isExosuit)
        {
            // PRAWN - horizontal only
            var newY = currentRotation.y + lookDelta.x;
            currentRotation.y = Mathf.Clamp(newY, -Main.Config.PRAWNAngleLimit, Main.Config.PRAWNAngleLimit);

            var yawRotation = Quaternion.Euler(0f, currentRotation.y, 0f);
            cameraTransform.localRotation = originalRotation * yawRotation;
        }
        else
        {
            // Seamoth - horizontal and vertical with tilt
            var newX = currentRotation.x - lookDelta.y;
            var newY = currentRotation.y + lookDelta.x;

            currentRotation.x = Mathf.Clamp(newX, -Main.Config.SeamothVerticalLimit, Main.Config.SeamothVerticalLimit);
            currentRotation.y =
                Mathf.Clamp(newY, -Main.Config.SeamothHorizontalLimit, Main.Config.SeamothHorizontalLimit);

            // Calculate all rotations
            var yawRotation = Quaternion.Euler(0f, currentRotation.y, 0f);
            var pitchRotation = Quaternion.Euler(currentRotation.x, 0f, 0f);

            // Add tilt
            var tiltAmount = Mathf.Sin(currentRotation.y * Mathf.Deg2Rad) *
                             (Main.Config.CameraTiltAngle * Mathf.Deg2Rad);
            var tiltRotation = Quaternion.Euler(0f, 0f, -tiltAmount * Mathf.Rad2Deg);

            cameraTransform.localRotation = originalRotation * yawRotation * pitchRotation * tiltRotation;
        }
    }
}