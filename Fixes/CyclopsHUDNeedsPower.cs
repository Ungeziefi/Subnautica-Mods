using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(CyclopsHelmHUDManager))]
    public class CyclopsHUDNeedsPower
    {
        public static void UpdateDisplayComponents(SubRoot cyclops, bool isPowered)
        {
            void SetActive(Transform transform, bool active)
            {
                if (transform != null)
                {
                    transform.gameObject.SetActive(active);
                }
            }

            // Disable sonar components
            var sonarMap = cyclops.GetComponentInChildren<CyclopsSonarDisplay>(true);
            if (sonarMap)
            {
                SetActive(sonarMap.transform.Find("VolumetricLight"), isPowered);
                SetActive(sonarMap.transform.Find("Base"), isPowered);
                SetActive(sonarMap.transform.Find("CyclopsMini"), isPowered);
            }

            // Disable the compass
            SetActive(cyclops.GetComponentInChildren<CyclopsCompassHUD>(true)?.transform, isPowered);

            // Disable the holographic HUD
            SetActive(cyclops.GetComponentInChildren<CyclopsHolographicHUD>(true)?.transform, isPowered);

            // Disable the decoy screen
            SetActive(cyclops.GetComponentInChildren<CyclopsDecoyScreenHUDManager>(true)?.transform, isPowered);

            // Disable the vehicle terminal
            SetActive(cyclops.GetComponentInChildren<CyclopsVehicleStorageTerminalManager>()?.transform.Find("GUIScreen"), isPowered);

            // Disable the upgrade console
            SetActive(cyclops.transform.Find("UpgradeConsoleHUD"), isPowered);

            // Disable the lighting panel
            SetActive(cyclops.GetComponentInChildren<CyclopsLightingPanel>(true)?.transform, isPowered);

            // Disable SubName and its volumetric light
            SetActive(cyclops.GetComponentInChildren<CyclopsSubNameScreen>(true)?.transform, isPowered);
            SetActive(cyclops.transform.Find("SubName")?.Find("VolumetricLight"), isPowered);

            // Disable static lights
            SetActive(cyclops.transform.Find("CyclopsLightStatics"), isPowered);

            // Handle floodlights using CyclopsLightingPanel
            var lightingPanel = cyclops.GetComponentInChildren<CyclopsLightingPanel>(true);
            if (lightingPanel != null)
            {
                if (!isPowered)
                {
                    lightingPanel.floodlightsOn = false;
                    lightingPanel.SetExternalLighting(false);
                    lightingPanel.UpdateLightingButtons();
                }
            }
        }

        [HarmonyPatch(nameof(CyclopsHelmHUDManager.Update)), HarmonyPostfix]
        public static void Update(CyclopsHelmHUDManager __instance)
        {
            if (!Main.Config.CyclopsHUDNeedsPower || !__instance.LOD.IsFull())
            {
                return;
            }

            var powerRelay = __instance.GetComponentInParent<PowerRelay>();
            bool isPowered = powerRelay != null && powerRelay.IsPowered();

            UpdateDisplayComponents(__instance.subRoot, isPowered);

            // Disable all children to prevent clicking invisible buttons
            if (Main.Config.NoInvisibleCyclopsButtons)
            {
                SetChildrenActive(__instance.transform, isPowered);
            }
        }

        private static void SetChildrenActive(Transform parent, bool isActive)
        {
            foreach (Transform child in parent)
            {
                child.gameObject.SetActive(isActive);
            }
        }
    }
}