using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class ToggleBaseLights
    {
        private static bool hasToggled = false; // Avoids spamming toggles while holding the button
        private static bool IsDroneCameraActive() => uGUI_CameraDrone.main.activeCamera != null;

        private static bool CanToggleLights(SubRoot sub)
        {
            bool isPausedOrLoading = WaitScreen.IsWaiting
                || Cursor.visible
                || UWE.FreezeTime.HasFreezers()
                || Player.main.GetPDA().isOpen;

            Player player = Player.main;

            if (player == null
                || sub == null
                || sub.powerRelay == null
                || sub.powerRelay.GetPowerStatus() == PowerSystem.Status.Offline // - Base is Offline
                || !player.IsInBase() // Cyclops has its own toggle, exclude it
                || player.currentSub != sub
                || sub.lightingState == 1 // - Base is not in Danger state
                                          //   - lightingState: 0 = Operational, 1 = Danger (flooding), 2 = Damaged (no power or when lights are manually toggled off)
                || isPausedOrLoading
                || IsDroneCameraActive() // - Not using a Camera Drone (CameraDrone is inactive)
                || Inventory.main.GetHeldTool() != null) // Not holding any tool
                return false;

            return true;
        }

        [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.Update)), HarmonyPostfix]
        public static void SubRoot_Update(SubRoot __instance)
        {
            if (!Main.Config.ToggleBaseLights) return;

            if (!CanToggleLights(__instance)) return;

            bool isHoldingButton = GameInput.GetButtonHeld(Main.ToggleBaseLightsButton);
            float holdTime = GameInput.GetButtonHeldTime(Main.ToggleBaseLightsButton);

            if (isHoldingButton && holdTime > Main.Config.ToggleHoldDuration && !hasToggled)
            {
                __instance.subLightsOn = !__instance.subLightsOn;

                // Sound
                // The least necessary ternary operator ever
                string soundPath = __instance.subLightsOn ? "event:/sub/cyclops/lights_on" : "event:/sub/cyclops/lights_off";
                string soundId = __instance.subLightsOn ? "2103" : "2102";
                string soundName = __instance.subLightsOn ? "5384ec29-f493-4ac1-9f74-2c0b14d61440" : "95b877e8-2ccd-451d-ab5f-fc654feab173";
                FMODUWE.PlayOneShot(new FMODAsset() { id = soundId, path = soundPath, name = soundName, hideFlags = HideFlags.None }, MainCamera.camera.transform.position, 1f);

                hasToggled = true;
            }
            else if (!isHoldingButton)
            {
                hasToggled = false;
            }
        }

        [HarmonyPatch(typeof(GUIHand), nameof(GUIHand.OnUpdate)), HarmonyPostfix]
        public static void GUIHand_OnUpdate()
        {
            if (!Main.Config.ToggleBaseLights) return;

            SubRoot currentSub = Player.main.currentSub;
            if (!CanToggleLights(currentSub)) return;

            HandReticle.main.SetText(HandReticle.TextType.Use, $"Hold {GameInput.FormatButton(Main.ToggleBaseLightsButton)} to toggle base lights", false);
        }
    }
}