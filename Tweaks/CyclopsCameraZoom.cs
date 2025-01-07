using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch(typeof(uGUI_CameraCyclops))]
    public class CyclopsCameraZoom
    {
        private static Camera Camera => SNCameraRoot.main.mainCamera;
        private static readonly float minFOV = Main.Config.CCZMinimumFOV;
        private static readonly float maxFOV = Main.Config.CCZMaximumFOV;
        private static readonly float zoomSpeed = Main.Config.CCZZoomSpeed;
        private static bool isCameraActive;
        private static float previousFOV;

        // Camera switch
        [HarmonyPatch(nameof(uGUI_CameraCyclops.SetCamera)), HarmonyPostfix]
        public static void SetCamera()
        {
            // Main.Logger.LogInfo("Camera SetCamera called.");
            ResetAndDisable(false);
        }

        // Save the FOV when entering camera mode
        [HarmonyPatch(typeof(CyclopsExternalCamsButton), nameof(CyclopsExternalCamsButton.CameraButtonActivated)), HarmonyPrefix]
        public static void CameraButtonActivated(CyclopsExternalCamsButton __instance)
        {
            previousFOV = Camera.fieldOfView;
        }

        // Camera enter and exit
        [HarmonyPatch(typeof(CyclopsExternalCams), nameof(CyclopsExternalCams.SetActive)), HarmonyPostfix]
        public static void SetActive(CyclopsExternalCams __instance)
        {
            isCameraActive = __instance.active;
            // Main.Logger.LogInfo($"Camera SetActive called: {isCameraActive}");
            if (!isCameraActive)
            {
                ResetAndDisable(true);
            }
        }

        [HarmonyPatch(nameof(uGUI_CameraCyclops.Update)), HarmonyPostfix]
        public static void Update()
        {
            if (!Main.Config.CyclopsCameraZoom || !isCameraActive || Camera == null)
            {
                return;
            }

            var config = Main.Config;
            float zoomDirection = 0f;

            if (Input.GetKey(config.CCZZoomInKey))
            {
                zoomDirection = -1f;
            }
            else if (Input.GetKey(config.CCZZoomOutKey))
            {
                zoomDirection = 1f;
            }

            if (zoomDirection != 0f)
            {
                float previousFOV = MiscSettings.fieldOfView;
                float newFOV = Mathf.Clamp(
                    previousFOV + (zoomDirection * zoomSpeed * Time.deltaTime),
                    minFOV,
                    maxFOV
                );

                if (newFOV != previousFOV)
                {
                    MiscSettings.fieldOfView = newFOV;
                    SNCameraRoot.main.SyncFieldOfView(newFOV);
                    // Main.Logger.LogInfo($"FOV change: Previous={previousFOV}, New={newFOV}, Delta={(newFOV - previousFOV):F2}");
                }
            }
        }

        private static void ResetAndDisable(bool disable)
        {
            if (Camera == null)
            {
                return;
            }

            if (disable)
            {
                isCameraActive = false;
                MiscSettings.fieldOfView = previousFOV;
                SNCameraRoot.main.SyncFieldOfView(previousFOV);
                // Main.Logger.LogInfo($"Exiting camera, reverting to original FOV: {previousFOV}");
            }
            else
            {
                MiscSettings.fieldOfView = maxFOV;
                SNCameraRoot.main.SyncFieldOfView(maxFOV);
                // Main.Logger.LogInfo($"Switching camera, resetting FOV to maximum: {maxFOV}");
            }
        }
    }
}