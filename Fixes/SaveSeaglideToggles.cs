using System.Collections;
using HarmonyLib;
using UWE;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(Seaglide))]
    public class SaveSeaglideToggles
    {
        [HarmonyPatch(nameof(Seaglide.OnHolster)), HarmonyPostfix]
        public static void OnHolsterPostfix(Seaglide __instance)
        {
            if (!Main.Config.SaveSeaglideToggles)
            {
                return;
            }

            SaveSeaglideState(__instance);
        }

        private static void SaveSeaglideState(Seaglide seaglide)
        {
            // Save map state (inverted)
            var seaglideMap = seaglide.GetComponent<VehicleInterface_MapController>();
            if (seaglideMap?.miniWorld != null)
            {
                Main.SaveData.SeaglideMapOff = !seaglideMap.miniWorld.active;
            }

            // Save light state (true when on)
            if (seaglide.toggleLights != null)
            {
                Main.SaveData.SeaglideLightOn = seaglide.toggleLights.lightsActive;
            }
        }

        private static IEnumerator LoadSeaglideState(Seaglide seaglide)
        {
            if (seaglide == null)
            {
                yield break;
            }

            // Wait for toggleLights to be initialized
            while (seaglide.toggleLights == null)
            {
                yield return null;
            }

            // Apply saved light state
            seaglide.toggleLights.SetLightsActive(Main.SaveData.SeaglideLightOn);

            // Get map component
            var map = seaglide.GetComponent<VehicleInterface_MapController>();
            if (map == null)
            {
                yield break;
            }

            // Wait for miniWorld to be initialized
            while (map.miniWorld == null)
            {
                yield return null;
            }

            // Apply saved map state (inverted)
            map.miniWorld.active = !Main.SaveData.SeaglideMapOff;
        }

        // Load states
        [HarmonyPatch(nameof(Seaglide.Start)), HarmonyPostfix]
        public static void Start(Seaglide __instance)
        {
            if (!Main.Config.SaveSeaglideToggles)
            {
                return;
            }

            CoroutineHost.StartCoroutine(LoadSeaglideState(__instance));
        }
    }
}