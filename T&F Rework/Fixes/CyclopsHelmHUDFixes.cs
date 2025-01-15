// Works but janky
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class CyclopsHelmHUDFixesOld
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
                if (element.GetComponent<Button>() ||
                    System.Array.Exists(HelmButtonNames, name => name == element.name))
                {
                    element.raycastTarget = isActive;
                }
            }
        }

        [HarmonyPatch(typeof(CyclopsHelmHUDManager), nameof(CyclopsHelmHUDManager.Update)), HarmonyPrefix]
        public static bool CyclopsHelmHUDManager_Update(CyclopsHelmHUDManager __instance)
        {
            if (!Main.Config.CyclopsHelmHUDFixes || !__instance.LOD.IsFull()) return true;

            // Get power status
            bool isPowered = __instance.GetComponentInParent<PowerRelay>()?.IsPowered() ?? false;

            // Get HUD status
            bool hudShouldBeActive = isPowered &&
                (bool)(AccessTools.Field(typeof(CyclopsHelmHUDManager), "hudActive").GetValue(__instance));

            // Update visibility and interactivity only if status has changed
            if (__instance.hudActive != hudShouldBeActive)
            {
                UpdateHUDElements(__instance.transform, hudShouldBeActive);
                __instance.hudActive = hudShouldBeActive;
            }

            // Only update if powered
            return isPowered;
        }
    }
}

// Alternative version
// Can't figure out how to disable the engine button when power on but HUD off
//using HarmonyLib;
//using UnityEngine;

//namespace Ungeziefi.Fixes
//{
//    [HarmonyPatch]
//    public class CyclopsHelmHUDFixes
//    {
//        private static bool IsHUDActive(Component __instance)
//        {
//            if (!Main.Config.CyclopsHelmHUDFixes) return true;

//            var helmHUDManager = __instance.GetComponentInParent<CyclopsHelmHUDManager>();
//            if (helmHUDManager != null)
//            {
//                return (bool)(AccessTools.Field(typeof(CyclopsHelmHUDManager), "hudActive").GetValue(helmHUDManager));
//            }
//            return false;
//        }

//        // Prevent helm from appearing when no power
//        [HarmonyPatch(typeof(CyclopsHelmHUDManager), nameof(CyclopsHelmHUDManager.Update)), HarmonyPrefix]
//        public static bool CyclopsHelmHUDManager_Update(CyclopsHelmHUDManager __instance)
//        {
//            if (!Main.Config.CyclopsHelmHUDFixes || !__instance.LOD.IsFull()) return true;

//            bool isPowered = __instance.GetComponentInParent<PowerRelay>()?.IsPowered() ?? false;

//            // Set HUD state to inactive if not powered
//            if (!isPowered)
//            {
//                AccessTools.Field(typeof(CyclopsHelmHUDManager), "hudActive").SetValue(__instance, false);
//            }

//            return true;
//        }

//        // Prevent button click when invisible
//        [HarmonyPatch(typeof(CyclopsDecoyLaunchButton), nameof(CyclopsDecoyLaunchButton.OnMouseEnter)), HarmonyPrefix]
//        public static bool CyclopsDecoyLaunchButton_OnMouseEnter(CyclopsDecoyLaunchButton __instance)
//        {
//            return IsHUDActive(__instance);
//        }

//        [HarmonyPatch(typeof(CyclopsExternalCamsButton), nameof(CyclopsExternalCamsButton.OnMouseEnter)), HarmonyPrefix]
//        public static bool CyclopsExternalCamsButton_OnMouseEnter(CyclopsExternalCamsButton __instance)
//        {
//            return IsHUDActive(__instance);
//        }

//        [HarmonyPatch(typeof(CyclopsShieldButton), nameof(CyclopsShieldButton.OnMouseEnter)), HarmonyPrefix]
//        public static bool CyclopsShieldButton_OnMouseEnter(CyclopsShieldButton __instance)
//        {
//            return IsHUDActive(__instance);
//        }

//        [HarmonyPatch(typeof(CyclopsSilentRunningAbilityButton), nameof(CyclopsSilentRunningAbilityButton.OnMouseEnter)), HarmonyPrefix]
//        public static bool CyclopsSilentRunningAbilityButton_OnMouseEnter(CyclopsSilentRunningAbilityButton __instance)
//        {
//            return IsHUDActive(__instance);
//        }

//        [HarmonyPatch(typeof(CyclopsSonarButton), nameof(CyclopsSonarButton.OnMouseEnter)), HarmonyPrefix]
//        public static bool CyclopsSonarButton_OnMouseEnter(CyclopsSonarButton __instance)
//        {
//            return IsHUDActive(__instance);
//        }
//    }
//}