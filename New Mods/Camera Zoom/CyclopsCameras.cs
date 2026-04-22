using HarmonyLib;
using UnityEngine;
using UWE;

namespace Ungeziefi.Camera_Zoom;

[HarmonyPatch]
public class CyclopsCameras
{
    private static int currentStep;
    public static bool IsCameraActive { get; private set; }

    // Enter camera
    [HarmonyPatch(typeof(CyclopsExternalCamsButton), nameof(CyclopsExternalCamsButton.CameraButtonActivated))]
    [HarmonyPrefix]
    public static void CyclopsExternalCamsButton_CameraButtonActivated()
    {
        ZoomUtils.PrepareForCameraMode();
        currentStep = 0;
    }

    // Flag state and exit if inactive
    [HarmonyPatch(typeof(CyclopsExternalCams), nameof(CyclopsExternalCams.SetActive))]
    [HarmonyPostfix]
    public static void CyclopsExternalCams_SetActive(CyclopsExternalCams __instance)
    {
        IsCameraActive = __instance.active;
        if (!IsCameraActive) ZoomUtils.ApplyFOV(ZoomUtils.DefaultFOV);
    }

    // Switch camera
    [HarmonyPatch(typeof(uGUI_CameraCyclops), nameof(uGUI_CameraCyclops.SetCamera))]
    [HarmonyPostfix]
    public static void uGUI_CameraCyclops_SetCamera()
    {
        ZoomUtils.ApplyFOV(ZoomUtils.DefaultFOV);
        currentStep = 0;
    }

    [HarmonyPatch(typeof(uGUI_CameraCyclops), nameof(uGUI_CameraCyclops.Update))]
    [HarmonyPostfix]
    public static void uGUI_CameraCyclops_Update()
    {
        if (!Main.Config.CCEnableFeature) return;

        var isPausedOrLoading = WaitScreen.IsWaiting
                                || Cursor.visible
                                || FreezeTime.HasFreezers()
                                || Player.main.GetPDA().isOpen;

        if (!IsCameraActive || isPausedOrLoading) return;

        if (Main.Config.CCSteppedZoom)
            ZoomUtils.HandleSteppedZoom(GameInput.GetButtonDown(Main.CCZoomInButton),
                GameInput.GetButtonDown(Main.CCZoomOutButton), ref currentStep, Main.Config.CCZoomSteps,
                Main.Config.CCUseBlinkEffect, Main.Config.CCBlinkSpeed, Main.Config.CCMinimumFOV,
                Main.Config.CCMaximumFOV);
        else
            ZoomUtils.HandleGradualZoom(Main.CCZoomInButton, Main.CCZoomOutButton, Main.Config.CCZoomSpeed,
                Main.Config.CCMinimumFOV, Main.Config.CCMaximumFOV);
    }
}