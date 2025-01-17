using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Anisotropic_Fix
{
    [HarmonyPatch]
    public class Core
    {
        [HarmonyPatch(typeof(MainCameraControl), nameof(MainCameraControl.Awake)), HarmonyPostfix]
        private static void UpdateBlipsPostfix()
        {
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
        }
    }
}