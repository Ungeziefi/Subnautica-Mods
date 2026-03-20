using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class ToggleBaseLights
    {
        private static bool hasToggled = false; // Avoids spamming toggles while holding the button
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
                && sub.powerRelay != null
                && player.IsInBase()
                && sub.powerRelay.GetPowerStatus() != PowerSystem.Status.Offline
                && sub.lightingState != 1
                && !WaitScreen.IsWaiting
                && !Cursor.visible
                && !IsDroneCameraActive();
        }

        [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.Update)), HarmonyPostfix]
        public static void SubRoot_Update(SubRoot __instance)
        {
            if (!Main.Config.ToggleBaseLights) return;

            Player player = Player.main;
            if (!CanToggleLights(player, __instance) || player.currentSub != __instance)
                return;

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

            Player player = Player.main;
            SubRoot currentSub = player.currentSub;
            bool isHoldingItem = Inventory.main.GetHeldTool() != null;
            if (!CanToggleLights(player, currentSub) || isHoldingItem)
                return;

            HandReticle.main.SetText(HandReticle.TextType.Use, $"Hold {GameInput.FormatButton(Main.ToggleBaseLightsButton)} to toggle base lights", false);
        }
    }
}