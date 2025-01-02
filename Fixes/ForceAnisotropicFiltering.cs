using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(MainCameraControl))]
    public class ForceAnisotropicFiltering
    {
        [HarmonyPatch(nameof(MainCameraControl.Awake)), HarmonyPostfix]
        public static void Awake(ScannerTool __instance)
        {
            if (Main.Config.ForceAnisotropicFiltering)
            {
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
            }
        }
    }
}