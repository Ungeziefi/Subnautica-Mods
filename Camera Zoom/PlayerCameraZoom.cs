using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Camera_Zoom
{
    public class PlayerCameraZoom
    {
        private static Camera Camera => SNCameraRoot.main.mainCamera;
        private static bool isZoomActive;
        private static bool isTransitioning;
        private static float originalFOV;
        private static float transitionStartFOV;
        private static float transitionTime;

        // Prevent PDACameraFOVControl from interfering
        [HarmonyPatch(typeof(PDACameraFOVControl))]
        public static class Camera_Zoom_PDACameraFOVControl
        {
            [HarmonyPatch(nameof(PDACameraFOVControl.Update)), HarmonyPrefix]
            public static bool Update()
            {
                return !(isZoomActive || isTransitioning);
            }
        }

        // Update the mask's anchors during zoom
        [HarmonyPatch(typeof(PlayerMask))]
        public static class PDACameraFOVControl_PlayerMask
        {
            [HarmonyPatch(nameof(PlayerMask.UpdateForCamera)), HarmonyPrefix]
            public static bool UpdateForCamera(PlayerMask __instance)
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
        }

        private static bool IsPlayerInValidState()
        {
            Player player = Player.main;
            if (player == null)
            {
                return false;
            }

            // Check for piloting modes
            if (player.mode == Player.Mode.Piloting && !Main.Config.PCAllowCyclopsZoom)
            {
                return false;  // Cyclops
            }

            if (player.mode == Player.Mode.LockedPiloting && !Main.Config.PCAllowVehicleZoom)
            {
                return false;  // Seamoth/PRAWN
            }

            return !CyclopsCameraZoom.isCameraActive && !Cursor.visible;
        }

        [HarmonyPatch(typeof(SNCameraRoot))]
        public static class Camera_Zoom_SNCameraRoot
        {
            [HarmonyPatch(nameof(SNCameraRoot.Update)), HarmonyPrefix]
            public static void Update()
            {
                if (!Main.Config.PCEnableFeature || Camera == null)
                {
                    return;
                }

                var config = Main.Config;

                if (isTransitioning && Cursor.visible)
                {
                    SNCameraRoot.main.SyncFieldOfView(originalFOV);
                    isTransitioning = false;
                    isZoomActive = false;
                    transitionTime = 0f;
                    // Main.Logger.LogInfo("Zoom reset - cursor visible");
                    return;
                }

                if (isZoomActive && !IsPlayerInValidState())
                {
                    isZoomActive = false;
                    isTransitioning = true;
                    transitionStartFOV = Camera.fieldOfView;
                    transitionTime = 0f;
                    // Main.Logger.LogInfo("Zoom reset - invalid player state");
                }

                if (IsPlayerInValidState() && (Input.GetKeyDown(config.PCZoomKey) || Input.GetKeyDown(config.PCSecondaryZoomKey)))
                {
                    if (!isZoomActive && !isTransitioning)
                    {
                        originalFOV = Camera.fieldOfView;
                        // Main.Logger.LogInfo($"Saved new original FOV: {originalFOV:F2}");
                    }

                    isZoomActive = !isZoomActive;

                    if (config.PCInstantZoom)
                    {
                        // For instant zoom, just set the FOV directly
                        float targetFOV = isZoomActive ? config.PCTargetFOV : originalFOV;
                        SNCameraRoot.main.SyncFieldOfView(targetFOV);
                        // Main.Logger.LogInfo($"Instant zoom complete: FOV={targetFOV:F2}, ZoomActive={isZoomActive}");
                    }
                    else
                    {
                        // For smooth zoom, start the transition
                        isTransitioning = true;
                        transitionStartFOV = Camera.fieldOfView;
                        transitionTime = 0f;
                        // Main.Logger.LogInfo($"Zoom toggled: {isZoomActive}");
                    }
                }

                // Only process transition if instant zoom is disabled
                if (isTransitioning && !config.PCInstantZoom)
                {
                    float transitionDuration = 1f / config.PCZoomSpeed;

                    transitionTime += Time.deltaTime;
                    float t = Mathf.Clamp01(transitionTime / transitionDuration);
                    t = t * t * (3f - 2f * t);

                    float targetFOV = isZoomActive ? config.PCTargetFOV : originalFOV;
                    float newFOV = Mathf.Lerp(transitionStartFOV, targetFOV, t);

                    SNCameraRoot.main.SyncFieldOfView(newFOV);
                    // Main.Logger.LogInfo($"Transitioning FOV: {newFOV:F2}, Progress: {t:F2}, Speed: {config.PCZoomSpeed:F2}");

                    if (t >= 1f)
                    {
                        SNCameraRoot.main.SyncFieldOfView(targetFOV);
                        isTransitioning = false;
                        transitionTime = 0f;
                        // Main.Logger.LogInfo($"Zoom transition complete: FOV={targetFOV:F2}, ZoomActive={isZoomActive}");
                    }
                }
            }
        }
    }
}