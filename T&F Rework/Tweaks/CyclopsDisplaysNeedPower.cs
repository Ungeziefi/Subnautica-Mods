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
                if (transform != null) transform.gameObject.SetActive(active);
            }

            var sonarMap = cyclops.GetComponentInChildren<CyclopsSonarDisplay>(true);
            if (sonarMap)
            {
                var sonarTransform = sonarMap.transform;
                SetActive(sonarTransform.Find("VolumetricLight"), isPowered);
                SetActive(sonarTransform.Find("Base"), isPowered);
                SetActive(sonarTransform.Find("CyclopsMini"), isPowered);
            }

            SetActive(cyclops.GetComponentInChildren<CyclopsCompassHUD>(true)?.transform, isPowered);
            SetActive(cyclops.GetComponentInChildren<CyclopsHolographicHUD>(true)?.transform, isPowered);
            SetActive(cyclops.GetComponentInChildren<CyclopsDecoyScreenHUDManager>(true)?.transform, isPowered);
            SetActive(cyclops.GetComponentInChildren<CyclopsVehicleStorageTerminalManager>()?.transform.Find("GUIScreen"), isPowered);
            SetActive(cyclops.transform.Find("UpgradeConsoleHUD"), isPowered);
            SetActive(cyclops.GetComponentInChildren<CyclopsLightingPanel>(true)?.transform, isPowered);
            SetActive(cyclops.GetComponentInChildren<CyclopsSubNameScreen>(true)?.transform, isPowered);
            SetActive(cyclops.transform.Find("SubName")?.Find("VolumetricLight"), isPowered); // Volumetric light below SubName
            SetActive(cyclops.transform.Find("CyclopsLightStatics"), isPowered); // Fake light

            var lightingPanel = cyclops.GetComponentInChildren<CyclopsLightingPanel>(true);
            if (lightingPanel != null && !isPowered)
            {
                lightingPanel.floodlightsOn = false;
                lightingPanel.SetExternalLighting(false);
                lightingPanel.UpdateLightingButtons();
            }

            var helmHUD = cyclops.GetComponentInChildren<CyclopsHelmHUDManager>(true);
            if (helmHUD != null)
            {
                AccessTools.Field(typeof(CyclopsHelmHUDManager), "hudActive").SetValue(helmHUD, isPowered);
            }
        }

        [HarmonyPatch(typeof(CyclopsHelmHUDManager), nameof(CyclopsHelmHUDManager.Update)), HarmonyPostfix]
        public static void CyclopsHelmHUDManager_Update(CyclopsHelmHUDManager __instance)
        {
            if (!Main.Config.CyclopsDisplaysNeedPower || !__instance.LOD.IsFull()) return;

            var powerRelay = __instance.GetComponentInParent<PowerRelay>();
            bool isPowered = powerRelay != null && powerRelay.IsPowered();

            // Disable when power is off
            UpdateDisplayComponents(__instance.subRoot, isPowered);
        }
    }
}