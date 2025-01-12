using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class CyclopsHelmHUDFixes
    {
        // Button names
        private static readonly string[] HelmButtonNames =
        {
            "Button_Camera",
            "Button_SilentRunning",
            "Button_Decoy",
            "Button_Sonar",
            "Button_Shield",
            "EngineOff_Button"
        };

        private static void UpdateHUDElements(Transform hudTransform, bool isActive)
        {
            // Update UI elements in children
            foreach (var element in hudTransform.GetComponentsInChildren<Image>(true))
            {
                // Only process buttons
                if (element.GetComponent<Button>() || System.Array.Exists(HelmButtonNames, name => name == element.name))
                {
                    element.raycastTarget = isActive;
                }
            }
        }

        [HarmonyPatch(typeof(CyclopsHelmHUDManager), nameof(CyclopsHelmHUDManager.Update)), HarmonyPrefix]
        public static bool CyclopsHelmHUDManager_Update(CyclopsHelmHUDManager __instance)
        {
            if (!Main.Config.CyclopsHelmHUDFixes || !__instance.LOD.IsFull())
            {
                return true;
            }

            // Get power status
            bool isPowered = __instance.GetComponentInParent<PowerRelay>()?.IsPowered() ?? false;

            // Get HUD status
            bool hudShouldBeActive = isPowered &&
                (bool)(AccessTools.Field(typeof(CyclopsHelmHUDManager), "hudActive").GetValue(__instance));

            // Update visibility and interactivity
            UpdateHUDElements(__instance.transform, hudShouldBeActive);

            // Only update if powered
            return isPowered;
        }
    }
}