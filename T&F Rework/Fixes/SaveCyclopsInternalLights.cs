using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class SaveCyclopsInternalLights
    {
        private static string GetCyclopsId(CyclopsLightingPanel panel)
        {
            if (panel == null)
                return null;

            if (panel.cyclopsRoot == null)
                return null;

            GameObject gameObject = panel.cyclopsRoot.gameObject;
            if (gameObject == null)
                return null;

            PrefabIdentifier prefabIdentifier = gameObject.GetComponent<PrefabIdentifier>();
            if (prefabIdentifier == null)
                return null;

            return prefabIdentifier.Id;
        }

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
                panel.cyclopsRoot.ForceLightingState(panel.lightingOn);
                panel.UpdateLightingButtons();
            }
        }

        // Save state on toggle
        [HarmonyPatch(typeof(CyclopsLightingPanel), nameof(CyclopsLightingPanel.ToggleInternalLighting)), HarmonyPostfix]
        public static void CyclopsLightingPanel_ToggleInternalLighting(CyclopsLightingPanel __instance)
        {
            if (!Main.Config.SaveCyclopsInternalLights) return;

            string cyclopsId = GetCyclopsId(__instance);
            if (cyclopsId == null) return;

            UpdateInternalLightState(cyclopsId, !__instance.lightingOn);
        }

        // Load state
        [HarmonyPatch(typeof(CyclopsLightingPanel), nameof(CyclopsLightingPanel.Start)), HarmonyPostfix]
        public static void CyclopsLightingPanel_Start(CyclopsLightingPanel __instance)
        {
            if (!Main.Config.SaveCyclopsInternalLights) return;

            string cyclopsId = GetCyclopsId(__instance);
            if (cyclopsId == null) return;

            RestoreInternalLightState(__instance, cyclopsId);
        }
    }
}