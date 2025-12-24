using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.Better_Scanner_Blips_Remake
{
    public partial class BetterScannerBlipsRemake
    {
        private static readonly StringBuilder stringBuilder = new(128);

        private static void UpdateBlip(uGUI_ResourceTracker.Blip blip, ResourceTrackerDatabase.ResourceInfo resource, float distance, Camera camera, int count)
        {
            bool isFragment = resource.techType == TechType.Fragment;
            bool isKnownFragment = false;

            // Always start enabled to ignore the dot product check in uGUI_ResourceTracker.UpdateBlips
            blip.gameObject.SetActive(true);

            // Disable known fragments if configured
            if (isFragment && (Main.Config.HideKnownFragmentBlips || Main.Config.AppendKnown))
            {
                isKnownFragment = KnownFragmentFilter.IsKnownFragment(resource.uniqueId);
            }
            if (Main.Config.HideKnownFragmentBlips && isFragment && isKnownFragment)
            {
                blip.gameObject.SetActive(false);
                return;
            }

            // Use or create cache for components
            if (!blipComponents.TryGetValue(blip.gameObject, out var components))
            {
                components = (
                    blip.gameObject.GetComponent<Graphic>(),
                    blip.gameObject.GetComponent<CanvasRenderer>()
                );
                blipComponents[blip.gameObject] = components;
            }

            // Calculate screen position
            Vector3 viewportPoint = camera.WorldToViewportPoint(resource.position);
            Vector2 screenPos = CalculateScreenPosition(viewportPoint, resource);

            // Check if outside viewport
            if (float.IsNaN(screenPos.x) && float.IsNaN(screenPos.y))
            {
                blip.gameObject.SetActive(false);
                return;
            }

            // Update position and scale
            blip.rect.anchorMin = blip.rect.anchorMax = screenPos;
            float scale = CalculateScale(distance);
            blip.rect.localScale = new Vector3(scale, scale, scale);

            ApplyColors(blip, components);

            // Text visibility
            bool textVisible = Main.Config.TextVisibility != "Hide all" &&
                (!Main.Config.LimitTextVisibilityByDistance || distance <= Main.Config.TextVisibilityDistance);

            if (textVisible)
            {
                UpdateBlipText(blip, resource, distance, count, isFragment, isKnownFragment);
            }
            else if (blip.text.color.a > 0)
            {
                blip.text.SetAlpha(0f);
            }

            // Adjust alpha for distant blips
            components.renderer.SetAlpha(distance >= Main.Config.MaximumRange ?
                Main.Config.DistantAlpha : 1f);
        }

        private static Vector2 CalculateScreenPosition(Vector3 viewportPoint, ResourceTrackerDatabase.ResourceInfo resource = null)
        {
#pragma warning disable Harmony003 // Harmony non-ref patch parameters modified
            bool isBehindCamera = viewportPoint.z < 0;

            // Set NaN for blips outside viewport when edge blips are disabled
            if (!Main.Config.ShowEdgeBlips && isBehindCamera)
            {
                return new Vector2(float.NaN, float.NaN);
            }

            if (!Main.Config.ShowEdgeBlips)
            {
                return new Vector2(viewportPoint.x, viewportPoint.y);
            }

            bool isOutside = viewportPoint.x < 0 || viewportPoint.x > 1f ||
                            viewportPoint.y < 0 || viewportPoint.y > 1f || isBehindCamera;

            return isOutside ? CalculateEdgePosition(viewportPoint, isBehindCamera, resource.uniqueId)
                              : new Vector2(viewportPoint.x, viewportPoint.y);
        }

        private static float CalculateScale(float distance)
        {
            if (distance >= Main.Config.MaximumRange)
                return Main.Config.MinimumScale;

            return (-1f + Main.Config.MinimumScale) * (distance / Main.Config.MaximumRange) + 1f;
        }

        private static void ApplyColors(uGUI_ResourceTracker.Blip blip, (Graphic graphic, CanvasRenderer renderer) components)
        {
            if (blip.text != null)
                blip.text.color = ColorManagement.GetCachedTextColor();

            if (components.graphic.material != null)
                components.graphic.material.SetColor("_Color", ColorManagement.GetCachedBlipColor());
#pragma warning restore Harmony003 // Harmony non-ref patch parameters modified
        }

        private static void UpdateBlipText(uGUI_ResourceTracker.Blip blip, ResourceTrackerDatabase.ResourceInfo resource,
                                          float distance, int count, bool isFragment, bool isKnownFragment)
        {
            string resourceName = GetResourceName(resource, count, isFragment, isKnownFragment);
            string distanceText = $"{Mathf.RoundToInt(distance)}{Language.main.Get("MeterSuffix")}";
            string displayText = FormatText(resourceName, distanceText, count);

            blip.text.SetText(displayText);
            blip.text.SetAlpha(1f);
        }

        #region Name Helpers
        private static string GetResourceName(ResourceTrackerDatabase.ResourceInfo resource, int count, bool isFragment, bool isKnownFragment)
        {
            string resourceName = isFragment
                ? GetFragmentNameWithoutKnown(resource.uniqueId) + (Main.Config.AppendKnown && isKnownFragment ? " (known)" : "")
                : Language.main.Get(resource.techType.AsString(false));

            return count > 1 && Main.Config.TextVisibility != "Hide count"
                ? $"{resourceName} x{count}"
                : resourceName;
        }

        private static string GetFragmentNameWithoutKnown(string uniqueId)
        {
            // Default fragment name
            string name = Language.main.Get("Fragment");

            if (Main.Config.SpecificFragmentNames)
            {
                TechType specificType = KnownFragmentFilter.GetFragmentType(uniqueId);
                if (specificType != TechType.None)
                {
                    name = Language.main.Get(specificType.AsString(false));
                }
            }

            return name;
        }
        #endregion

        #region Text Formatter
        private static string FormatText(string resourceName, string distanceText, int count)
        {
            stringBuilder.Clear();

            return Main.Config.TextVisibility switch
            {
                "Hide name" => count > 1 ? $"x{count}\r\n{distanceText}" : distanceText,
                "Hide distance" => resourceName,
                "Hide all" => "",
                _ => $"{resourceName}\r\n{distanceText}"
            };
        }
        #endregion
    }
}