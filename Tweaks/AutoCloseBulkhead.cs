using System.Collections.Generic;
using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class AutoCloseBulkheadPatch
    {
        private static bool wasLeaking = false;

        private static void CloseBulkheadDoorsNearLeak(Leakable leakable)
        {
            // Find bulkhead doors
            SubRoot subRoot = leakable.gameObject.GetComponentInParent<SubRoot>();
            if (subRoot == null) return;
            BulkheadDoor[] doors = subRoot.GetComponentsInChildren<BulkheadDoor>();

            // Find leak points
            List<VFXSubLeakPoint> leakPoints = leakable.GetLeakPoints();
            if (leakPoints == null || leakPoints.Count == 0) return;

            // Close doors instantly
            foreach (BulkheadDoor door in doors)
            {
                if (door.opened)
                {
                    door.SetState(false);
                }
            }
        }

        [HarmonyPatch(typeof(Leakable), nameof(Leakable.UpdateLeakPoints)), HarmonyPostfix]
        public static void Leakable_UpdateLeakPoints(Leakable __instance)
        {
            if (!Main.Config.AutoCloseBulkheadDoors)
            {
                return;
            }

            // Monitor leak state
            bool isLeaking = __instance.IsLeaking();
            if (!wasLeaking && isLeaking)
            {
                wasLeaking = true;
                CloseBulkheadDoorsNearLeak(__instance);
            }
            else if (!isLeaking)
            {
                wasLeaking = false;
            }
        }
    }
}