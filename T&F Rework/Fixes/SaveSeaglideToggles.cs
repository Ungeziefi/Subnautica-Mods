// To-Do: Fix delayed save due to OnHolster

//using HarmonyLib;
//using System.Collections;
//using UWE;

//namespace Ungeziefi.Fixes
//{
//    [HarmonyPatch]
//    public class SaveSeaglideToggles
//    {
//        private static void SaveSeaglideState(Seaglide seaglide)
//        {
//            // Save map off
//            var seaglideMap = seaglide.GetComponent<VehicleInterface_MapController>();
//            if (seaglideMap?.miniWorld != null)
//                Main.SaveData.SeaglideMapOff = !seaglideMap.miniWorld.active;

//            // Save light on
//            if (seaglide.toggleLights != null)
//                Main.SaveData.SeaglideLightOn = seaglide.toggleLights.lightsActive;
//        }

//        private static IEnumerator LoadSeaglideState(Seaglide seaglide)
//        {
//            if (seaglide == null) yield break;

//            if (seaglide.toggleLights == null) yield return null;
//            seaglide.toggleLights.SetLightsActive(Main.SaveData.SeaglideLightOn);

//            var map = seaglide.GetComponent<VehicleInterface_MapController>();
//            if (map == null) yield break;

//            if (map.miniWorld == null) yield return null;
//            map.miniWorld.active = !Main.SaveData.SeaglideMapOff;
//        }

//        [HarmonyPatch(typeof(Seaglide), nameof(Seaglide.OnHolster)), HarmonyPrefix]
//        public static void Seaglide_OnHolster(Seaglide __instance)
//        {
//            if (!Main.Config.SaveSeaglideToggles) return;

//            SaveSeaglideState(__instance);
//        }

//        // Load state on start
//        [HarmonyPatch(typeof(Seaglide), nameof(Seaglide.Start)), HarmonyPostfix]
//        public static void Seaglide_Start(Seaglide __instance)
//        {
//            if (!Main.Config.SaveSeaglideToggles) return;

//            CoroutineHost.StartCoroutine(LoadSeaglideState(__instance));
//        }
//    }
//}