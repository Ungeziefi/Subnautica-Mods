﻿using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch(typeof(BaseLadder))]
    public class RotatableLadders
    {
        public static string GetLadderCoords(Transform transform)
        {
            var pos = new Vector3Int(
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y),
                Mathf.RoundToInt(transform.position.z)
            );
            return $"{pos.x},{pos.y},{pos.z}";
        }

        [HarmonyPatch(nameof(BaseLadder.OnHandHover)), HarmonyPostfix]
        public static void OnHandHover(BaseLadder __instance, GUIHand hand)
        {
            if (__instance == null || !__instance.enabled)
            {
                return;
            }

            HandReticle.main.SetText(HandReticle.TextType.HandSubscript, $"Press {Main.Config.RotateLadderKey} to rotate", false, GameInput.Button.None);

            if (Input.GetKeyDown(Main.Config.RotateLadderKey))
            {
                Transform parent = __instance.transform.parent;
                if (parent == null)
                {
                    return;
                }

                string coords = GetLadderCoords(parent);

                // Calculate and apply new rotation
                float newYRotation = (parent.localRotation.eulerAngles.y + 90f) % 360f;
                parent.localRotation = Quaternion.Euler(0f, newYRotation, 0f);

                // If rotation is back to default (0), remove the saved data
                if (newYRotation == 0f)
                {
                    if (Main.SaveData.RotatedLadders.ContainsKey(coords))
                    {
                        Main.SaveData.RotatedLadders.Remove(coords);
                        // Main.Logger.LogInfo($"Removed rotation data for ladder at {coords}");
                    }
                }
                else
                {
                    // Otherwise save the new rotation
                    Main.SaveData.RotatedLadders[coords] = newYRotation;
                }
            }
        }

        [HarmonyPatch(nameof(BaseLadder.Start)), HarmonyPostfix]
        public static void Start(BaseLadder __instance)
        {
            Transform parent = __instance.transform.parent;

            if (!Main.Config.RotatableLadders || __instance == null || parent == null)
            {
                return;
            }

            string coords = GetLadderCoords(parent);

            if (Main.SaveData.RotatedLadders.TryGetValue(coords, out float savedRotation))
            {
                parent.localRotation = Quaternion.Euler(0f, savedRotation, 0f);
            }
        }
    }
}