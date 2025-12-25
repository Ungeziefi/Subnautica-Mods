using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class ToggleLightsForSleep
    {
        private static bool IsDroneCameraActive() => uGUI_CameraDrone.main.activeCamera != null;

        private static bool CanToggleLights(Player player, SubRoot sub)
        {
            // Only allow toggling lights if:
            // - Player is in a base (Cyclops has its own toggle)
            // - Base is not in Danger state
            //   - lightingState: 0 = Operational, 1 = Danger (flooding), 2 = Damaged (no power or when lights are manually toggled off)
            // - Base has power (not Offline)
            // - Loading has finished
            // - No menu is open
            // - Not using a Camera Drone (CameraDrone is inactive)
            return player != null
                && sub != null
                && player.IsInBase()
                && sub.powerRelay.GetPowerStatus() != PowerSystem.Status.Offline
                && sub.lightingState != 1
                && !WaitScreen.IsWaiting
                && !Cursor.visible
                && !IsDroneCameraActive();
        }

        [HarmonyPatch(typeof(Bed), nameof(Bed.ResetAnimParams)), HarmonyPostfix]
        public static void Bed_ResetAnimParams()
        {
            if (!Main.Config.ToggleLightsForSleep) return;

            Player player = Player.main;
            if (!CanToggleLights(player, player.currentSub))
                return;

            if (player.currentSub.subLightsOn)
            {
                player.currentSub.subLightsOn = false;
                FMODUWE.PlayOneShot(new FMODAsset() { id = "2102", path = "event:/sub/cyclops/lights_off", name = "95b877e8-2ccd-451d-ab5f-fc654feab173", hideFlags = HideFlags.None }, MainCamera.camera.transform.position, 1f);
            }
        }

        [HarmonyPatch(typeof(Bed), nameof(Bed.OnCinematicEnd)), HarmonyPostfix]
        public static void Bed_OnCinematicEnd()
        {
            if (!Main.Config.ToggleLightsForSleep) return;

            Player player = Player.main;
            if (!CanToggleLights(player, player.currentSub))
                return;

            Player.main.StartCoroutine(TurnLightsOnAfterDelay());
        }

        private static IEnumerator TurnLightsOnAfterDelay()
        {
            yield return new WaitForSeconds(Main.Config.LightsOnAfterSleepDelay);

            Player player = Player.main;
            if (!CanToggleLights(player, player.currentSub) || player.currentSub.subLightsOn)
                yield break;

            player.currentSub.subLightsOn = true;
            FMODUWE.PlayOneShot(new FMODAsset() { id = "2103", path = "event:/sub/cyclops/lights_on", name = "5384ec29-f493-4ac1-9f74-2c0b14d61440", hideFlags = HideFlags.None }, MainCamera.camera.transform.position, 1f);
        }
    }
}