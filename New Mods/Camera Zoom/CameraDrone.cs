using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Camera_Zoom
{
    [HarmonyPatch]
    public class CameraDrone
    {
        private static Camera Camera => SNCameraRoot.main.mainCamera;
        private static bool IsCameraActive => uGUI_CameraDrone.main.activeCamera != null;
        private static float previousFOV;
        private static readonly float minFOV = Main.Config.CDMinimumFOV;
        private static readonly float maxFOV = Main.Config.CDMaximumFOV;
        private static readonly float zoomSpeed = Main.Config.CDZoomSpeed;
        private static int currentZoomStep = 0;
        private static Coroutine blackFadeCoroutine = null;
        private const string CAMERA_TYPE = "CameraDrone";

        private static void ResetAndDisable(bool disable)
        {
            if (Camera == null || SNCameraRoot.main == null)
                return;

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
        [HarmonyPatch(typeof(MapRoomCamera), nameof(MapRoomCamera.ControlCamera)), HarmonyPrefix]
        public static void MapRoomCamera_ControlCamera()
        {
            ZoomUtils.InitializeCameraMode(
                CAMERA_TYPE,
                ref previousFOV,
                ref currentZoomStep,
                Main.Config.CDSteppedZoom && Main.Config.CDUseBlinkEffect
            );
        }

        // Reset on exit
        [HarmonyPatch(typeof(MapRoomCamera), nameof(MapRoomCamera.FreeCamera)), HarmonyPostfix]
        public static void MapRoomCamera_FreeCamera()
        {
            ResetAndDisable(true);
        }

        // Reset on camera switch
        [HarmonyPatch(typeof(MapRoomCamera), nameof(MapRoomCamera.ExitLockedMode)), HarmonyPostfix]
        public static void MapRoomCamera_ExitLockedMode()
        {
            ResetAndDisable(false);
        }

        // Zoom in/out
        [HarmonyPatch(typeof(MapRoomCamera), nameof(MapRoomCamera.HandleInput)), HarmonyPostfix]
        public static void MapRoomCamera_HandleInput()
        {
            // Check for return to menu
            if (SNCameraRoot.main == null || Camera == null)
                return;

            // Zoom processing check
            if (!Main.Config.CDEnableFeature || !IsCameraActive || Cursor.visible)
                return;

            // Handle different zoom modes
            if (Main.Config.CDSteppedZoom)
            {
                bool zoomInPressed = GameInput.GetButtonDown(Main.CDZoomInButton);
                bool zoomOutPressed = GameInput.GetButtonDown(Main.CDZoomOutButton);

                ZoomUtils.HandleSteppedZoom(
                    zoomInPressed,
                    zoomOutPressed,
                    ref currentZoomStep,
                    Main.Config.CDZoomSteps,
                    Main.Config.CDUseBlinkEffect,
                    Main.Config.CDBlinkSpeed,
                    minFOV,
                    maxFOV,
                    CAMERA_TYPE,
                    ref blackFadeCoroutine
                );
            }
            else
            {
                ZoomUtils.HandleGradualZoom(
                    Main.CDZoomInButton,
                    Main.CDZoomOutButton,
                    zoomSpeed,
                    minFOV,
                    maxFOV
                );
            }
        }

        // Add zoom controls to camera bindings display
        [HarmonyPatch(typeof(uGUI_CameraDrone), nameof(uGUI_CameraDrone.UpdateBindings)), HarmonyPostfix]
        public static void uGUI_CameraDrone_UpdateBindings(uGUI_CameraDrone __instance)
        {
            if (!Main.Config.CCEnableFeature) return;

            string zoomIn = GameInput.FormatButton(Main.CDZoomInButton, false);
            string zoomOut = GameInput.FormatButton(Main.CDZoomOutButton, false);
            __instance.stringControls = __instance.stringControls.Insert(0, $"Use {zoomIn} and {zoomOut} to zoom\n");
        }
    }
}