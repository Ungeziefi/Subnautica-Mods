using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch(typeof(uGUI_CameraCyclops))]
    public class CyclopsCameraZoom
    {
        private static Camera cyclopsCamera;
        private static float defaultFOV;
        private static float minFOV = Main.Config.CCZMinimumFOV;
        private static float maxFOV = Main.Config.CCZMaximumFOV;
        private static float zoomSpeed = Main.Config.CCZZoomSpeed;
        private static bool isCameraActive = false;

        [HarmonyPatch(nameof(uGUI_CameraCyclops.OnEnable)), HarmonyPrefix]
        public static void OnEnable(uGUI_CameraCyclops __instance)
        {
            FindCyclopsCamera();
        }

        [HarmonyPatch(nameof(uGUI_CameraCyclops.OnDisable)), HarmonyPrefix]
        public static void OnDisable(uGUI_CameraCyclops __instance)
        {
            ResetZoom();
            isCameraActive = false;
        }

        [HarmonyPatch(nameof(uGUI_CameraCyclops.SetCamera)), HarmonyPostfix]
        public static void SetCamera(uGUI_CameraCyclops __instance, int index)
        {
            // Reset the zoom also when changing the camera
            ResetZoom();
        }

        [HarmonyPatch(nameof(uGUI_CameraCyclops.Update)), HarmonyPostfix]
        public static void Update(uGUI_CameraCyclops __instance)
        {
            if (!Main.Config.CyclopsCameraZoom || !isCameraActive)
            {
                return;
            }

            HandleZoom();
        }

        [HarmonyPatch(typeof(CyclopsExternalCams), "SetActive"), HarmonyPostfix]
        public static void SetActive_Postfix(CyclopsExternalCams __instance)
        {
            if (__instance.active)
            {
                isCameraActive = true;
                // Main.Logger.LogInfo("Cyclops external cams on.");
            }
            else
            {
                isCameraActive = false;
                // Main.Logger.LogInfo("Cyclops external cams off.");
            }
        }

        private static void FindCyclopsCamera()
        {
            // Try to find the camera within the player camera system
            if (Player.main != null)
            {
                cyclopsCamera = Player.main.GetComponentInChildren<Camera>();
                if (cyclopsCamera != null)
                {
                    defaultFOV = cyclopsCamera.fieldOfView;
                    // Main.Logger.LogInfo("Cyclops camera initialized with default FOV: " + defaultFOV);
                }
            }
        }

        private static void ResetZoom()
        {
            if (cyclopsCamera != null)
            {
                cyclopsCamera.fieldOfView = defaultFOV;
                // Main.Logger.LogInfo("Cyclops camera zoom reset to default FOV: " + defaultFOV);
            }
        }

        private static void HandleZoom()
        {
            if (cyclopsCamera != null)
            {
                var config = Main.Config;

                if (Input.GetKey(config.CCZZoomInKey))
                {
                    // Main.Logger.LogInfo("Zooming in with key: " + config.ZoomInKey);
                    cyclopsCamera.fieldOfView -= zoomSpeed * Time.deltaTime;
                    cyclopsCamera.fieldOfView = Mathf.Clamp(cyclopsCamera.fieldOfView, minFOV, maxFOV);
                }

                else if (Input.GetKey(config.CCZZoomOutKey))
                {
                    // Main.Logger.LogInfo("Zooming out with key: " + config.ZoomOutKey);
                    cyclopsCamera.fieldOfView += zoomSpeed * Time.deltaTime;
                    cyclopsCamera.fieldOfView = Mathf.Clamp(cyclopsCamera.fieldOfView, minFOV, maxFOV);
                }
            }
        }
    }
}