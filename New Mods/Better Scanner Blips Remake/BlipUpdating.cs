using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.Better_Scanner_Blips_Remake
{
    public partial class BetterScannerBlipsRemake
    {
        private static void UpdateBlip(uGUI_ResourceTracker.Blip blip, ResourceTrackerDatabase.ResourceInfo resource, float distance, Camera camera, int count)
        {
            blip.gameObject.SetActive(true);

            // Use or create cache for components
            if (!blipComponents.TryGetValue(blip.gameObject, out var components))
            {
                var graphic = blip.gameObject.GetComponent<Graphic>();
                var renderer = blip.gameObject.GetComponent<CanvasRenderer>();
                components = (graphic, renderer);
                blipComponents[blip.gameObject] = components;
            }

            // Get viewport position
            Vector3 viewportPoint = camera.WorldToViewportPoint(resource.position);
            Vector2 screenPos;

            // Handle edge blips
            if (Main.Config.ShowEdgeBlips)
            {
                bool isBehindCamera = viewportPoint.z < 0;
                bool isOutside = viewportPoint.x < 0 || viewportPoint.x > 1f ||
                                viewportPoint.y < 0 || viewportPoint.y > 1f || isBehindCamera;

                if (isOutside)
                {
                    screenPos = CalculateEdgePosition(viewportPoint, isBehindCamera);
                }
                else
                {
                    screenPos = new Vector2(viewportPoint.x, viewportPoint.y);
                }
            }
            else
            {
                screenPos = new Vector2(viewportPoint.x, viewportPoint.y);
            }

            // Update position
            blip.rect.anchorMin = screenPos;
            blip.rect.anchorMax = screenPos;

            // Update scale based on distance
            float scale = distance >= Main.Config.MaximumRange ?
                Main.Config.MinimumScale :
                (-1f + Main.Config.MinimumScale) * (distance / Main.Config.MaximumRange) + 1f;
            blip.rect.localScale = new Vector3(scale, scale, scale);

            // Apply colors
            if (blip.text != null)
            {
                blip.text.color = ColorManagement.GetCachedTextColor();
            }

            if (components.graphic != null && components.graphic.material != null)
            {
                components.graphic.material.SetColor("_Color", ColorManagement.GetCachedBlipColor());
            }

            // Handle text visibility
            bool showText = Main.Config.TextVisibility != "Hide all" &&
                          (!Main.Config.LimitTextVisibilityByDistance || distance <= Main.Config.TextVisibilityDistance);

            if (showText)
            {
                string resourceName = Language.main.Get(resource.techType.AsString(false));
                string distanceText = $"{Mathf.RoundToInt(distance)}{Language.main.Get("MeterSuffix")}";

                // Format the resource name based on visibility settings
                string resourceLabel;

                if (Main.Config.TextVisibility == "Hide count" && count > 1)
                {
                    // Just show the resource name without count
                    resourceLabel = resourceName;
                }
                else if (count > 1)
                {
                    // Show resource name with count
                    resourceLabel = $"{resourceName} x{count}";
                }
                else
                {
                    // Single resource
                    resourceLabel = resourceName;
                }

                // Generate final display text based on visibility settings
                string displayText;
                switch (Main.Config.TextVisibility)
                {
                    case "Hide name":
                        if (count > 1)
                            displayText = $"x{count}\r\n{distanceText}";
                        else
                            displayText = distanceText;
                        break;
                    case "Hide distance":
                        displayText = resourceLabel;
                        break;
                    case "Hide all":
                        displayText = "";
                        break;
                    default: // Default or Hide count (count is already handled in resourceLabel)
                        displayText = $"{resourceLabel}\r\n{distanceText}";
                        break;
                }

                blip.text.SetText(displayText);
                blip.text.SetAlpha(1f);
            }
            else
            {
                blip.text.SetAlpha(0f);
            }

            // Adjust alpha for distant blips
            if (distance >= Main.Config.MaximumRange)
            {
                components.renderer.SetAlpha(Main.Config.DistantAlpha);
            }
            else
            {
                components.renderer.SetAlpha(1f);
            }
        }
    }
}