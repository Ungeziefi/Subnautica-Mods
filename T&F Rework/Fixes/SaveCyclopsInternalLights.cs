using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class SaveCyclopsInternalLights
    {
        private static string GetCyclopsId(CyclopsLightingPanel panel) =>
            panel?.cyclopsRoot?.gameObject?.GetComponent<PrefabIdentifier>()?.Id;

        private static void UpdateInternalLightState(string cyclopsId, bool internalOff)
        {
            // Save when turning off, remove when turning on
            if (internalOff)
                Main.SaveData.CyclopsesWithInternalLightOff.Add(cyclopsId);
            else
                Main.SaveData.CyclopsesWithInternalLightOff.Remove(cyclopsId);
        }

        private static void RestoreInternalLightState(CyclopsLightingPanel panel, string cyclopsId)
        {
            if (Main.SaveData.CyclopsesWithInternalLightOff.Contains(cyclopsId))
            {
                panel.lightingOn = false;
                panel.cyclopsRoot.ForceLightingState(false);
                panel.UpdateLightingButtons();
            }
        }

        // Save state on toggle
        [HarmonyPatch(typeof(CyclopsLightingPanel), nameof(CyclopsLightingPanel.ToggleInternalLighting)), HarmonyPostfix]
        public static void CyclopsLightingPanel_ToggleInternalLighting(CyclopsLightingPanel __instance)
        {
            if (!Main.Config.SaveCyclopsInternalLights) return;

            string cyclopsId = GetCyclopsId(__instance);
            if (cyclopsId != null)
                UpdateInternalLightState(cyclopsId, !__instance.lightingOn);
        }

        // Load state
        [HarmonyPatch(typeof(CyclopsLightingPanel), nameof(CyclopsLightingPanel.Start)), HarmonyPostfix]
        public static void CyclopsLightingPanel_Start(CyclopsLightingPanel __instance)
        {
            string cyclopsId = GetCyclopsId(__instance);
            if (!Main.Config.SaveCyclopsInternalLights || cyclopsId != null)
                RestoreInternalLightState(__instance, cyclopsId);
        }
    }
}