// To-Do: Fix not restoring on game reload

using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class SaveCyclopsFloodlights
    {
        private static string GetCyclopsId(CyclopsLightingPanel panel) =>
            panel?.cyclopsRoot?.gameObject?.GetComponent<PrefabIdentifier>()?.Id;

        private static void UpdateExternalLightState(string cyclopsId, bool externalOn)
        {
            // Save when turning on, remove when turning off
            if (externalOn)
                Main.SaveData.CyclopsesWithFloodlightsOn.Add(cyclopsId);
            else
                Main.SaveData.CyclopsesWithFloodlightsOn.Remove(cyclopsId);
        }

        private static void RestoreExternalLightState(CyclopsLightingPanel panel, string cyclopsId)
        {
            if (panel == null || string.IsNullOrEmpty(cyclopsId))
                return;

            if (Main.SaveData.CyclopsesWithFloodlightsOn.Contains(cyclopsId))
            {
                panel.floodlightsOn = true;
                panel.SetExternalLighting(true);
                panel.UpdateLightingButtons();
            }
        }

        // Save state on toggle
        [HarmonyPatch(typeof(CyclopsLightingPanel), nameof(CyclopsLightingPanel.ToggleFloodlights)), HarmonyPostfix]
        public static void CyclopsLightingPanel_ToggleFloodlights(CyclopsLightingPanel __instance)
        {
            if (!Main.Config.SaveCyclopsFloodlights) return;

            string cyclopsId = GetCyclopsId(__instance);
            if (cyclopsId != null)
                UpdateExternalLightState(cyclopsId, __instance.floodlightsOn);
        }

        // Save state on build
        [HarmonyPatch(typeof(CyclopsLightingPanel), nameof(CyclopsLightingPanel.SubConstructionComplete)), HarmonyPostfix]
        public static void CyclopsLightingPanel_SubConstructionComplete(CyclopsLightingPanel __instance)
        {
            if (!Main.Config.SaveCyclopsFloodlights) return;

            string cyclopsId = GetCyclopsId(__instance);
            if (cyclopsId != null)
                UpdateExternalLightState(cyclopsId, __instance.floodlightsOn);
        }

        // Load state
        [HarmonyPatch(typeof(CyclopsLightingPanel), nameof(CyclopsLightingPanel.Start)), HarmonyPostfix]
        public static void CyclopsLightingPanel_Start(CyclopsLightingPanel __instance)
        {
            if (!Main.Config.SaveCyclopsFloodlights) return;

            string cyclopsId = GetCyclopsId(__instance);
            if (cyclopsId != null)
                RestoreExternalLightState(__instance, cyclopsId);
        }
    }
}