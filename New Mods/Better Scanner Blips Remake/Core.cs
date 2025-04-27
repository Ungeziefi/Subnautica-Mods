using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.Better_Scanner_Blips_Remake
{
    [HarmonyPatch]
    public class BetterScannerBlipsRemake
    {
        #region Fields
        private static bool blipsEnabled = true;

        // Original colors
        // Blip: 1.00f, 0.64f, 0.00f, 1.00f
        // Text: 1.00f, 0.68f, 0.00f, 1.00f
        public static Color originalBlipColor = new Color(1.00f, 0.64f, 0.00f, 1.00f);
        public static Color originalTextColor = new Color(1.00f, 0.68f, 0.00f, 1.00f);

        // Cached colors
        private static Color cachedBlipColor;
        private static Color cachedTextColor;

        // Cached settings state to detect changes
        private static bool lastUseCustomBlipColor;
        private static bool lastUseCustomTextColor;
        private static float lastBlipRed, lastBlipGreen, lastBlipBlue;
        private static float lastTextRed, lastTextGreen, lastTextBlue;
        private static bool colorsInitialized = false;
        #endregion

        #region Color Cache Management
        public static void UpdateColorCache()
        {
            Config config = Main.Config;

            // Update cached settings values
            lastUseCustomBlipColor = config.UseCustomBlipColor;
            lastUseCustomTextColor = config.UseCustomTextColor;
            lastBlipRed = config.BlipColorRed;
            lastBlipGreen = config.BlipColorGreen;
            lastBlipBlue = config.BlipColorBlue;
            lastTextRed = config.TextColorRed;
            lastTextGreen = config.TextColorGreen;
            lastTextBlue = config.TextColorBlue;

            // Update cached colors
            cachedBlipColor = config.UseCustomBlipColor ? config.GetBlipColor() : originalBlipColor;
            cachedTextColor = config.UseCustomTextColor ? config.GetTextColor() : originalTextColor;

            colorsInitialized = true;
        }

        public static bool ColorSettingsChanged()
        {
            if (!colorsInitialized) return true;

            Config config = Main.Config;

            return lastUseCustomBlipColor != config.UseCustomBlipColor ||
                   lastUseCustomTextColor != config.UseCustomTextColor ||
                   (config.UseCustomBlipColor && (
                       lastBlipRed != config.BlipColorRed ||
                       lastBlipGreen != config.BlipColorGreen ||
                       lastBlipBlue != config.BlipColorBlue)) ||
                   (config.UseCustomTextColor && (
                       lastTextRed != config.TextColorRed ||
                       lastTextGreen != config.TextColorGreen ||
                       lastTextBlue != config.TextColorBlue));
        }
        #endregion

        #region Harmony Patches
        [HarmonyPatch(typeof(Player), nameof(Player.Update)), HarmonyPostfix]
        private static void Player_Update()
        {
            if (!Main.Config.EnableFeature) return;

            if (Input.GetKeyDown(Main.Config.ToggleBlipsKey))
            {
                blipsEnabled = !blipsEnabled;
            }

            // Check if color settings changed and update cache if necessary
            if (ColorSettingsChanged())
            {
                UpdateColorCache();
            }
        }

        [HarmonyPatch(typeof(uGUI_ResourceTracker), nameof(uGUI_ResourceTracker.UpdateBlips)), HarmonyPostfix]
        private static void uGUI_ResourceTracker_UpdateBlips(
            HashSet<ResourceTrackerDatabase.ResourceInfo> ___nodes,
            List<uGUI_ResourceTracker.Blip> ___blips,
            bool ___visible)
        {
            if (!Main.Config.EnableFeature || !___visible) return;

            // Initialize colors if not already done
            if (!colorsInitialized)
            {
                UpdateColorCache();
            }

            var cameraTransform = MainCamera.camera.transform;
            int blipIndex = 0;

            foreach (var resource in ___nodes)
            {
                var dirToResource = resource.position - cameraTransform.position;

                // Only process resources in front of the camera
                if (Vector3.Dot(dirToResource, cameraTransform.forward) <= 0f) continue;

                var blip = ___blips[blipIndex];
                var distance = Vector3.Distance(resource.position, cameraTransform.position);

                // Check if blip should be hidden by manual toggle or auto-hide
                bool shouldHideBlipManually = !blipsEnabled && (!Main.Config.RangeBasedToggle || distance <= Main.Config.ToggleRange);

                // Auto-hide nearby blips if enabled and not using range-based toggle
                bool shouldAutoHide = Main.Config.AutoHideNearbyBlips && !Main.Config.RangeBasedToggle &&
                                     distance <= Main.Config.AutoHideDistance;

                if (shouldHideBlipManually || shouldAutoHide)
                {
                    blip.text.SetAlpha(0f);
                    blip.gameObject.GetComponent<CanvasRenderer>().SetAlpha(0f);
                    blipIndex++;
                    continue;
                }

                // Apply cached blip color
                Graphic graphic = blip.gameObject.GetComponent<Graphic>();
                if (graphic != null && graphic.material != null)
                {
                    graphic.material.SetColor("_Color", cachedBlipColor);
                }

                // Apply cached text color
                if (blip.text != null)
                {
                    blip.text.color = cachedTextColor;
                }

                // Scale based on distance
                var scale = (-1f + Main.Config.MinimumScale) *
                    (Math.Min(distance, Main.Config.MaximumRange) / Main.Config.MaximumRange) + 1f;
                blip.rect.localScale = Vector3.one * scale;

                var renderer = blip.gameObject.GetComponent<CanvasRenderer>();

                // Distant resources
                if (distance >= Main.Config.MaximumRange)
                {
                    blip.text.SetAlpha(0f);
                    renderer.SetAlpha(Main.Config.DistantAlpha);
                    blipIndex++;
                    continue;
                }

                // Update nearby resource display
                renderer.SetAlpha(1f);
                blip.text.SetAlpha(1f);

                string resourceName = Language.main.Get(resource.techType.AsString(false));
                string distanceText = $"{Mathf.RoundToInt(distance)}{Language.main.Get("MeterSuffix")}";

                // Visibility options
                if (Main.Config.TextVisibility == "Hide both")
                {
                    blip.text.SetAlpha(0f);
                }
                else
                {
                    // Set text and keep it visible
                    blip.text.SetAlpha(1f);

                    string displayText = Main.Config.TextVisibility switch
                    {
                        "Hide resource name" => distanceText,
                        "Hide distance" => resourceName,
                        _ => $"{resourceName}\r\n{distanceText}" // Default
                    };

                    blip.text.SetText(displayText, true);
                }

                blipIndex++;
            }
        }
        #endregion
    }
}