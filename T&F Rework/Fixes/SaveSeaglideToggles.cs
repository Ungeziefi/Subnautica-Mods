using System.Collections;
using HarmonyLib;
using UWE;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class SaveSeaglideToggles
    {
        private static void SaveSeaglideState(Seaglide seaglide)
        {
            // Save map off
            var seaglideMap = seaglide.GetComponent<VehicleInterface_MapController>();
            if (seaglideMap?.miniWorld != null)
                Main.SaveData.SeaglideMapOff = !seaglideMap.miniWorld.active;

            // Save light on
            if (seaglide.toggleLights != null)
                Main.SaveData.SeaglideLightOn = seaglide.toggleLights.lightsActive;
        }

        private static IEnumerator LoadSeaglideState(Seaglide seaglide)
        {
            if (seaglide == null) yield break;

            while (seaglide.toggleLights == null) yield return null;
            seaglide.toggleLights.SetLightsActive(Main.SaveData.SeaglideLightOn);

            var map = seaglide.GetComponent<VehicleInterface_MapController>();
            if (map == null) yield break;

            while (map.miniWorld == null) yield return null;
            map.miniWorld.active = !Main.SaveData.SeaglideMapOff;
        }

        // Save states
        [HarmonyPatch(typeof(Seaglide), nameof(Seaglide.OnHolster)), HarmonyPostfix]
        public static void Seaglide_OnHolster(Seaglide __instance)
        {
            if (!Main.Config.SaveSeaglideToggles) return;

            SaveSeaglideState(__instance);
        }

        // Load states
        [HarmonyPatch(typeof(Seaglide), nameof(Seaglide.Start)), HarmonyPostfix]
        public static void Seaglide_Start(Seaglide __instance)
        {
            if (!Main.Config.SaveSeaglideToggles) return;

            CoroutineHost.StartCoroutine(LoadSeaglideState(__instance));
        }
    }
}