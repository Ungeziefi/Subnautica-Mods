using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Camera_Zoom
{
    [HarmonyPatch]
    public class CyclopsCamerasZoom
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
            isCameraActive = !disable;
            MiscSettings.fieldOfView = disable ? previousFOV : maxFOV;
            SNCameraRoot.main.SyncFieldOfView(MiscSettings.fieldOfView);
        }

        // Save FOV on enter
        [HarmonyPatch(typeof(CyclopsExternalCamsButton), nameof(CyclopsExternalCamsButton.CameraButtonActivated)), HarmonyPrefix]
        public static void CyclopsExternalCamsButton_CameraButtonActivated() => previousFOV = Camera.fieldOfView;

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
            if (!Main.Config.CCEnableFeature || !isCameraActive || Cursor.visible) return;

            int zoomDirection = 0;
            if (Input.GetKey(Main.Config.CCZoomInKey) || GameInput.GetButtonHeld(GameInput.Button.MoveForward))
                zoomDirection = -1; // Zoom in
            else if (Input.GetKey(Main.Config.CCZoomOutKey) || GameInput.GetButtonHeld(GameInput.Button.MoveBackward))
                zoomDirection = 1; // Zoom out

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
                }
            }
        }
    }
}