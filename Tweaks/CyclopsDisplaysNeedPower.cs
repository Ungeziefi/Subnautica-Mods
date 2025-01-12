using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class CyclopsDisplaysNeedPower
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

            // Handle each component
            // Sonar
            var sonarMap = cyclops.GetComponentInChildren<CyclopsSonarDisplay>(true);
            if (sonarMap)
            {
                SetActive(sonarMap.transform.Find("VolumetricLight"), isPowered);
                SetActive(sonarMap.transform.Find("Base"), isPowered);
                SetActive(sonarMap.transform.Find("CyclopsMini"), isPowered);
            }

            // Compass
            SetActive(cyclops.GetComponentInChildren<CyclopsCompassHUD>(true)?.transform, isPowered);

            // Holographic HUD
            SetActive(cyclops.GetComponentInChildren<CyclopsHolographicHUD>(true)?.transform, isPowered);

            // Decoy screen
            SetActive(cyclops.GetComponentInChildren<CyclopsDecoyScreenHUDManager>(true)?.transform, isPowered);

            // Vehicle terminal
            SetActive(cyclops.GetComponentInChildren<CyclopsVehicleStorageTerminalManager>()?.transform.Find("GUIScreen"), isPowered);

            // Upgrade console
            SetActive(cyclops.transform.Find("UpgradeConsoleHUD"), isPowered);

            // Lighting panel
            SetActive(cyclops.GetComponentInChildren<CyclopsLightingPanel>(true)?.transform, isPowered);

            // SubName and its volumetric light
            SetActive(cyclops.GetComponentInChildren<CyclopsSubNameScreen>(true)?.transform, isPowered);
            SetActive(cyclops.transform.Find("SubName")?.Find("VolumetricLight"), isPowered);

            // Static lights
            SetActive(cyclops.transform.Find("CyclopsLightStatics"), isPowered);

            // Floodlights
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

        [HarmonyPatch(typeof(CyclopsHelmHUDManager), nameof(CyclopsHelmHUDManager.Update)), HarmonyPostfix]
        public static void CyclopsHelmHUDManager_Update(CyclopsHelmHUDManager __instance)
        {
            if (!Main.Config.CyclopsDisplaysNeedPower || !__instance.LOD.IsFull())
            {
                return;
            }

            var powerRelay = __instance.GetComponentInParent<PowerRelay>();
            bool isPowered = powerRelay != null && powerRelay.IsPowered();

            // Disable components above when power is off
            UpdateDisplayComponents(__instance.subRoot, isPowered);
        }
    }
}