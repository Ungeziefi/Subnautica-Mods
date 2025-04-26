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
        private static Color originalBlipColor = new Color(1.00f, 0.64f, 0.00f, 1.00f);
        private static Color originalTextColor = new Color(1.00f, 0.68f, 0.00f, 1.00f);
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
        }

        [HarmonyPatch(typeof(uGUI_ResourceTracker), nameof(uGUI_ResourceTracker.UpdateBlips)), HarmonyPostfix]
        private static void uGUI_ResourceTracker_UpdateBlips(
            HashSet<ResourceTrackerDatabase.ResourceInfo> ___nodes,
            List<uGUI_ResourceTracker.Blip> ___blips,
            bool ___visible)
        {
            if (!Main.Config.EnableFeature || !___visible) return;

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

                // Apply or restore blip color
                Graphic graphic = blip.gameObject.GetComponent<Graphic>();
                if (graphic != null && graphic.material != null)
                {
                    if (Main.Config.UseCustomBlipColor)
                    {
                        graphic.material.SetColor("_Color", Main.Config.GetBlipColor());
                    }
                    else
                    {
                        graphic.material.SetColor("_Color", originalBlipColor);
                    }
                }

                // Apply or restore text color
                if (blip.text != null)
                {
                    if (Main.Config.UseCustomTextColor)
                    {
                        blip.text.color = Main.Config.GetTextColor();
                    }
                    else
                    {
                        blip.text.color = originalTextColor;
                    }
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

                // Handle different visibility options
                switch (Main.Config.TextVisibility)
                {
                    case "Hide resource name":
                        blip.text.SetText(distanceText, true);
                        break;

                    case "Hide distance":
                        blip.text.SetText(resourceName, true);
                        break;

                    case "Hide both":
                        blip.text.SetAlpha(0f);
                        break;

                    default:
                        blip.text.SetText($"{resourceName}\r\n{distanceText}", true);
                        break;
                }

                blipIndex++;
            }
        }
        #endregion
    }
}