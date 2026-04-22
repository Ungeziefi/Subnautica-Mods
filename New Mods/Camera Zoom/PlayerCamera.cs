using HarmonyLib;
using UnityEngine;
using UWE;

namespace Ungeziefi.Camera_Zoom;

[HarmonyPatch]
public class PlayerCamera
{
    private static bool isZoomed, isTweening;
    private static float startFOV, zoomT, baseFOV;

    private static bool canZoom()
    {
        var isPausedOrLoading = WaitScreen.IsWaiting
                                || Cursor.visible
                                || FreezeTime.HasFreezers()
                                || Player.main.GetPDA().isOpen;

        var player = Player.main;

        return player != null &&
               SNCameraRoot.main.mainCamera != null &&
               !isPausedOrLoading &&
               !uGUI_CameraDrone.main.activeCamera &&
               !CyclopsCameras.IsCameraActive &&
               (Main.Config.PCAllowWhileBuilding || !Builder.isPlacing);
    }

    public static void ForceReset()
    {
        if (!isZoomed && !isTweening) return;

        ZoomUtils.ApplyFOV(baseFOV);
        isZoomed = isTweening = false;
    }

    [HarmonyPatch(typeof(SNCameraRoot), nameof(SNCameraRoot.Update))]
    [HarmonyPrefix]
    public static void SNCameraRoot_Update()
    {
        if (SNCameraRoot.main == null) return;
        var config = Main.Config;
        var inVehicle = Player.main.mode == Player.Mode.LockedPiloting;
        if (inVehicle ? !config.VCEnableFeature : !config.PCEnableFeature) return;

        var key = inVehicle ? Main.VCZoomButton : Main.PCZoomButton;
        var target = inVehicle ? config.VCTargetFOV : config.PCTargetFOV;
        var speed = inVehicle ? config.VCZoomSpeed : config.PCZoomSpeed;
        var instant = inVehicle ? config.VCInstantZoom : config.PCInstantZoom;

        if (canZoom() && GameInput.GetButtonDown(key))
        {
            if (!isZoomed && !isTweening) baseFOV = SNCameraRoot.main.mainCamera.fieldOfView;
            isZoomed = !isZoomed;
            if (instant)
            {
                ZoomUtils.ApplyFOV(isZoomed ? target : baseFOV);
            }
            else
            {
                isTweening = true;
                startFOV = SNCameraRoot.main.mainCamera.fieldOfView;
                zoomT = 0;
            }
        }

        if (isTweening)
        {
            zoomT = Mathf.MoveTowards(zoomT, 1f, Time.deltaTime * speed);
            ZoomUtils.ApplyFOV(Mathf.Lerp(startFOV, isZoomed ? target : baseFOV, zoomT * zoomT * (3f - 2f * zoomT)));
            if (zoomT >= 1f) isTweening = false;
        }

        if (!canZoom() && (isZoomed || isTweening)) ForceReset();
    }

    // Don't update mask anchors during zoom
    [HarmonyPatch(typeof(PlayerMask), nameof(PlayerMask.UpdateForCamera))]
    [HarmonyPrefix]
    public static bool PlayerMask_UpdateForCamera(PlayerMask __instance)
    {
        return !isZoomed && !isTweening;
    }
}