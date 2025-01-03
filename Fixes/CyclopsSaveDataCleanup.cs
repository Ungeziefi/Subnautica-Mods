using HarmonyLib;

namespace Ungeziefi.Fixes
{
    // Remove saved data when a Cyclops is destroyed
    [HarmonyPatch(typeof(SubRoot))]
    public class CyclopsSaveDataCleanup
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
            Main.SaveData.CyclopsesWithInternalLightOff.Remove(cyclopsId);
            Main.SaveData.CyclopsesWithFloodlightsOn.Remove(cyclopsId);
        }
    }
}