using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Camera_Zoom
{
    [HarmonyPatch]
    public class CyclopsCameras
    {
        private static Camera Camera => SNCameraRoot.main.mainCamera;
        private static bool isCameraActive;
        private static float previousFOV;
        private static readonly float minFOV = Main.Config.CCMinimumFOV;
        private static readonly float maxFOV = Main.Config.CCMaximumFOV;
        private static readonly float zoomSpeed = Main.Config.CCZoomSpeed;
        private static int currentZoomStep = 0;
        private static Coroutine blackFadeCoroutine = null;
        private const string CAMERA_TYPE = "CyclopsCameras";

        private static void ResetAndDisable(bool disable)
        {
            if (Camera == null || SNCameraRoot.main == null)
            {
                isCameraActive = false;
                return;
            }

            isCameraActive = !disable;

            if (disable)
            {
                // Restore FOV
                ZoomUtils.DeactivateCamera(CAMERA_TYPE, previousFOV, ref currentZoomStep, ref blackFadeCoroutine);
            }
            else
            {
                // Switch camera without changing FOV
                ZoomUtils.SwitchCamera(CAMERA_TYPE);
            }
        }

        // Save FOV on enter
        [HarmonyPatch(typeof(CyclopsExternalCamsButton), nameof(CyclopsExternalCamsButton.CameraButtonActivated)), HarmonyPrefix]
        public static void CyclopsExternalCamsButton_CameraButtonActivated()
        {
            ZoomUtils.InitializeCameraMode(
                CAMERA_TYPE,
                ref previousFOV,
                ref currentZoomStep,
                Main.Config.CCSteppedZoom && Main.Config.CCUseBlinkEffect
            );
        }

        // Set active state and reset on exit
        [HarmonyPatch(typeof(CyclopsExternalCams), nameof(CyclopsExternalCams.SetActive)), HarmonyPostfix]
        public static void CyclopsExternalCams_SetActive(CyclopsExternalCams __instance)
        {
            isCameraActive = __instance.active;
            if (!isCameraActive) ResetAndDisable(true);
        }

        // Reset on camera switch
        [HarmonyPatch(typeof(uGUI_CameraCyclops), nameof(uGUI_CameraCyclops.SetCamera)), HarmonyPostfix]
        public static void uGUI_CameraCyclops_SetCamera() => ResetAndDisable(false);

        // Zoom in/out
        [HarmonyPatch(typeof(uGUI_CameraCyclops), nameof(uGUI_CameraCyclops.Update)), HarmonyPostfix]
        public static void uGUI_CameraCyclops_Update()
        {
            if (SNCameraRoot.main == null || Camera == null)
            {
                isCameraActive = false;
                return;
            }

            // Zoom processing check
            if (!Main.Config.CCEnableFeature || !isCameraActive || Cursor.visible)
                return;

            // Handle different zoom modes
            if (Main.Config.CCSteppedZoom)
            {
                bool zoomInPressed = GameInput.GetButtonDown(Main.CCZoomInButton);
                bool zoomOutPressed = GameInput.GetButtonDown(Main.CCZoomOutButton);

                ZoomUtils.HandleSteppedZoom(
                    zoomInPressed,
                    zoomOutPressed,
                    ref currentZoomStep,
                    Main.Config.CCZoomSteps,
                    Main.Config.CCUseBlinkEffect,
                    Main.Config.CCBlinkSpeed,
                    minFOV,
                    maxFOV,
                    CAMERA_TYPE,
                    ref blackFadeCoroutine
                );
            }
            else
            {
                ZoomUtils.HandleGradualZoom(
                    Main.CCZoomInButton,
                    Main.CCZoomOutButton,
                    zoomSpeed,
                    minFOV,
                    maxFOV
                );
            }
        }
    }
}