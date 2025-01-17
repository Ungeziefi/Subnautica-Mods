using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Dynamic_Scanner_Blips
{
    [HarmonyPatch]
    public class Core
    {
        [HarmonyPatch(typeof(uGUI_ResourceTracker), nameof(uGUI_ResourceTracker.UpdateBlips)), HarmonyPostfix]
        private static void UpdateBlipsPostfix(
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
                blip.text.SetAlpha(1f);
                renderer.SetAlpha(1f);

                var resourceName = Language.main.Get(resource.techType.AsString(false));
                blip.text.SetText(Main.Config.ShowDistance
                    ? $"{resourceName}\r\n{Mathf.RoundToInt(distance)}{Language.main.Get("MeterSuffix")}"
                    : resourceName,
                    true);

                blipIndex++;
            }
        }
    }
}