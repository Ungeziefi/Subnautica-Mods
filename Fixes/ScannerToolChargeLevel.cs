using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    // Fixed missing charge level when equipping the scanner
    [HarmonyPatch(typeof(ScannerTool))]
    public class FixScannerToolChargeLevel
    {
        [HarmonyPatch(nameof(ScannerTool.Update)), HarmonyPrefix]
        public static bool Update(ScannerTool __instance)
        {
            if (Main.FixesConfig.ScannerChargeIndicator && __instance.isDrawn)
            {
                if (__instance.idleTimer > 0f)
                {
                    __instance.idleTimer = Mathf.Max(0f, __instance.idleTimer - Time.deltaTime);
                }
                var buttonFormat = LanguageCache.GetButtonFormat("ScannerSelfScanFormat", GameInput.Button.AltTool);
                HandReticle.main.SetTextRaw(HandReticle.TextType.Use, buttonFormat);
            }
            return false;
        }
    }
}