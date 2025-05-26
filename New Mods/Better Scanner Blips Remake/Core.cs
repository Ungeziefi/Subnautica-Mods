using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.Better_Scanner_Blips_Remake
{
    [HarmonyPatch]
    public partial class BetterScannerBlipsRemake
    {
        #region Fields
        internal static bool blipsEnabled = true;

        private static readonly Dictionary<int, CanvasRenderer> RendererCache = new Dictionary<int, CanvasRenderer>();
        private static readonly Dictionary<int, Graphic> GraphicCache = new Dictionary<int, Graphic>();

        private static readonly List<(ResourceTrackerDatabase.ResourceInfo resource, float distance, int blipIndex)> ResourcesWithDistances =
            new List<(ResourceTrackerDatabase.ResourceInfo, float, int)>(64);
        private static readonly HashSet<int> ProcessedBlipIndices = new HashSet<int>();
        private static readonly Dictionary<int, int> GroupCounts = new Dictionary<int, int>();
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

            // Clear collections for reuse
            ResourcesWithDistances.Clear();
            ProcessedBlipIndices.Clear();
            GroupCounts.Clear();

            CollectAndFilterResources(___nodes);
            ProcessResources(___blips);
        }
        #endregion

        #region Resource Processing
        private static void CollectAndFilterResources(HashSet<ResourceTrackerDatabase.ResourceInfo> nodes)
        {
            var cameraTransform = MainCamera.camera.transform;
            var cameraPosition = cameraTransform.position;
            var cameraForward = cameraTransform.forward;

            // Calculate resources with distances (only those in front of camera)
            int blipIndex = 0;
            foreach (var resource in nodes)
            {
                var dirToResource = resource.position - cameraPosition;

                // Only process resources in front of the camera
                if (Vector3.Dot(dirToResource, cameraForward) <= 0f)
                {
                    blipIndex++;
                    continue;
                }

                float sqrDistance = (resource.position - cameraPosition).sqrMagnitude;
                float distance = Mathf.Sqrt(sqrDistance);
                ResourcesWithDistances.Add((resource, distance, blipIndex));
                blipIndex++;
            }

            // Sort resources by distance (closest first)
            if (ResourcesWithDistances.Count > 1)
            {
                ResourcesWithDistances.Sort((a, b) => a.distance.CompareTo(b.distance));
            }
        }

        private static void ProcessResources(List<uGUI_ResourceTracker.Blip> blips)
        {
            var visibleResources = ResourcesWithDistances;

            // Group resources if enabled
            if (Main.Config.GroupNearbyResources && ResourcesWithDistances.Count > 0)
            {
                var groupedResources = GroupResources(ResourcesWithDistances);

                // Store each group's count for lookup
                foreach (var group in groupedResources)
                {
                    GroupCounts[group.blipIndex] = group.count;
                }

                visibleResources = groupedResources.Select(g =>
                    (resource: g.primaryResource, g.distance, g.blipIndex))
                    .ToList();
            }

            // Apply blip count limit if enabled
            int resourceCount = visibleResources.Count;
            if (Main.Config.LimitVisibleBlips && resourceCount > Main.Config.MaximumVisibleBlips)
            {
                resourceCount = Main.Config.MaximumVisibleBlips;
            }

            UpdateBlipDisplay(blips, visibleResources, resourceCount);
        }
        #endregion

        #region Display Management
        private static void UpdateBlipDisplay(List<uGUI_ResourceTracker.Blip> blips, List<(ResourceTrackerDatabase.ResourceInfo resource, float distance, int blipIndex)> visibleResources, int resourceCount)
        {
            // Cached colors
            Color cachedBlipColor = ColorManagement.GetCachedBlipColor();
            Color cachedTextColor = ColorManagement.GetCachedTextColor();

            // Process visible resources
            for (int i = 0; i < resourceCount; i++)
            {
                var (resource, distance, index) = visibleResources[i];

                // Validate index
                if (index < 0 || index >= blips.Count)
                {
                    continue; // Skip if index is invalid
                }

                ProcessedBlipIndices.Add(index);
                var blip = blips[index];

                if (ShouldHideBlip(distance))
                {
                    blip.text.SetAlpha(0f);
                    GetOrCreateRenderer(blip.gameObject, index).SetAlpha(0f);
                    continue;
                }

                UpdateBlipAppearance(blip, index, distance, cachedBlipColor, cachedTextColor);
                UpdateBlipText(blip, resource, distance, index);
            }

            // Hide unprocessed blips
            HideUnprocessedBlips(blips);
        }

        private static bool ShouldHideBlip(float distance)
        {
            // Visibility checks
            bool shouldHideBlipManually = !blipsEnabled && (!Main.Config.RangeBasedToggle || distance <= Main.Config.ToggleRange);

            bool shouldAutoHide = Main.Config.AutoHideNearbyBlips &&
                                    !Main.Config.RangeBasedToggle &&
                                    distance <= Main.Config.AutoHideDistance;

            bool shouldHideInHabitat = Main.Config.HideBlipsInsideHabitats &&
                                        Player.main.IsInBase() &&
                                        uGUI_CameraDrone.main != null;

            return shouldHideBlipManually || shouldAutoHide || shouldHideInHabitat;
        }

        private static void UpdateBlipAppearance(uGUI_ResourceTracker.Blip blip, int index, float distance, Color blipColor, Color textColor)
        {
            // Apply cached color - use cached component references
            Graphic graphic = GetOrCreateGraphic(blip.gameObject, index);
            if (graphic != null && graphic.material != null)
            {
                graphic.material.SetColor("_Color", blipColor);
            }

            // Apply cached text color
            if (blip.text != null)
            {
                blip.text.color = textColor;
            }

            // Scale based on distance
            float scale;
            if (distance >= Main.Config.MaximumRange)
            {
                scale = Main.Config.MinimumScale;
            }
            else
            {
                scale = (-1f + Main.Config.MinimumScale) * (distance / Main.Config.MaximumRange) + 1f;
            }
            blip.rect.localScale = new Vector3(scale, scale, scale);

            var renderer = GetOrCreateRenderer(blip.gameObject, index);

            // Distant resources handling
            if (distance >= Main.Config.MaximumRange)
            {
                blip.text.SetAlpha(0f);
                renderer.SetAlpha(Main.Config.DistantAlpha);
                return;
            }

            // Update nearby resource display
            renderer.SetAlpha(1f);

            // Text visibility check
            if (Main.Config.TextVisibility == "Hide both" ||
                (Main.Config.LimitTextVisibilityByDistance && distance > Main.Config.TextVisibilityDistance))
            {
                blip.text.SetAlpha(0f);
                return;
            }

            // Process if visible
            blip.text.SetAlpha(1f);
        }

        private static void UpdateBlipText(uGUI_ResourceTracker.Blip blip, ResourceTrackerDatabase.ResourceInfo resource, float distance, int index)
        {
            // Get resource name and distance text
            string resourceName = Language.main.Get(resource.techType.AsString(false));
            string distanceText = $"{Mathf.RoundToInt(distance)}{Language.main.Get("MeterSuffix")}";

            // Get group count if available
            int groupCount = 1;
            if (Main.Config.GroupNearbyResources && GroupCounts.TryGetValue(index, out int count))
            {
                groupCount = count;
            }

            // Build display text
            string displayText;
            string resourceLabel = (groupCount > 1) ? $"{resourceName} x{groupCount}" : resourceName;

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

        private static void HideUnprocessedBlips(List<uGUI_ResourceTracker.Blip> blips)
        {
            int blipCount = blips.Count;
            for (int i = 0; i < blipCount; i++)
            {
                if (!ProcessedBlipIndices.Contains(i))
                {
                    var blip = blips[i];
                    blip.text.SetAlpha(0f);
                    GetOrCreateRenderer(blip.gameObject, i).SetAlpha(0f);
                }
            }
        }
        #endregion

        #region Helper Methods
        private static CanvasRenderer GetOrCreateRenderer(GameObject obj, int key)
        {
            if (!RendererCache.TryGetValue(key, out var renderer))
            {
                renderer = obj.GetComponent<CanvasRenderer>();
                RendererCache[key] = renderer;
            }
            return renderer;
        }

        private static Graphic GetOrCreateGraphic(GameObject obj, int key)
        {
            if (!GraphicCache.TryGetValue(key, out var graphic))
            {
                graphic = obj.GetComponent<Graphic>();
                GraphicCache[key] = graphic;
            }
            return graphic;
        }
        #endregion
    }
}