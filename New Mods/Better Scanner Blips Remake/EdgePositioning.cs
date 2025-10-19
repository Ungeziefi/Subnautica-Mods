using System.Collections.Generic;
using UnityEngine;

namespace Ungeziefi.Better_Scanner_Blips_Remake
{
    public partial class BetterScannerBlipsRemake
    {
        private static readonly Dictionary<string, Vector2> lastPositions = new();
        private static readonly Dictionary<string, Vector2> velocities = new();

        private static Vector2 CalculateEdgePosition(Vector3 viewportPoint, bool isBehindCamera = false, string resourceId = "")
        {
            // Get camera orientation vectors
            var camera = MainCamera.camera;
            Vector3 camRight = camera.transform.right;
            Vector3 camUp = camera.transform.up;

            // Convert to world position
#pragma warning disable Harmony003 // Harmony non-ref patch parameters modified
            Vector3 worldPos = camera.ViewportToWorldPoint(
                new Vector3(viewportPoint.x, viewportPoint.y, 10f));
#pragma warning restore Harmony003 // Harmony non-ref patch parameters modified

            // Prevent jittering
            worldPos.x = Mathf.Round(worldPos.x);
            worldPos.y = Mathf.Round(worldPos.y);
            worldPos.z = Mathf.Round(worldPos.z);

            if (isBehindCamera)
            {
                // Invert direction for blips behind the camera
                worldPos = camera.transform.position - (worldPos - camera.transform.position);
            }

            // Get direction in camera space
            Vector3 dirToResource = worldPos - camera.transform.position;
            dirToResource.Normalize();

            // Project onto camera plane
            float rightDot = Vector3.Dot(dirToResource, camRight);
            float upDot = Vector3.Dot(dirToResource, camUp);

            // Create 2D direction
            Vector2 direction = new(rightDot, upDot);

            // Handle zero direction
            if (direction.sqrMagnitude < 0.001f)
            {
                direction = new Vector2(0, 1); // Default up
            }
            else
            {
                direction.Normalize();
            }

            // Calculate target position (without smoothing)
            Vector2 targetPosition;

            if (Main.Config.UseCircularEdgeBlips)
            {
                // Place blips in a circle around screen center
                float minScreenDim = Mathf.Min(Screen.width, Screen.height);
                float maxRadius = 0.5f - (Main.Config.EdgeMargin / minScreenDim);
                float radius = Mathf.Min(Main.Config.CircleRadius / 100f, maxRadius);

                targetPosition = new Vector2(
                    0.5f + direction.x * radius,
                    0.5f + direction.y * radius
                );
            }
            else
            {
                // Place blips at screen edges
                float x, y;
                // Determine which edge to use based on direction and screen aspect ratio
                if (Mathf.Abs(direction.x) * Screen.height > Mathf.Abs(direction.y) * Screen.width)
                {
                    // Hit left/right edge
                    x = Mathf.Sign(direction.x) * 0.5f;
                    y = direction.y / direction.x * x;
                }
                else
                {
                    // Hit top/bottom edge
                    y = Mathf.Sign(direction.y) * 0.5f;
                    x = direction.x / direction.y * y;
                }

                float marginX = Main.Config.EdgeMargin / Screen.width;
                float marginY = Main.Config.EdgeMargin / Screen.height;

                targetPosition = new Vector2(
                    Mathf.Clamp(x + 0.5f, marginX, 1f - marginX),
                    Mathf.Clamp(y + 0.5f, marginY, 1f - marginY)
                );
            }

            #region Smoothing
            if (!string.IsNullOrEmpty(resourceId))
            {
                // Init velocity
                if (!velocities.TryGetValue(resourceId, out var currentVelocity))
                {
                    currentVelocity = velocities[resourceId] = Vector2.zero;
                }

                // Apply smoothing
                if (lastPositions.TryGetValue(resourceId, out Vector2 prevPos))
                {
                    targetPosition = Vector2.SmoothDamp(
                        prevPos,
                        targetPosition,
                        ref currentVelocity,
                        Main.Config.SmoothingTime,
                        Mathf.Infinity,
                        Time.deltaTime);
                }

                // Update stored values for next frame
                velocities[resourceId] = currentVelocity;
                return lastPositions[resourceId] = targetPosition;
            }
            #endregion

            return targetPosition;
        }
    }
}