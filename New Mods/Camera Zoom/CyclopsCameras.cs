using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Camera_Zoom
{
    [HarmonyPatch]
    public class CyclopsCameras
    {
        private static Camera Camera => SNCameraRoot.main.mainCamera;
        private static readonly float minFOV = Main.Config.CCMinimumFOV;
        private static readonly float maxFOV = Main.Config.CCMaximumFOV;
        private static readonly float zoomSpeed = Main.Config.CCZoomSpeed;
        private static bool isCameraActive;
        private static float previousFOV;

        // Stepped zoom
        private static int currentZoomStep = 0;

        // Blink effect
        private static Coroutine blackFadeCoroutine = null;
        private const string OVERLAY_NAME = "CyclopsCameras";

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
                // Restore FOV and reset states
                ZoomUtils.ResetZoomState(previousFOV, ref currentZoomStep, OVERLAY_NAME, ref blackFadeCoroutine);
            }
            else
            {
                ZoomUtils.ApplyFOV(maxFOV);
            }
        }

        // Save FOV on enter
        [HarmonyPatch(typeof(CyclopsExternalCamsButton), nameof(CyclopsExternalCamsButton.CameraButtonActivated)), HarmonyPrefix]
        public static void CyclopsExternalCamsButton_CameraButtonActivated()
        {
            previousFOV = Camera.fieldOfView;
            currentZoomStep = 0;

            // Initialize black overlay if needed
            if (Main.Config.CCSteppedZoom && Main.Config.CCUseBlinkEffect)
            {
                ZoomUtils.GetBlackOverlay(OVERLAY_NAME);
            }
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
            // Check for return to menu
            if (SNCameraRoot.main == null || Camera == null)
            {
                isCameraActive = false; // Reset the active state
                return;
            }

            // Zoom processing check
            if (!Main.Config.CCEnableFeature || !isCameraActive || Cursor.visible)
                return;

            // Handle different zoom modes
            if (Main.Config.CCSteppedZoom)
            {
                bool zoomInPressed = Input.GetKeyDown(Main.Config.CCZoomInKey);
                bool zoomOutPressed = Input.GetKeyDown(Main.Config.CCZoomOutKey);

                ZoomUtils.HandleSteppedZoom(
                    zoomInPressed,
                    zoomOutPressed,
                    ref currentZoomStep,
                    Main.Config.CCZoomSteps,
                    Main.Config.CCUseBlinkEffect,
                    Main.Config.CCBlinkSpeed,
                    minFOV,
                    maxFOV,
                    OVERLAY_NAME,
                    ref blackFadeCoroutine
                );
            }
            else
            {
                ZoomUtils.HandleGradualZoom(
                    Main.Config.CCZoomInKey,
                    Main.Config.CCZoomOutKey,
                    zoomSpeed,
                    minFOV,
                    maxFOV
                );
            }
        }
    }
}