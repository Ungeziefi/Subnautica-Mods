using HarmonyLib;
using UnityEngine;
using UWE;

namespace Ungeziefi.Cockpit_Free_Look;

[HarmonyPatch]
public partial class CockpitFreeLook
{
    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    [HarmonyPostfix]
    public static void Player_Update()
    {
        if (!ShouldProcessFreeLook(out var vehicle, out var isExosuit)) return;

        if (Main.Config.HoldKeyMode)
            HandleHoldMode(vehicle, isExosuit);
        else
            HandleToggleMode(vehicle, isExosuit);
    }

    private static bool ShouldProcessFreeLook(out Vehicle vehicle, out bool isExosuit)
    {
        vehicle = null;
        isExosuit = false;

        if (!Main.Config.SeamothEnableFeature && !Main.Config.PRAWNEnableFeature)
            return false;

        var player = Player.main;
        if (player == null
            || player.mode != Player.Mode.LockedPiloting
            || Cursor.visible
            || FreezeTime.HasFreezers()
            || Player.main.GetPDA().isOpen)
            return false;

        vehicle = player.currentMountedVehicle;
        if (vehicle == null)
            return false;

        isExosuit = vehicle is Exosuit;
        var isValidVehicle = (isExosuit && Main.Config.PRAWNEnableFeature) ||
                             (!isExosuit && Main.Config.SeamothEnableFeature);

        return isValidVehicle;
    }

    private static void HandleHoldMode(Vehicle vehicle, bool isExosuit)
    {
        var isButtonHeld = GameInput.GetButtonHeld(Main.FreeLookButton);

        if (isButtonHeld && !isLooking)
            StartFreeLook(vehicle, isExosuit);
        else if (!isButtonHeld && isLooking) EndFreeLook();
    }

    private static void HandleToggleMode(Vehicle vehicle, bool isExosuit)
    {
        var isButtonPressed = GameInput.GetButtonDown(Main.FreeLookButton);

        if (isButtonPressed)
        {
            if (!isLooking)
                StartFreeLook(vehicle, isExosuit);
            else
                EndFreeLook();
        }
    }

    private static void StartFreeLook(Vehicle vehicle, bool isExosuit)
    {
        isLooking = true;
        isReturning = false;
        originalRotation = MainCamera.camera.transform.localRotation;
        currentRotation = Vector2.zero;

        if (isExosuit) DisableExosuitArms(vehicle as Exosuit);
    }

    private static void EndFreeLook()
    {
        isLooking = false;
        isReturning = true;
        returnTime = 0f;
    }
}