using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(CyclopsLightingPanel))]
    public class SaveCyclopsFloodlights
    {
        private static string GetCyclopsId(CyclopsLightingPanel panel)
        {
            return panel?.cyclopsRoot?.gameObject?.GetComponent<PrefabIdentifier>()?.Id;
        }

        private static void UpdateExternalLightState(string cyclopsId, bool externalOn)
        {
            // Save when turning on
            if (externalOn)
            {
                Main.SaveData.CyclopsesWithFloodlightsOn.Add(cyclopsId);
            }

            // Dont save when turning off
            else
            {
                Main.SaveData.CyclopsesWithFloodlightsOn.Remove(cyclopsId);
            }
        }

        private static void RestoreExternalLightState(CyclopsLightingPanel panel, string cyclopsId)
        {
            if (Main.SaveData.CyclopsesWithFloodlightsOn.Contains(cyclopsId))
            {
                panel.floodlightsOn = true;
                panel.SetExternalLighting(true);
                panel.UpdateLightingButtons();
            }
        }

        [HarmonyPatch(nameof(CyclopsLightingPanel.ToggleFloodlights)), HarmonyPostfix]
        public static void ToggleFloodlights(CyclopsLightingPanel __instance)
        {
            if (!Main.Config.SaveCyclopsFloodlights)
            {
                return;
            }

            string cyclopsId = GetCyclopsId(__instance);
            if (cyclopsId != null)
            {
                UpdateExternalLightState(cyclopsId, __instance.floodlightsOn);
            }
        }

        [HarmonyPatch(nameof(CyclopsLightingPanel.SubConstructionComplete)), HarmonyPostfix]
        public static void SubConstructionComplete(CyclopsLightingPanel __instance)
        {
            string cyclopsId = GetCyclopsId(__instance);
            if (!Main.Config.SaveCyclopsFloodlights || cyclopsId != null)
            {
                UpdateExternalLightState(cyclopsId, __instance.floodlightsOn);
            }
        }

        [HarmonyPatch(nameof(CyclopsLightingPanel.Start)), HarmonyPostfix]
        public static void Start(CyclopsLightingPanel __instance)
        {
            string cyclopsId = GetCyclopsId(__instance);
            if (!Main.Config.SaveCyclopsFloodlights || cyclopsId != null)
            {
                RestoreExternalLightState(__instance, cyclopsId);
            }
        }
    }
}