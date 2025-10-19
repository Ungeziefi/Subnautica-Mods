using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Camera_Zoom
{
    [HarmonyPatch]
    public class PlayerCamera
    {
        private static Camera Camera => SNCameraRoot.main.mainCamera;
        private static bool IsDroneCameraActive() => uGUI_CameraDrone.main?.activeCamera != null;
        private static bool isZoomActive, isTransitioning;
        private static float originalFOV, transitionStartFOV, transitionTime;

        private static bool IsInVehicle()
        {
            var player = Player.main;
            return player != null && player.mode == Player.Mode.LockedPiloting;
        }

        // Check for menu, piloting, Drone Camera, and building state
        private static bool IsValidState()
        {
            var player = Player.main;
            return player != null &&
                   Camera != null &&
                   !Cursor.visible &&
                   player.mode != Player.Mode.Piloting &&
                   !IsDroneCameraActive() &&
                   (Main.Config.PCAllowWhileBuilding || !Builder.isPlacing);
        }

        // Prevent PDACameraFOVControl from interfering
        [HarmonyPatch(typeof(PDACameraFOVControl), nameof(PDACameraFOVControl.Update)), HarmonyPrefix]
        public static bool PDACameraFOVControl_Update() => !(isZoomActive || isTransitioning);

        // Update mask anchors during zoom
        [HarmonyPatch(typeof(PlayerMask), nameof(PlayerMask.UpdateForCamera)), HarmonyPrefix]
        public static bool PlayerMask_UpdateForCamera(PlayerMask __instance)
        {
            if (isZoomActive || isTransitioning)
            {
                __instance.GetViewSpaceAnchors(originalFOV, Camera.aspect,
                    out var topLeftAnchor, out var topMiddleAnchor, out var topRightAnchor,
                    out var bottomLeftAnchor, out var bottomMiddleAnchor, out var bottomRightAnchor);

                __instance.topLeft.localPosition = topLeftAnchor - __instance.topLeftOffset + __instance.topLeftStartPosition;
                __instance.topMiddle.localPosition = topMiddleAnchor - __instance.topMiddleOffset + __instance.topMiddleStartPosition;
                __instance.topRight.localPosition = topRightAnchor - __instance.topRightOffset + __instance.topRightStartPosition;
                __instance.bottomLeft.localPosition = bottomLeftAnchor - __instance.bottomLeftOffset + __instance.bottomLeftStartPosition;
                __instance.bottomMiddle.localPosition = bottomMiddleAnchor - __instance.bottomMiddleOffset + __instance.bottomMiddleStartPosition;
                __instance.bottomRight.localPosition = bottomRightAnchor - __instance.bottomRightOffset + __instance.bottomRightStartPosition;

                __instance.currentFov = Camera.fieldOfView;
                __instance.currentAspect = Camera.aspect;
                return false;
            }
            return true;
        }

        // Zoom
        [HarmonyPatch(typeof(SNCameraRoot), nameof(SNCameraRoot.Update)), HarmonyPrefix]
        public static void SNCameraRoot_Update()
        {
            var config = Main.Config;
            if (Camera == null) return;

            bool isVehicle = IsInVehicle();

            if ((!config.PCEnableFeature && !isVehicle) ||
                (!config.VCEnableFeature && isVehicle))
                return;

            // Reset when invalid state
            if (!IsValidState() && (isZoomActive || isTransitioning))
            {
                ResetZoomState();
                return;
            }

            // Get context-specific settings
            bool instantZoom = isVehicle ? config.VCInstantZoom : config.PCInstantZoom;
            GameInput.Button zoomKey = isVehicle ? Main.VCZoomButton : Main.PCZoomButton;
            float targetFOV = isVehicle ? config.VCTargetFOV : config.PCTargetFOV;
            float zoomSpeed = isVehicle ? config.VCZoomSpeed : config.PCZoomSpeed;

            // Input and set original FOV
            if (IsValidState() && GameInput.GetButtonDown(zoomKey))
            {
                if (!isZoomActive && !isTransitioning) originalFOV = Camera.fieldOfView;
                isZoomActive = !isZoomActive;

                if (instantZoom)
                {
                    ZoomUtils.ApplyFOV(isZoomActive ? targetFOV : originalFOV);
                }
                else
                {
                    StartTransition(isZoomActive);
                }
            }

            // Reset if invalid during transition
            if (isTransitioning && !instantZoom)
            {
                if (!IsValidState())
                {
                    ResetZoomState();
                    return;
                }
                UpdateTransition(targetFOV, zoomSpeed);
            }
        }

        private static void ResetZoomState()
        {
            ZoomUtils.ApplyFOV(originalFOV);
            isTransitioning = isZoomActive = false;
            transitionTime = 0f;
        }

        private static void StartTransition(bool zoomingIn)
        {
            isTransitioning = true;
            transitionStartFOV = Camera.fieldOfView;
            transitionTime = 0f;
        }

        private static void UpdateTransition(float targetFOV, float zoomSpeed)
        {
            float transitionDuration = 1f / zoomSpeed;
            transitionTime += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTime / transitionDuration);
            t = t * t * (3f - 2f * t); // Smoothstep interpolation
            float newFOV = Mathf.Lerp(transitionStartFOV, isZoomActive ? targetFOV : originalFOV, t);

            ZoomUtils.ApplyFOV(newFOV);

            if (t >= 1f)
            {
                isTransitioning = false;
                transitionTime = 0f;
                ZoomUtils.ApplyFOV(isZoomActive ? targetFOV : originalFOV);
            }
        }
    }
}