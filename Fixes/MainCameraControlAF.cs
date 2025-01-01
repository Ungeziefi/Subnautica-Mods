using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    // Fixed missing charge level when equipping the scanner
    [HarmonyPatch(typeof(MainCameraControl))]
    public class FixMainCameraControlAF
    {
        [HarmonyPatch(nameof(MainCameraControl.Awake)), HarmonyPostfix]
        public static void Awake(ScannerTool __instance)
        {
            if (Main.FixesConfig.ForceAnisotropicFiltering)
            {
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                // Main.Logger.LogInfo("Anisotropic filtering is now forced enabled."); 
            }
        }
    }
}