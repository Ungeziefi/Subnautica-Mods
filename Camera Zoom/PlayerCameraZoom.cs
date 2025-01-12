using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Camera_Zoom
{
    [HarmonyPatch]
    public class PlayerCameraZoom
    {
        private static Camera Camera => SNCameraRoot.main.mainCamera;
        private static bool isZoomActive, isTransitioning;
        private static float originalFOV, transitionStartFOV, transitionTime;

        // Drone Camera check
        private static bool IsDroneCameraActive() =>
            uGUI_CameraDrone.main && uGUI_CameraDrone.main.gameObject.activeInHierarchy &&
            uGUI_CameraDrone.main.GetCamera()?.IsReady() == true;

        // Check for menu, piloting, Drone Camera
        // Toggle Seamoth/PRAWN Suit zoom
        private static bool IsValidState()
        {
            var player = Player.main;
            return player != null && Camera != null && !Cursor.visible &&
                   player.mode != Player.Mode.Piloting && !IsDroneCameraActive() &&
                   (player.mode != Player.Mode.LockedPiloting || Main.Config.PCAllowVehicleZoom);
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
            if (!config.PCEnableFeature || Camera == null) return;

            // Reset when invalid state
            if (!IsValidState() && (isZoomActive || isTransitioning))
            {
                ResetZoomState(originalFOV);
                return;
            }

            // Input and set original FOV
            if (IsValidState() && (Input.GetKeyDown(config.PCZoomKey) || Input.GetKeyDown(config.PCSecondaryZoomKey)))
            {
                if (!isZoomActive && !isTransitioning) originalFOV = Camera.fieldOfView;
                isZoomActive = !isZoomActive;
                if (config.PCInstantZoom)
                    SNCameraRoot.main.SyncFieldOfView(isZoomActive ? config.PCTargetFOV : originalFOV);
                else
                    StartTransition(isZoomActive);
            }

            // Reset if invalid during transition
            if (isTransitioning && !config.PCInstantZoom)
            {
                if (!IsValidState())
                {
                    ResetZoomState(originalFOV);
                    return;
                }
                UpdateTransition(config);
            }
        }
        private static void ResetZoomState(float targetFOV)
        {
            SNCameraRoot.main.SyncFieldOfView(targetFOV);
            isTransitioning = isZoomActive = false;
            transitionTime = 0f;
        }

        private static void StartTransition(bool zoomingIn)
        {
            isTransitioning = true;
            transitionStartFOV = Camera.fieldOfView;
            transitionTime = 0f;
        }

        private static void UpdateTransition(Config config)
        {
            float transitionDuration = 1f / config.PCZoomSpeed;
            transitionTime += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTime / transitionDuration);
            t = t * t * (3f - 2f * t); // Smoothstep interpolation
            float targetFOV = isZoomActive ? config.PCTargetFOV : originalFOV;
            float newFOV = Mathf.Lerp(transitionStartFOV, targetFOV, t);
            SNCameraRoot.main.SyncFieldOfView(newFOV);
            if (t >= 1f)
            {
                isTransitioning = false;
                transitionTime = 0f;
                SNCameraRoot.main.SyncFieldOfView(targetFOV);
            }
        }
    }
}