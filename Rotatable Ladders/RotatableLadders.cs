﻿using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Rotatable_Ladders
{
    [HarmonyPatch]
    public class RotatableLadders
    {
        public static string GetLadderCoords(Transform transform) =>
            $"{Mathf.RoundToInt(transform.position.x)},{Mathf.RoundToInt(transform.position.y)},{Mathf.RoundToInt(transform.position.z)}";

        [HarmonyPatch(typeof(BaseLadder), nameof(BaseLadder.OnHandHover)), HarmonyPostfix]
        public static void BaseLadder_OnHandHover(BaseLadder __instance, GUIHand hand)
        {
            if (!Main.Config.EnableFeature || __instance == null || !__instance.enabled) return;

            var primaryDevice = GameInput.GetPrimaryDevice();
            var rotateText = primaryDevice == GameInput.Device.Controller ? "Press to rotate" : $"Press {Main.Config.RotateLadderKey} to rotate";

            HandReticle.main.SetText(type: HandReticle.TextType.HandSubscript,
                text: rotateText,
                translate: false,
                button: primaryDevice == GameInput.Device.Controller ? GameInput.Button.AltTool : GameInput.Button.None);

            // Calculate and apply rotation
            if (Input.GetKeyDown(Main.Config.RotateLadderKey) ||
                ((GameInput.GetPrimaryDevice() == GameInput.Device.Controller) && GameInput.GetButtonDown(GameInput.Button.AltTool)))
            {
                Transform parent = __instance.transform.parent;
                if (parent == null) return;

                string coords = GetLadderCoords(parent);
                float newYRotation = (parent.localRotation.eulerAngles.y + 90f) % 360f;
                parent.localRotation = Quaternion.Euler(0f, newYRotation, 0f);

                // Only save if rotated
                if (newYRotation == 0f)
                    Main.SaveData.RotatedLadders.Remove(coords);
                else
                    Main.SaveData.RotatedLadders[coords] = newYRotation;
            }
        }

        [HarmonyPatch(typeof(BaseLadder), nameof(BaseLadder.Start)), HarmonyPostfix]
        public static void BaseLadder_Start(BaseLadder __instance)
        {
            Transform parent = __instance.transform.parent;
            if (!Main.Config.EnableFeature || __instance == null || parent == null) return;

            // Load saved rotation
            string coords = GetLadderCoords(parent);
            if (Main.SaveData.RotatedLadders.TryGetValue(coords, out float savedRotation))
                parent.localRotation = Quaternion.Euler(0f, savedRotation, 0f);
        }
    }
}