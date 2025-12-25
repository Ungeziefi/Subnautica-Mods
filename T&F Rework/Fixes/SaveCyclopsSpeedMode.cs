using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class SaveCyclopsSpeedMode
    {
        private static string GetCyclopsId(CyclopsMotorModeButton button)
        {
            if (button == null)
                return null;

            if (button.subRoot == null)
                return null;

            GameObject gameObject = button.subRoot.gameObject;
            if (gameObject == null)
                return null;

            PrefabIdentifier prefabIdentifier = gameObject.GetComponent<PrefabIdentifier>();
            if (prefabIdentifier == null)
                return null;

            return prefabIdentifier.Id;
        }

        private static void UpdateSpeedMode(string cyclopsId, CyclopsMotorMode.CyclopsMotorModes mode)
        {
            // Remove when default, save otherwise
            if (mode == CyclopsMotorMode.CyclopsMotorModes.Standard)
                Main.SaveData.CyclopsSpeedMode.Remove(cyclopsId);
            else
                Main.SaveData.CyclopsSpeedMode[cyclopsId] = (int)mode;
        }

        [HarmonyPatch(typeof(CyclopsMotorModeButton), nameof(CyclopsMotorModeButton.OnClick)), HarmonyPostfix]
        public static void CyclopsMotorModeButton_OnClick(CyclopsMotorModeButton __instance)
        {
            string cyclopsId = GetCyclopsId(__instance);
            if (!Main.Config.SaveCyclopsSpeedMode || cyclopsId == null) return;

            UpdateSpeedMode(cyclopsId, __instance.motorModeIndex);
        }

        [HarmonyPatch(typeof(CyclopsMotorModeButton), nameof(CyclopsMotorModeButton.Start)), HarmonyPostfix]
        public static void CyclopsMotorModeButton_Start(CyclopsMotorModeButton __instance)
        {
            string cyclopsId = GetCyclopsId(__instance);
            if (!Main.Config.SaveCyclopsSpeedMode || cyclopsId == null) return;

            // Restore the saved speed mode state
            if (Main.SaveData.CyclopsSpeedMode.TryGetValue(cyclopsId, out int savedMode))
                __instance.SetCyclopsMotorMode((CyclopsMotorMode.CyclopsMotorModes)savedMode);
        }
    }
}