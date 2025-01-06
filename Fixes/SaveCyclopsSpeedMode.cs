using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(CyclopsMotorModeButton))]
    public class SaveCyclopsSpeedMode
    {
        // Helper method to get the unique ID of the Cyclops
        private static string GetCyclopsId(CyclopsMotorModeButton button)
        {
            return button?.subRoot?.gameObject?.GetComponent<PrefabIdentifier>()?.Id;
        }

        // Helper method to update the speed mode state
        private static void UpdateSpeedMode(string cyclopsId, CyclopsMotorMode.CyclopsMotorModes mode)
        {
            // Don't save the default mode
            if (mode == CyclopsMotorMode.CyclopsMotorModes.Standard)
            {
                Main.SaveData.CyclopsSpeedMode.Remove(cyclopsId);
            }

            // Save the current mode
            else
            {
                Main.SaveData.CyclopsSpeedMode[cyclopsId] = (int)mode;
            }
        }

        [HarmonyPatch(nameof(CyclopsMotorModeButton.OnClick)), HarmonyPostfix]
        public static void OnClick(CyclopsMotorModeButton __instance)
        {
            string cyclopsId = GetCyclopsId(__instance);
            if (!Main.Config.SaveCyclopsSpeedMode || cyclopsId == null)
            {
                return;
            }

            UpdateSpeedMode(cyclopsId, __instance.motorModeIndex);
        }

        [HarmonyPatch(nameof(CyclopsMotorModeButton.Start)), HarmonyPostfix]
        public static void Start(CyclopsMotorModeButton __instance)
        {
            string cyclopsId = GetCyclopsId(__instance);
            if (!Main.Config.SaveCyclopsSpeedMode || cyclopsId == null)
            {
                return;
            }

            // Restore the saved speed mode state
            if (Main.SaveData.CyclopsSpeedMode.TryGetValue(cyclopsId, out int savedMode))
            {
                __instance.SetCyclopsMotorMode((CyclopsMotorMode.CyclopsMotorModes)savedMode);
            }
        }
    }
}