using UnityEngine;
using System.Collections.Generic;

namespace Ungeziefi.Better_Scanner_Blips_Remake
{
    public partial class BetterScannerBlipsRemake
    {
        private static readonly Dictionary<int, Vector2> BlipScreenPositionCache = new Dictionary<int, Vector2>();
        private static Camera mainCamera;
        private static float screenWidth;
        private static float screenHeight;

        private static void SetupEdgeBlipSystem()
        {
            mainCamera = MainCamera.camera;
            UpdateScreenDimensions();
        }

        private static void UpdateScreenDimensions()
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;
        }

        private static bool ShouldShowAsEdgeBlip(Vector3 resourcePosition, out Vector2 screenPos, TechType techType)
        {
            screenPos = Vector2.zero;

            if (mainCamera == null)
            {
                mainCamera = MainCamera.camera;
                if (mainCamera == null) return false;
            }

            // Convert world position to viewport position
            Vector3 viewportPoint = mainCamera.WorldToViewportPoint(resourcePosition);

            // Resources behind camera
            bool isBehindCamera = viewportPoint.z < 0;
            if (isBehindCamera)
            {
                viewportPoint = new Vector3(1f - viewportPoint.x, 1f - viewportPoint.y, 0f);
            }

            // Check if point is outside the viewport
            bool isOutside = viewportPoint.x < 0 || viewportPoint.x > 1f ||
                             viewportPoint.y < 0 || viewportPoint.y > 1f || isBehindCamera;

            if (isOutside && Main.Config.ShowEdgeBlips)
            {
                // Calculate edge position
                screenPos = Main.Config.UseCircularEdgeBlips ?
                    CalculateCircularEdgePosition(viewportPoint) :
                    CalculateRectangularEdgePosition(viewportPoint);
                return true;
            }

            // For points inside the viewport, just use their position
            screenPos = new Vector2(viewportPoint.x, viewportPoint.y);
            return false;
        }

        private static Vector2 CalculateRectangularEdgePosition(Vector3 viewportPoint)
        {
            // Calculate direction from center to the point
#pragma warning disable Harmony003 // Harmony non-ref patch parameters modified
            Vector2 direction = new Vector2(viewportPoint.x - 0.5f, viewportPoint.y - 0.5f);
#pragma warning restore Harmony003 // Harmony non-ref patch parameters modified

            // Zero direction case
            if (direction.sqrMagnitude < 0.0001f)
                return new Vector2(0.5f, 0.5f);

            direction.Normalize();

            // Intersection with screen edge
            float x, y;
            if (Mathf.Abs(direction.x) * screenHeight > Mathf.Abs(direction.y) * screenWidth)
            {
                // Intersect with left or right edge
                x = Mathf.Sign(direction.x) * 0.5f;
                y = direction.y / direction.x * x;
            }
            else
            {
                // Intersect with top or bottom edge
                y = Mathf.Sign(direction.y) * 0.5f;
                x = direction.x / direction.y * y;
            }

            // Margin
            float marginX = Main.Config.EdgeMargin / screenWidth;
            float marginY = Main.Config.EdgeMargin / screenHeight;

            return new Vector2(
                Mathf.Clamp(x + 0.5f, marginX, 1f - marginX),
                Mathf.Clamp(y + 0.5f, marginY, 1f - marginY)
            );
        }

        private static Vector2 CalculateCircularEdgePosition(Vector3 viewportPoint)
        {
            // Calculate direction from center to the point
#pragma warning disable Harmony003 // Harmony non-ref patch parameters modified
            Vector2 direction = new Vector2(viewportPoint.x - 0.5f, viewportPoint.y - 0.5f);
#pragma warning restore Harmony003 // Harmony non-ref patch parameters modified

            // Zero direction case
            if (direction.sqrMagnitude < 0.0001f)
                return new Vector2(0.5f, 0.5f);

            direction.Normalize();

            // Radius with margin
            float minScreenDim = Mathf.Min(screenWidth, screenHeight);
            float maxRadius = 0.5f - (Main.Config.EdgeMargin / minScreenDim);
            float radius = Mathf.Min(Main.Config.CircleRadius / 100f, maxRadius);

            // Place on circle in direction of resource
            return new Vector2(
                0.5f + direction.x * radius,
                0.5f + direction.y * radius
            );
        }

        private static void UpdateBlipPosition(uGUI_ResourceTracker.Blip blip, int index, Vector3 resourcePosition, TechType techType)
        {
            if (ShouldShowAsEdgeBlip(resourcePosition, out Vector2 screenPos, techType))
            {
                // Position blip at edge
                blip.rect.anchorMin = screenPos;
                blip.rect.anchorMax = screenPos;
                blip.rect.anchoredPosition = Vector2.zero;
                BlipScreenPositionCache[index] = screenPos;
            }
            else if (BlipScreenPositionCache.Remove(index))
            {
                // Removed from cache
            }
        }

        private static void CollectAndFilterResourcesWithEdgeBlips(HashSet<ResourceTrackerDatabase.ResourceInfo> nodes)
        {
            var cameraPosition = MainCamera.camera.transform.position;
            UpdateScreenDimensions();

            // Pre-allocate capacity
            ResourcesWithDistances.Clear();
            ResourcesWithDistances.Capacity = Mathf.Max(ResourcesWithDistances.Capacity, nodes.Count);

            // Process all resources
            int blipIndex = 0;
            foreach (var resource in nodes)
            {
                Vector3 dirToResource = resource.position - cameraPosition;
                float distance = Mathf.Sqrt(dirToResource.sqrMagnitude);
                ResourcesWithDistances.Add((resource, distance, blipIndex));
                blipIndex++;
            }

            // Sort by distance
            if (ResourcesWithDistances.Count > 1)
            {
                ResourcesWithDistances.Sort((a, b) => a.distance.CompareTo(b.distance));
            }
        }

        private static void ModifyBlipProcessing(uGUI_ResourceTracker.Blip blip, ResourceTrackerDatabase.ResourceInfo resource, int index, float distance)
        {
            if (Main.Config.ShowEdgeBlips)
            {
                UpdateBlipPosition(blip, index, resource.position, resource.techType);
            }
        }
    }
}