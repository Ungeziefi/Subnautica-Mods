using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace Ungeziefi.Moonpool_Rotation
{
    [HarmonyPatch]
    public class MoonpoolRotation
    {
        private static bool isRotatingAny = false;

        [HarmonyPatch(typeof(VehicleDockingBay), nameof(VehicleDockingBay.LateUpdate)), HarmonyPrefix]
        public static bool VehicleDockingBay_LateUpdate(VehicleDockingBay __instance)
        {
            if (!Main.Config.EnableFeature || isRotatingAny) return true;

            // Skip if no vehicle or no moonpool anim
            Vehicle dockingVehicle = __instance.interpolatingVehicle;
            if (dockingVehicle == null) return true;

            // Find the moonpool animation transform
            Transform moonpoolAnim = __instance.transform.parent?.Find("moon_pool_anim");
            if (moonpoolAnim == null) return true;

            if (Main.Config.UseAdvancedRotation)
            {
                // Start advanced rotation sequence
                isRotatingAny = true;
                __instance.StartCoroutine(AdvancedRotationSequence(__instance, moonpoolAnim, dockingVehicle.transform));
                return false; // Skip LateUpdate during rotation
            }
            else
            {
                // Original 180-degree flip mode - only if opposite direction
                if (Vector3.Dot(dockingVehicle.transform.right, moonpoolAnim.right) < 0)
                {
                    isRotatingAny = true;
                    __instance.StartCoroutine(RotationSequence(__instance, moonpoolAnim));
                    return false; // Skip LateUpdate during rotation
                }
            }

            return true;
        }

        private static IEnumerator RotationSequence(VehicleDockingBay dockingBay, Transform moonpoolAnim)
        {
            // Store original rotation
            Quaternion originalRotation = moonpoolAnim.rotation;

            // Instant 180-degree rotation
            moonpoolAnim.rotation = originalRotation * Quaternion.Euler(0f, 180f, 0f);

            // Configurable pause before returning
            yield return new WaitForSeconds(Main.Config.WaitBeforeRotation);

            // Smoothly rotate back to original position
            yield return RotateTransform(moonpoolAnim, originalRotation, Main.Config.MaxReturnRotationDuration);

            // Reset state flag
            isRotatingAny = false;
        }

        private static IEnumerator AdvancedRotationSequence(VehicleDockingBay dockingBay, Transform moonpoolAnim, Transform vehicleTransform)
        {
            // Store original rotation
            Quaternion originalRotation = moonpoolAnim.rotation;

            // Calculate target rotation to match vehicle direction
            Vector3 targetForward = new Vector3(vehicleTransform.forward.x, 0, vehicleTransform.forward.z).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(targetForward, Vector3.up);

            // Instantly rotate to match vehicle direction
            moonpoolAnim.rotation = targetRotation;

            // Configurable pause before returning
            yield return new WaitForSeconds(Main.Config.WaitBeforeRotation);

            // Calculate rotation difference for duration
            float angleDifference = Quaternion.Angle(targetRotation, originalRotation);

            // Duration with min and max
            float rotationDuration = Mathf.Max(Main.Config.MinReturnRotationDuration, Mathf.Lerp(0.2f, Main.Config.MaxReturnRotationDuration, angleDifference / 180f));

            // Smoothly rotate back to original position
            yield return RotateTransform(moonpoolAnim, originalRotation, rotationDuration);

            // Reset state flag
            isRotatingAny = false;
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