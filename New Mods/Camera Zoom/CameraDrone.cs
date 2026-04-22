using HarmonyLib;
using UnityEngine;
using UWE;

namespace Ungeziefi.Camera_Zoom;

[HarmonyPatch]
public class CameraDrone
{
    private static int currentStep;

    // Enter camera
    [HarmonyPatch(typeof(MapRoomCamera), nameof(MapRoomCamera.ControlCamera))]
    [HarmonyPrefix]
    public static void MapRoomCamera_ControlCamera()
    {
        ZoomUtils.PrepareForCameraMode();
        currentStep = 0;
    }

    // Exit or change camera
    [HarmonyPatch(typeof(MapRoomCamera), nameof(MapRoomCamera.FreeCamera))]
    [HarmonyPostfix]
    public static void MapRoomCamera_FreeCamera()
    {
        ZoomUtils.ApplyFOV(ZoomUtils.DefaultFOV);
    }

    [HarmonyPatch(typeof(MapRoomCamera), nameof(MapRoomCamera.HandleInput))]
    [HarmonyPostfix]
    public static void MapRoomCamera_HandleInput()
    {
        if (!Main.Config.CDEnableFeature) return;

        var isPausedOrLoading = WaitScreen.IsWaiting
                                || Cursor.visible
                                || FreezeTime.HasFreezers()
                                || Player.main.GetPDA().isOpen;

        if (isPausedOrLoading) return;

        if (Main.Config.CDSteppedZoom)
            ZoomUtils.HandleSteppedZoom(GameInput.GetButtonDown(Main.CDZoomInButton),
                GameInput.GetButtonDown(Main.CDZoomOutButton), ref currentStep, Main.Config.CDZoomSteps,
                Main.Config.CDUseBlinkEffect, Main.Config.CDBlinkSpeed, Main.Config.CDMinimumFOV,
                Main.Config.CDMaximumFOV);
        else
            ZoomUtils.HandleGradualZoom(Main.CDZoomInButton, Main.CDZoomOutButton, Main.Config.CDZoomSpeed,
                Main.Config.CDMinimumFOV, Main.Config.CDMaximumFOV);
    }
}