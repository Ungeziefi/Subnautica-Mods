using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Camera_Zoom
{
    [HarmonyPatch]
    public class PlayerCameraZoom
    {
        private static Camera Camera => SNCameraRoot.main.mainCamera;
        private static bool isZoomActive;
        private static bool isTransitioning;
        private static float originalFOV;
        private static float transitionStartFOV;
        private static float transitionTime;

        // Drone Camera check
        private static bool IsDroneCameraActive()
        {
            if (uGUI_CameraDrone.main && uGUI_CameraDrone.main.gameObject.activeInHierarchy)
            {
                MapRoomCamera drone = uGUI_CameraDrone.main.GetCamera();
                return drone != null && drone.IsReady();
            }
            return false;
        }

        // Check for menu, piloting, Drone Camera
        // Toggle Seamoth/PRAWN Suit zoom
        private static bool IsValidState()
        {
            Player player = Player.main;

            if (player == null ||
                Camera == null ||
                Cursor.visible ||
                player.mode == Player.Mode.Piloting ||
                IsDroneCameraActive())
            {
                return false;
            }

            if (player.mode == Player.Mode.LockedPiloting && !Main.Config.PCAllowVehicleZoom)
            {
                return false;
            }

            return !CyclopsCameraZoom.isCameraActive && !Cursor.visible;
        }

        // Prevent PDACameraFOVControl from interfering
        [HarmonyPatch(typeof(PDACameraFOVControl), nameof(PDACameraFOVControl.Update)), HarmonyPrefix]
        public static bool PDACameraFOVControl_Update()
        {
            return !(isZoomActive || isTransitioning);
        }

        // Update mask anchors during zoom
        [HarmonyPatch(typeof(PlayerMask), nameof(PlayerMask.UpdateForCamera)), HarmonyPrefix]
        public static bool PlayerMask_UpdateForCamera(PlayerMask __instance)
        {
            if (isZoomActive || isTransitioning)
            {
                Vector3 topLeftAnchor, topMiddleAnchor, topRightAnchor,
                       bottomLeftAnchor, bottomMiddleAnchor, bottomRightAnchor;

                __instance.GetViewSpaceAnchors(originalFOV, Camera.aspect,
                    out topLeftAnchor, out topMiddleAnchor, out topRightAnchor,
                    out bottomLeftAnchor, out bottomMiddleAnchor, out bottomRightAnchor);

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

        // Zoom in/out
        [HarmonyPatch(typeof(SNCameraRoot), nameof(SNCameraRoot.Update)), HarmonyPrefix]
        public static void SNCameraRoot_Update()
        {
            var config = Main.Config;

            if (!config.PCEnableFeature || Camera == null)
            {
                return;
            }

            // Reset zoom if invalid state
            if (!IsValidState() && (isZoomActive || isTransitioning))
            {
                ResetZoomState(originalFOV);
                return;
            }

            // Handle zoom toggle input
            bool zoomKeyPressed = Input.GetKeyDown(config.PCZoomKey) ||
                                 Input.GetKeyDown(config.PCSecondaryZoomKey);

            if (IsValidState() && zoomKeyPressed)
            {
                if (!isZoomActive && !isTransitioning)
                {
                    originalFOV = Camera.fieldOfView;
                }

                isZoomActive = !isZoomActive;

                if (config.PCInstantZoom)
                {
                    SNCameraRoot.main.SyncFieldOfView(isZoomActive ? config.PCTargetFOV : originalFOV);
                }
                else
                {
                    StartTransition(isZoomActive);
                }
            }

            // Handle smooth transition
            if (isTransitioning && !config.PCInstantZoom)
            {
                // Check state during zoom transition
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
            isTransitioning = false;
            isZoomActive = false;
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