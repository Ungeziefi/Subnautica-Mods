using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class ScannerRoomSpeedMultiplier
    {
        private const float VANILLA_SCAN_INTERVAL = 14f;

        [HarmonyPatch(typeof(MapRoomFunctionality), nameof(MapRoomFunctionality.UpdateScanRangeAndInterval)), HarmonyPrefix]
        public static bool MapRoomFunctionality_UpdateScanRangeAndInterval_Prefix(MapRoomFunctionality __instance)
        {
            float previousInterval = __instance.scanInterval;

            int rangeUpgradeCount = __instance.storageContainer.container.GetCount(TechType.MapRoomUpgradeScanRange);
            __instance.scanRange = Mathf.Min(500f, 300f + rangeUpgradeCount * 50f);

            int speedUpgradeCount = __instance.storageContainer.container.GetCount(TechType.MapRoomUpgradeScanSpeed);
            __instance.scanInterval = Mathf.Max(1f, VANILLA_SCAN_INTERVAL * Main.Config.ScannerRoomSpeedMultiplier - speedUpgradeCount * 3f);

            __instance.ObtainResourceNodes(__instance.typeToScan);
            if (__instance.onScanRangeChanged != null)
                __instance.onScanRangeChanged();

            return false;
        }
    }
}