using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(CyclopsLightingPanel))]
    public class SaveCyclopsInternalLights
    {
        private static string GetCyclopsId(CyclopsLightingPanel panel)
        {
            return panel?.cyclopsRoot?.gameObject?.GetComponent<PrefabIdentifier>()?.Id;
        }

        private static void UpdateInternalLightState(string cyclopsId, bool internalOff)
        {
            if (internalOff)
            {
                Main.SaveData.CyclopsesWithInternalLightOff.Add(cyclopsId);
            }
            else
            {
                Main.SaveData.CyclopsesWithInternalLightOff.Remove(cyclopsId);
            }
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

        [HarmonyPatch(nameof(CyclopsLightingPanel.ToggleInternalLighting)), HarmonyPostfix]
        public static void ToggleInternalLighting(CyclopsLightingPanel __instance)
        {
            string cyclopsId = GetCyclopsId(__instance);
            if (cyclopsId != null)
            {
                UpdateInternalLightState(cyclopsId, !__instance.lightingOn);
            }
        }

        [HarmonyPatch(nameof(CyclopsLightingPanel.Start)), HarmonyPostfix]
        public static void Start(CyclopsLightingPanel __instance)
        {
            string cyclopsId = GetCyclopsId(__instance);
            if (!Main.Config.SaveCyclopsInternalLights || cyclopsId != null)
            {
                RestoreInternalLightState(__instance, cyclopsId);
            }
        }
    }
}