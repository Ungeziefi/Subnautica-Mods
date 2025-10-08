using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class PRAWNSuitLightToggle
    {
        #region Sounds
        public static FMODAsset lightOnSound;
        public static FMODAsset lightOffSound;

        private static void CreateLightSounds(Exosuit exosuit)
        {
            lightOnSound = ScriptableObject.CreateInstance<FMODAsset>();
            lightOnSound.path = "event:/sub/seamoth/seaglide_light_on";
            lightOnSound.id = "{fe76457f-0c94-4245-a080-8a5b2f8853c4}";

            lightOffSound = ScriptableObject.CreateInstance<FMODAsset>();
            lightOffSound.path = "event:/sub/seamoth/seaglide_light_off";
            lightOffSound.id = "{b52592a9-19f5-45d1-ad56-7d355fc3dcc3}";
        }

        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.Start)), HarmonyPostfix]
        public static void Exosuit_Start(Exosuit __instance)
        {
            CreateLightSounds(__instance);
        }
        #endregion

        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.Update)), HarmonyPostfix]
        public static void Exosuit_Update(Exosuit __instance)
        {
            if (Main.Config.TPSLEnableFeature)
            {

            }
        }
    }
}