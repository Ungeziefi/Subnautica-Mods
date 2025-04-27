using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.Better_Scanner_Blips_Remake
{
    [HarmonyPatch]
    public partial class BetterScannerBlipsRemake
    {
        [HarmonyPatch(typeof(Player), nameof(Player.Update)), HarmonyPostfix]
        private static void Player_Update()
        {
            if (!Main.Config.EnableFeature) return;

            if (Input.GetKeyDown(Main.Config.ToggleBlipsKey))
            {
                blipsEnabled = !blipsEnabled;
            }

            // Check if color settings changed and update cache if necessary
            if (ColorManagement.ColorSettingsChanged())
            {
                ColorManagement.UpdateColorCache();
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
            if (!ColorManagement.colorsInitialized)
            {
                ColorManagement.UpdateColorCache();
            }

            var cameraTransform = MainCamera.camera.transform;

            // Create a list of resources with their distances
            var resourcesWithDistances = new List<(ResourceTrackerDatabase.ResourceInfo resource, float distance, int blipIndex)>();

            int blipIndex = 0;
            foreach (var resource in ___nodes)
            {
                var dirToResource = resource.position - cameraTransform.position;

                // Only process resources in front of the camera
                if (Vector3.Dot(dirToResource, cameraTransform.forward) <= 0f) continue;

                float distance = Vector3.Distance(resource.position, cameraTransform.position);
                resourcesWithDistances.Add((resource, distance, blipIndex));
                blipIndex++;
            }

            // Sort resources by distance (closest first)
            resourcesWithDistances.Sort((a, b) => a.distance.CompareTo(b.distance));

            var visibleResources = resourcesWithDistances;

            // Create a dictionary to store group counts (key: blipIndex, value: count)
            Dictionary<int, int> groupCounts = new Dictionary<int, int>();

            // Group resources if enabled
            if (Main.Config.GroupNearbyResources && resourcesWithDistances.Count > 0)
            {
                var groupedResources = GroupResources(resourcesWithDistances);

                // Store each group's count in our dictionary for easy lookup
                foreach (var group in groupedResources)
                {
                    groupCounts[group.blipIndex] = group.count;
                }

                // The GroupResources method now handles both grouping and individual resources
                // when the player is within the breaking distance
                visibleResources = groupedResources.Select(g =>
                    (resource: g.primaryResource, g.distance, g.blipIndex))
                    .ToList();
            }

            // Apply blip count limit if enabled
            if (Main.Config.LimitVisibleBlips)
            {
                int maxCount = Main.Config.MaximumVisibleBlips;
                visibleResources = visibleResources.Take(maxCount).ToList();
            }

            // Keep track of which indices we've processed
            var processedBlipIndices = new HashSet<int>();

            // Process each visible resource
            foreach (var (resource, distance, index) in visibleResources)
            {
                processedBlipIndices.Add(index);
                var blip = ___blips[index];

                // Check if blip should be hidden by manual toggle or auto-hide
                bool shouldHideBlipManually = !blipsEnabled && (!Main.Config.RangeBasedToggle || distance <= Main.Config.ToggleRange);

                // Auto-hide nearby blips if enabled and not using range-based toggle
                bool shouldAutoHide = Main.Config.AutoHideNearbyBlips && !Main.Config.RangeBasedToggle &&
                                     distance <= Main.Config.AutoHideDistance;

                // Hide blips when player is inside a habitat if that setting is enabled
                bool shouldHideInHabitat = Main.Config.HideBlipsInsideHabitats && Player.main.IsInBase();

                if (shouldHideBlipManually || shouldAutoHide || shouldHideInHabitat)
                {
                    blip.text.SetAlpha(0f);
                    blip.gameObject.GetComponent<CanvasRenderer>().SetAlpha(0f);
                    continue;
                }

                // Apply cached blip color
                Graphic graphic = blip.gameObject.GetComponent<Graphic>();
                if (graphic != null && graphic.material != null)
                {
                    graphic.material.SetColor("_Color", ColorManagement.GetCachedBlipColor());
                }

                // Apply cached text color
                if (blip.text != null)
                {
                    blip.text.color = ColorManagement.GetCachedTextColor();
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
                    continue;
                }

                // Update nearby resource display
                renderer.SetAlpha(1f);

                string resourceName = Language.main.Get(resource.techType.AsString(false));
                string distanceText = $"{Mathf.RoundToInt(distance)}{Language.main.Get("MeterSuffix")}";

                // Get the group count from dictionary, or default to 1 if not found
                int groupCount = 1;
                if (Main.Config.GroupNearbyResources && groupCounts.TryGetValue(index, out int count))
                {
                    groupCount = count;
                }

                // Visibility options
                if (Main.Config.TextVisibility == "Hide both" || (Main.Config.LimitTextVisibilityByDistance && distance > Main.Config.TextVisibilityDistance))
                {
                    blip.text.SetAlpha(0f);
                }
                else
                {
                    // Set text and keep it visible
                    blip.text.SetAlpha(1f);

                    // Build the display text based on settings and group count
                    string displayText;
                    string resourceLabel = resourceName;

                    // Add group count to resource name if grouping is enabled and count > 1
                    if (groupCount > 1)
                    {
                        resourceLabel = $"{resourceName} x{groupCount}";
                    }

                    switch (Main.Config.TextVisibility)
                    {
                        case "Hide resource name":
                            displayText = distanceText;
                            break;
                        case "Hide distance":
                            displayText = resourceLabel;
                            break;
                        default: // Default
                            displayText = $"{resourceLabel}\r\n{distanceText}";
                            break;
                    }

                    blip.text.SetText(displayText, true);
                }
            }

            // Hide unprocessed blips
            int blipCount = ___blips.Count;
            for (int i = 0; i < blipCount; i++)
            {
                if (!processedBlipIndices.Contains(i))
                {
                    var blip = ___blips[i];
                    blip.text.SetAlpha(0f);
                    blip.gameObject.GetComponent<CanvasRenderer>().SetAlpha(0f);
                }
            }
        }
    }
}