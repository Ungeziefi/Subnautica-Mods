using HarmonyLib;
using UnityEngine.UI;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class DisableGhostHelmButtons
    {
        private static void ToggleRaycasters(CyclopsHelmHUDManager instance, bool enabled)
        {
            var helmHUDVisuals = instance.transform.Find("HelmHUDVisuals");
            if (helmHUDVisuals == null) return;

            var leftCanvas = helmHUDVisuals.Find("Canvas_LeftHUD");
            if (leftCanvas != null)
            {
                var leftRaycaster = leftCanvas.GetComponent<GraphicRaycaster>();
                if (leftRaycaster != null)
                    leftRaycaster.enabled = enabled;
            }

            var rightCanvas = helmHUDVisuals.Find("Canvas_RightHUD");
            if (rightCanvas != null)
            {
                var rightRaycaster = rightCanvas.GetComponent<uGUI_GraphicRaycaster>();
                if (rightRaycaster != null)
                    rightRaycaster.enabled = enabled;
            }
        }

        // Disable raycasters on start
        [HarmonyPatch(typeof(CyclopsHelmHUDManager), nameof(CyclopsHelmHUDManager.Start)), HarmonyPostfix]
        public static void CyclopsHelmHUDManager_Start(CyclopsHelmHUDManager __instance)
        {
            if (!Main.Config.DisableGhostHelmButtons) return;

            ToggleRaycasters(__instance, false);
        }

        // Enable raycasters when piloting
        [HarmonyPatch(typeof(CyclopsHelmHUDManager), nameof(CyclopsHelmHUDManager.StartPiloting)), HarmonyPostfix]
        public static void CyclopsHelmHUDManager_StartPiloting(CyclopsHelmHUDManager __instance)
        {
            if (!Main.Config.DisableGhostHelmButtons) return;

            ToggleRaycasters(__instance, true);
        }

        // Disable again when stopping piloting
        [HarmonyPatch(typeof(CyclopsHelmHUDManager), nameof(CyclopsHelmHUDManager.StopPiloting)), HarmonyPostfix]
        public static void CyclopsHelmHUDManager_StopPiloting(CyclopsHelmHUDManager __instance)
        {
            if (!Main.Config.DisableGhostHelmButtons) return;

            ToggleRaycasters(__instance, false);
        }
    }
}