using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace Ungeziefi.Moonpool_Rotation
{
    [HarmonyPatch]
    public class MoonpoolRotation
    {
        private static MonoBehaviour coroutineHost;
        private static Transform moonpoolAnimCache;
        private static bool isRotating = false;
        private static bool needsRotation = false;
        private static float WAITBEFOREROTATION = 1.8f;
        private static float RETURNROTATIONDURATION = 1.5f;

        [HarmonyPatch(typeof(VehicleDockingBay), nameof(VehicleDockingBay.Start)), HarmonyPostfix]
        public static void VehicleDockingBay_Start(VehicleDockingBay __instance)
        {
            coroutineHost = __instance;
            moonpoolAnimCache = __instance.transform.parent?.Find("moon_pool_anim");
        }

        [HarmonyPatch(typeof(VehicleDockingBay), nameof(VehicleDockingBay.LateUpdate)), HarmonyPrefix]
        public static bool VehicleDockingBay_LateUpdate(VehicleDockingBay __instance)
        {
            if (!Main.Config.EnableFeature || moonpoolAnimCache == null) return true;

            Vehicle dockingVehicle = __instance.interpolatingVehicle;

            if (dockingVehicle != null && !isRotating && !needsRotation)
            {
                // dot < 0 = opposite direction
                if (Vector3.Dot(dockingVehicle.transform.right, moonpoolAnimCache.right) < 0)
                {
                    needsRotation = true;
                    coroutineHost.StartCoroutine(RotationSequence(moonpoolAnimCache));
                    return false; // Skip LateUpdate during rotation
                }
            }

            return true;
        }

        private static IEnumerator RotationSequence(Transform moonpoolAnim)
        {
            isRotating = true;

            // Store original rotation for return phase
            Quaternion originalRotation = moonpoolAnim.rotation;

            // Initial 180-degree rotation (instant)
            moonpoolAnim.rotation = originalRotation * Quaternion.Euler(0f, 180f, 0f);

            // Configurable pause before returning
            yield return new WaitForSeconds(WAITBEFOREROTATION);

            // Smoothly rotate back to original position
            yield return RotateTransform(moonpoolAnim, originalRotation, RETURNROTATIONDURATION);

            // Reset state flags
            isRotating = false;
            needsRotation = false;
        }

        private static IEnumerator RotateTransform(Transform moonpoolAnim, Quaternion targetRotation, float duration)
        {
            float elapsedTime = 0;
            Quaternion startRotation = moonpoolAnim.rotation;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                // Acceleration and deceleration
                float t = Mathf.SmoothStep(0, 1, elapsedTime / duration);
                moonpoolAnim.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
                yield return null;
            }

            // Ensure final rotation just in case
            moonpoolAnim.rotation = targetRotation;
        }
    }
}