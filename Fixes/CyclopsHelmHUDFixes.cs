using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class CyclopsHelmHUDFixes
    {
        // Cache the button names for performance
        private static readonly string[] HelmButtonNames =
        {
            "Button_Camera",
            "Button_SilentRunning",
            "Button_Decoy",
            "Button_Sonar",
            "Button_Shield",
            "EngineOff_Button"
        };

        [HarmonyPatch(typeof(CyclopsHelmHUDManager), nameof(CyclopsHelmHUDManager.Update)), HarmonyPrefix]
        public static bool CyclopsHelmHUDManager_Update(CyclopsHelmHUDManager __instance)
        {
            // Early return if LOD check fails
            if (!Main.Config.CyclopsHelmHUDFixes || !__instance.LOD.IsFull())
            {
                return true;
            }

            // Get power status
            bool isPowered = __instance.GetComponentInParent<PowerRelay>()?.IsPowered() ?? false;

            // Get HUD status - only matters if we have power
            bool hudShouldBeActive = isPowered &&
                (bool)(AccessTools.Field(typeof(CyclopsHelmHUDManager), "hudActive").GetValue(__instance));

            // Update UI elements visibility and interactivity
            UpdateHUDElements(__instance.transform, hudShouldBeActive);

            // Only run original Update if we have power
            return isPowered;
        }

        private static void UpdateHUDElements(Transform hudTransform, bool isActive)
        {
            // Update all UI elements in children
            foreach (var element in hudTransform.GetComponentsInChildren<Image>(true))
            {
                // Only process button-related elements
                if (element.GetComponent<Button>() || System.Array.Exists(HelmButtonNames, name => name == element.name))
                {
                    element.raycastTarget = isActive;
                }
            }
        }
    }
}