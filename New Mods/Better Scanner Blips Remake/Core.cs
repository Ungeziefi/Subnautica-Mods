using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UWE;

namespace Ungeziefi.Better_Scanner_Blips_Remake;

[HarmonyPatch]
public partial class BetterScannerBlipsRemake
{
    internal static bool blipsEnabled = true;
    private static readonly Dictionary<GameObject, (Graphic graphic, CanvasRenderer renderer)> blipComponents = new();
    private static readonly List<(ResourceTrackerDatabase.ResourceInfo resource, int count)> resourcePool = new(256);

    [HarmonyPatch(typeof(uGUI_ResourceTracker), nameof(uGUI_ResourceTracker.UpdateBlips))]
    [HarmonyPostfix]
    private static void uGUI_ResourceTracker_UpdateBlips(
        HashSet<ResourceTrackerDatabase.ResourceInfo> ___nodes,
        List<uGUI_ResourceTracker.Blip> ___blips,
        bool ___visible)
    {
        if (!___visible) return;

        resourcePool.Clear();

        var isPausedOrLoading = WaitScreen.IsWaiting
                                || Cursor.visible
                                || FreezeTime.HasFreezers()
                                || Player.main.GetPDA().isOpen;

        if (GameInput.GetButtonDown(Main.ToggleBlipsButton) && !isPausedOrLoading && !IsInHiddenLocation())
        {
            blipsEnabled = !blipsEnabled;
            if (Main.Config.ShowBlipToggleMessage)
                ErrorMessage.AddMessage(blipsEnabled ? "Scanner blips enabled" : "Scanner blips disabled");
        }

        if (!ColorManagement.colorsInitialized || ColorManagement.ColorSettingsChanged())
            ColorManagement.UpdateColorCache();

        var camera = MainCamera.camera;
        if (Main.Config.GroupNearbyResources)
            GroupResourcesToPool(___nodes, camera.transform.position, resourcePool);
        else
            foreach (var resource in ___nodes)
                resourcePool.Add((resource, 1));

        var activeBlipCount = 0;
        foreach (var (resource, count) in resourcePool)
        {
            var distance = Vector3.Distance(resource.position, camera.transform.position);

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
        for (var i = activeBlipCount; i < ___blips.Count; i++)
        {
            var blip = ___blips[i];
            blip.gameObject.SetActive(false);
        }
    }

    #region Cleanup

    [HarmonyPatch(typeof(uGUI_ResourceTracker), nameof(uGUI_ResourceTracker.OnDestroy))]
    [HarmonyPostfix]
    private static void uGUI_ResourceTracker_OnDestroy()
    {
        blipComponents.Clear();
    }

    #endregion

    #region Helpers

    private static bool IsInHiddenLocation()
    {
        var inHabitat = Main.Config.HideBlipsInsideHabitats && Player.main.IsInBase() &&
                        uGUI_CameraDrone.main.activeCamera == null;
        var inCyclops = Main.Config.HideBlipsInsideCyclops && Player.main.IsInSubmarine();

        return inHabitat || inCyclops;
    }

    private static bool ShouldHideBlip(float distance)
    {
        var shouldHideBlipManually =
            !blipsEnabled && (!Main.Config.RangeBasedToggle || distance <= Main.Config.ToggleRange);
        var shouldAutoHide = Main.Config.AutoHideNearbyBlips && !Main.Config.RangeBasedToggle &&
                             distance <= Main.Config.AutoHideDistance;
        var isInHiddenLocation = IsInHiddenLocation();

        return shouldHideBlipManually || shouldAutoHide || isInHiddenLocation;
    }

    #endregion
}