using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(CyclopsMotorModeButton))]
    public class SaveCyclopsSpeedMode
    {
        [HarmonyPatch(nameof(CyclopsMotorModeButton.OnClick)), HarmonyPostfix]
        public static void OnClick(CyclopsMotorModeButton __instance)
        {
            var identifier = __instance.subRoot.gameObject.GetComponent<PrefabIdentifier>();
            if (identifier == null)
            {
                return;
            }

            string cyclopsId = identifier.Id;

            // No point in saving the default speed mode
            if (__instance.motorModeIndex == CyclopsMotorMode.CyclopsMotorModes.Standard)
            {
                Main.SaveData.CyclopsSpeedMode.Remove(cyclopsId);
            }

            Main.SaveData.CyclopsSpeedMode[cyclopsId] = (int)__instance.motorModeIndex;
        }

        [HarmonyPatch(nameof(CyclopsMotorModeButton.Start)), HarmonyPostfix]
        public static void Start(CyclopsMotorModeButton __instance)
        {
            var identifier = __instance.subRoot.gameObject.GetComponent<PrefabIdentifier>();
            if (!Main.Config.SaveCyclopsSpeedMode || identifier == null)
            {
                return;
            }

            string cyclopsId = identifier.Id;

            // Restore the saved speed mode
            if (Main.SaveData.CyclopsSpeedMode.TryGetValue(cyclopsId, out int savedMode))
            {
                __instance.SetCyclopsMotorMode((CyclopsMotorMode.CyclopsMotorModes)savedMode);
            }
        }
    }

    // Remove the speed mode data when a Cyclops is destroyed
    [HarmonyPatch(typeof(SubRoot))]
    public class CyclopsDestructionHandler
    {
        [HarmonyPatch(nameof(SubRoot.OnKill)), HarmonyPrefix]
        public static void OnKill(SubRoot __instance)
        {
            var identifier = __instance.gameObject.GetComponent<PrefabIdentifier>();
            if (identifier == null)
            {
                return;
            }

            string cyclopsId = identifier.Id;
            Main.SaveData.CyclopsSpeedMode.Remove(cyclopsId);
        }
    }
}