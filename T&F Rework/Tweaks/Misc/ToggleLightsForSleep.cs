using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks;

[HarmonyPatch]
public class ToggleLightsForSleep
{
    private static bool IsDroneCameraActive()
    {
        return uGUI_CameraDrone.main.activeCamera != null;
    }

    private static bool CanToggleLights(SubRoot sub)
    {
        var player = Player.main;

        if (player == null
            || sub == null
            || sub.powerRelay == null
            || sub.powerRelay.GetPowerStatus() == PowerSystem.Status.Offline // - Base is Offline
            || !player.IsInBase() // Cyclops has its own toggle, exclude it
            || player.currentSub != sub
            || sub.lightingState == 1 // - Base is not in Danger state
            //   - lightingState: 0 = Operational, 1 = Danger (flooding), 2 = Damaged (no power or when lights are manually toggled off)
            || player.cinematicModeActive) // Sleeping
            return false;

        return true;
    }

    [HarmonyPatch(typeof(Bed), nameof(Bed.ResetAnimParams))]
    [HarmonyPostfix]
    public static void Bed_ResetAnimParams()
    {
        if (!Main.Config.ToggleLightsForSleep) return;

        var player = Player.main;
        if (!CanToggleLights(player.currentSub)) return;

        if (player.currentSub.subLightsOn)
        {
            player.currentSub.subLightsOn = false;
            FMODUWE.PlayOneShot(
                new FMODAsset
                {
                    id = "2102", path = "event:/sub/cyclops/lights_off", name = "95b877e8-2ccd-451d-ab5f-fc654feab173",
                    hideFlags = HideFlags.None
                }, MainCamera.camera.transform.position);
        }
    }

    [HarmonyPatch(typeof(Bed), nameof(Bed.OnCinematicEnd))]
    [HarmonyPostfix]
    public static void Bed_OnCinematicEnd()
    {
        if (!Main.Config.ToggleLightsForSleep) return;

        var player = Player.main;
        if (!CanToggleLights(player.currentSub)) return;

        Player.main.StartCoroutine(TurnLightsOnAfterDelay());
    }

    private static IEnumerator TurnLightsOnAfterDelay()
    {
        yield return new WaitForSeconds(Main.Config.LightsOnAfterSleepDelay);

        var player = Player.main;
        if (!CanToggleLights(player.currentSub) || player.currentSub.subLightsOn)
            yield break;

        player.currentSub.subLightsOn = true;
        FMODUWE.PlayOneShot(
            new FMODAsset
            {
                id = "2103", path = "event:/sub/cyclops/lights_on", name = "5384ec29-f493-4ac1-9f74-2c0b14d61440",
                hideFlags = HideFlags.None
            }, MainCamera.camera.transform.position);
    }
}