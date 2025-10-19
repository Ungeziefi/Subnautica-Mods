using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.Better_Scanner_Blips_Remake
{
    [HarmonyPatch]
    public partial class BetterScannerBlipsRemake
    {
        internal static bool blipsEnabled = true;
        private static readonly Dictionary<GameObject, (Graphic graphic, CanvasRenderer renderer)> blipComponents = new();
        private static List<(ResourceTrackerDatabase.ResourceInfo resource, int count)> resourcePool = new(256);

        [HarmonyPatch(typeof(uGUI_ResourceTracker), nameof(uGUI_ResourceTracker.UpdateBlips)), HarmonyPostfix]
        private static void uGUI_ResourceTracker_UpdateBlips(
            HashSet<ResourceTrackerDatabase.ResourceInfo> ___nodes,
            List<uGUI_ResourceTracker.Blip> ___blips,
            bool ___visible)
        {
            if (!___visible) return;

            if (Input.GetKeyDown(Main.Config.ToggleBlipsKey) && !Cursor.visible && !IsInHiddenLocation())
            {
                blipsEnabled = !blipsEnabled;
                if (Main.Config.ShowBlipToggleMessage)
                {
                    ErrorMessage.AddMessage(blipsEnabled ? "Scanner blips enabled" : "Scanner blips disabled");
                }
            }

            if (!ColorManagement.colorsInitialized || ColorManagement.ColorSettingsChanged())
            {
                ColorManagement.UpdateColorCache();
            }

            var camera = MainCamera.camera;

            // Clear pool but keep capacity
            resourcePool.Clear();

            if (Main.Config.GroupNearbyResources)
            {
                GroupResourcesToPool(___nodes, camera.transform.position, resourcePool);
            }
            else
            {
                foreach (var resource in ___nodes)
                {
                    resourcePool.Add((resource, 1));
                }
            }

            int activeBlipCount = 0;
            foreach (var (resource, count) in resourcePool)
            {
                float distance = Vector3.Distance(resource.position, camera.transform.position);

                // Skip hidden blips
                if (ShouldHideBlip(distance))
                    continue;

                // Stop if over the limit
                if (activeBlipCount >= ___blips.Count)
                    break;

                var blip = ___blips[activeBlipCount];
                UpdateBlip(blip, resource, distance, camera, count);
                activeBlipCount++;
            }

            // Hide unused blips
            for (int i = activeBlipCount; i < ___blips.Count; i++)
            {
                var blip = ___blips[i];
                blip.gameObject.SetActive(false);
            }
        }

        #region Helpers
        private static bool IsInHiddenLocation()
        {
            bool inHabitat = Main.Config.HideBlipsInsideHabitats && Player.main.IsInBase() && uGUI_CameraDrone.main?.activeCamera == null;
            bool inCyclops = Main.Config.HideBlipsInsideCyclops && Player.main.IsInSubmarine();

            return inHabitat || inCyclops;
        }

        private static bool ShouldHideBlip(float distance)
        {
            bool shouldHideBlipManually = !blipsEnabled && (!Main.Config.RangeBasedToggle || distance <= Main.Config.ToggleRange);
            bool shouldAutoHide = Main.Config.AutoHideNearbyBlips && !Main.Config.RangeBasedToggle && distance <= Main.Config.AutoHideDistance;
            bool isInHiddenLocation = IsInHiddenLocation();

            return shouldHideBlipManually || shouldAutoHide || isInHiddenLocation;
        }
        #endregion

        #region Cleanup
        [HarmonyPatch(typeof(uGUI_ResourceTracker), nameof(uGUI_ResourceTracker.OnDestroy)), HarmonyPostfix]
        private static void uGUI_ResourceTracker_OnDestroy()
        {
            blipComponents.Clear();
        }
        #endregion
    }
}