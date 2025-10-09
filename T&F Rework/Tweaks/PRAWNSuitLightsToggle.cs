using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class PRAWNSuitLightsToggle
    {
        private static FMODAsset lightsOnSound;
        private static FMODAsset lightsOffSound;
        private static bool soundsInitialized = false;

        private static void InitializeLightsSounds()
        {
            if (soundsInitialized) return;

            lightsOnSound = ScriptableObject.CreateInstance<FMODAsset>();
            lightsOnSound.path = "event:/sub/seamoth/seaglide_light_on";
            lightsOnSound.id = "{fe76457f-0c94-4245-a080-8a5b2f8853c4}";

            lightsOffSound = ScriptableObject.CreateInstance<FMODAsset>();
            lightsOffSound.path = "event:/sub/seamoth/seaglide_light_off";
            lightsOffSound.id = "{b52592a9-19f5-45d1-ad56-7d355fc3dcc3}";

            soundsInitialized = true;
        }

        private static string GetExosuitId(Exosuit exosuit) =>
            exosuit?.gameObject?.GetComponent<PrefabIdentifier>()?.Id;

        // Public because used in PRAWNSuitLightsFollowsCamera as well
        public static Transform GetLightsTransform(Exosuit exosuit)
        {
            return exosuit?.leftArmAttach?.transform?.Find("lights_parent")
                   ?? exosuit?.transform?.Find("lights_parent");
        }

        private static void ToggleLights(Exosuit exosuit)
        {
            Transform lightsTransform = GetLightsTransform(exosuit);

            var lightsObject = lightsTransform.gameObject;
            var isCurrentlyOn = lightsObject.activeSelf;
            var exosuitId = GetExosuitId(exosuit);

            if (!isCurrentlyOn && exosuit.energyInterface.hasCharge)
            {
                lightsObject.SetActive(true);
                if (!string.IsNullOrEmpty(exosuitId))
                {
                    Main.SaveData.PRAWNSuitsWithLightOff.Remove(exosuitId);
                }
                if (lightsOnSound != null)
                {
                    Utils.PlayFMODAsset(lightsOnSound, exosuit.transform.position);
                }
            }
            else if (isCurrentlyOn)
            {
                lightsObject.SetActive(false);
                if (!string.IsNullOrEmpty(exosuitId))
                {
                    Main.SaveData.PRAWNSuitsWithLightOff.Add(exosuitId);
                }
                if (lightsOffSound != null)
                {
                    Utils.PlayFMODAsset(lightsOffSound, exosuit.transform.position);
                }
            }
        }

        // TO-DO: Fix not loading saved data on fresh game start
        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.Start)), HarmonyPostfix]
        public static void Exosuit_Start(Exosuit __instance)
        {
            InitializeLightsSounds();

            var exosuitId = GetExosuitId(__instance);
            if (Main.SaveData.PRAWNSuitsWithLightOff.Contains(exosuitId))
            {
                Main.Logger.LogInfo($"PRAWNSuit {exosuitId} contained, turning lights off on start");
                var lightsTransform = GetLightsTransform(__instance);
                if (lightsTransform != null)
                {
                    lightsTransform.gameObject.SetActive(false);
                    Main.Logger.LogInfo($"PRAWNSuit {exosuitId} lights set to off on start");
                }
            }
            else
            {
                Main.Logger.LogInfo($"PRAWNSuit {exosuitId} not contained");
            }
        }

        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.Update)), HarmonyPostfix]
        public static void Exosuit_Update(Exosuit __instance)
        {
            if (!Main.Config.TPSLEnableFeature) return;

            if (!Cursor.visible &&
                Player.main.currentMountedVehicle == __instance &&
                Input.GetKeyDown(Main.Config.PRAWNSuitLightsToggleKey))
            {
                ToggleLights(__instance);
            }
        }
    }
}