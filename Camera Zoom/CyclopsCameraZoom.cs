using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Camera_Zoom
{
    [HarmonyPatch]
    public class CyclopsCameraZoom
    {
        private static Camera Camera => SNCameraRoot.main.mainCamera;
        private static readonly float minFOV = Main.Config.CCMinimumFOV;
        private static readonly float maxFOV = Main.Config.CCMaximumFOV;
        private static readonly float zoomSpeed = Main.Config.CCZoomSpeed;
        internal static bool isCameraActive;
        private static float previousFOV;
        private static void ResetAndDisable(bool disable)
        {
            if (Camera == null || SNCameraRoot.main == null)
            {
                isCameraActive = false;
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

        // Save FOV when entering camera
        [HarmonyPatch(typeof(CyclopsExternalCamsButton), nameof(CyclopsExternalCamsButton.CameraButtonActivated)), HarmonyPrefix]
        public static void CyclopsExternalCamsButton_CameraButtonActivated(CyclopsExternalCamsButton __instance)
        {
            previousFOV = Camera.fieldOfView;
        }

        // Camera enter and exit
        [HarmonyPatch(typeof(CyclopsExternalCams), nameof(CyclopsExternalCams.SetActive)), HarmonyPostfix]
        public static void CyclopsExternalCams_SetActive(CyclopsExternalCams __instance)
        {
            isCameraActive = __instance.active;
            // Main.Logger.LogInfo($"Camera SetActive called: {isCameraActive}");
            if (!isCameraActive)
            {
                ResetAndDisable(true);
            }
        }

        // Camera switch
        [HarmonyPatch(typeof(uGUI_CameraCyclops), nameof(uGUI_CameraCyclops.SetCamera)), HarmonyPostfix]
        public static void uGUI_CameraCyclops_SetCamera()
        {
            // Main.Logger.LogInfo("Camera SetCamera called.");
            ResetAndDisable(false);
        }

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
            {
                return;
            }

            var config = Main.Config;
            int zoomDirection = 0; // 0 = no zoom, 1 = zoom out, -1 = zoom in

            if (Input.GetKey(Main.Config.CCZoomInKey) || GameInput.GetButtonHeld(GameInput.Button.MoveForward))
            {
                zoomDirection = -1;
            }
            else if (Input.GetKey(Main.Config.CCZoomOutKey) || GameInput.GetButtonHeld(GameInput.Button.MoveBackward))
            {
                zoomDirection = 1;
            }

            if (zoomDirection != 0)
            {
                float previousFOV = MiscSettings.fieldOfView;
                float newFOV = Mathf.Clamp(
                    previousFOV + (zoomDirection * zoomSpeed * Time.deltaTime),
                    minFOV,
                    maxFOV
                );

                if (newFOV != previousFOV && SNCameraRoot.main != null)
                {
                    MiscSettings.fieldOfView = newFOV;
                    SNCameraRoot.main.SyncFieldOfView(newFOV);
                    // Main.Logger.LogInfo($"FOV change: Previous={previousFOV}, New={newFOV}, Delta={(newFOV - previousFOV):F2}");
                }
            }
        }
    }
}