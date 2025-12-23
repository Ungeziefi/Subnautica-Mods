using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class CyclopsSaveDataCleanup
    {
        [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.OnKill)), HarmonyPrefix]
        public static void SubRoot_OnKill(SubRoot __instance)
        {
            if (__instance == null) return;

            var prefab = __instance.gameObject.GetComponent<PrefabIdentifier>();
            if (prefab == null) return;

            string cyclopsId = prefab.Id;

            Main.SaveData.CyclopsSpeedMode.Remove(cyclopsId);
            Main.SaveData.CyclopsesWithInternalLightOff.Remove(cyclopsId);
            Main.SaveData.CyclopsesWithFloodlightsOn.Remove(cyclopsId);
        }
    }
}