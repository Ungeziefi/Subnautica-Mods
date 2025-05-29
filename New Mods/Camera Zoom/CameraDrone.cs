using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Camera_Zoom
{
    [HarmonyPatch]
    public class CameraDrone
    {
        private static Camera Camera => SNCameraRoot.main.mainCamera;
        private static bool IsCameraActive => uGUI_CameraDrone.main?.activeCamera != null;
        private static readonly float minFOV = Main.Config.CDMinimumFOV;
        private static readonly float maxFOV = Main.Config.CDMaximumFOV;
        private static readonly float zoomSpeed = Main.Config.CDZoomSpeed;
        private static float previousFOV;

        private static void ResetAndDisable(bool disable)
        {
            if (Camera == null || SNCameraRoot.main == null)
                return;

            if (disable)
            {
                // Restore FOV
                Camera.fieldOfView = previousFOV;
                MiscSettings.fieldOfView = previousFOV;
                SNCameraRoot.main.SyncFieldOfView(previousFOV);
            }
            else
            {
                Camera.fieldOfView = maxFOV;
            }
        }

        // Save FOV on enter
        [HarmonyPatch(typeof(MapRoomCamera), nameof(MapRoomCamera.ControlCamera)), HarmonyPrefix]
        public static void MapRoomCamera_ControlCamera(MapRoomCamera __instance)
        {
            previousFOV = Camera.fieldOfView;
        }

        // Reset on exit
        [HarmonyPatch(typeof(MapRoomCamera), nameof(MapRoomCamera.FreeCamera)), HarmonyPostfix]
        public static void MapRoomCamera_FreeCamera(MapRoomCamera __instance)
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

            int zoomDirection = 0;
            if (Input.GetKey(Main.Config.CDZoomInKey))
                zoomDirection = -1; // Zoom in
            else if (Input.GetKey(Main.Config.CDZoomOutKey))
                zoomDirection = 1; // Zoom out

            if (zoomDirection != 0)
            {
                float currentFOV = Camera.fieldOfView;
                float newFOV = Mathf.Clamp(
                    currentFOV + (zoomDirection * zoomSpeed * Time.deltaTime),
                    minFOV,
                    maxFOV
                );

                if (newFOV != currentFOV && SNCameraRoot.main != null)
                {
                    MiscSettings.fieldOfView = newFOV;
                    SNCameraRoot.main.SyncFieldOfView(newFOV);
                }
            }
        }
    }
}