using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class CyclopsSaveDataCleanup
    {
        [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.OnKill)), HarmonyPrefix]
        public static void SubRoot_OnKill(SubRoot __instance)
        {
            var identifier = __instance.gameObject.GetComponent<PrefabIdentifier>();
            if (identifier == null) return;

            string cyclopsId = identifier.Id;
            Main.SaveData.CyclopsSpeedMode.Remove(cyclopsId);
            Main.SaveData.CyclopsesWithInternalLightOff.Remove(cyclopsId);
            Main.SaveData.CyclopsesWithFloodlightsOn.Remove(cyclopsId);
        }
    }
}