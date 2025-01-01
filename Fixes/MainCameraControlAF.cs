using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(MainCameraControl))]
    public class FixMainCameraControlAF
    {
        [HarmonyPatch(nameof(MainCameraControl.Awake)), HarmonyPostfix]
        public static void Awake(ScannerTool __instance)
        {
            if (Main.FixesConfig.ForceAnisotropicFiltering)
            {
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
            }
        }
    }
}