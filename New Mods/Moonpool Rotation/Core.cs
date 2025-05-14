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
        private static Transform launchbayCinematicCache;
        private static bool isRotating = false;

        [HarmonyPatch(typeof(VehicleDockingBay), nameof(VehicleDockingBay.Start)), HarmonyPostfix]
        public static void VehicleDockingBay_Start(VehicleDockingBay __instance)
        {
            if (coroutineHost == null)
            {
                coroutineHost = __instance;
            }

            Transform moonpoolAnim = __instance.transform.Find("moonpool_anim");
            if (moonpoolAnim != null)
            {
                moonpoolAnimCache = moonpoolAnim;

                // Look for launchbay_cinematic
                Transform launchbayCinematic = __instance.transform.Find("launchbay_cinematic");
                if (launchbayCinematic != null)
                {
                    launchbayCinematicCache = launchbayCinematic;
                }
            }
        }

        private static void EnsureLaunchPositionsMatchMoonpoolOrientation(Transform parent, Transform moonpoolAnim)
        {
            if (parent == null || moonpoolAnim == null) return;

            Transform launchFromLeft = parent.Find("LaunchFromLeft");
            Transform launchFromRight = parent.Find("LaunchFromRight");

            if (launchFromLeft == null || launchFromRight == null) return;
            
            // Get expected positions
            Vector3 moonpoolRight = moonpoolAnim.right;
            Vector3 expectedLeftPos = -moonpoolRight * 1.5f;
            Vector3 expectedRightPos = moonpoolRight * 1.5f;

            expectedLeftPos.y = launchFromLeft.localPosition.y;
            expectedLeftPos.z = launchFromLeft.localPosition.z;
            expectedRightPos.y = launchFromRight.localPosition.y;
            expectedRightPos.z = launchFromRight.localPosition.z;

            // Update positions
            launchFromLeft.localPosition = expectedLeftPos;
            launchFromRight.localPosition = expectedRightPos;
        }

        [HarmonyPatch(typeof(VehicleDockingBay), nameof(VehicleDockingBay.LateUpdate)), HarmonyPrefix]
        public static bool VehicleDockingBay_LateUpdate(VehicleDockingBay __instance)
        {
            if (!Main.Config.EnableFeature || moonpoolAnimCache == null) return true;

            Vehicle dockingVehicle = __instance.interpolatingVehicle;

            if (dockingVehicle != null && !isRotating)
            {
                // Always use launchbay_cinematic as reference if available
                if (launchbayCinematicCache != null)
                {
                    // Compare vehicle direction with launchbay_cinematic
                    float dotProduct = Vector3.Dot(dockingVehicle.transform.right, launchbayCinematicCache.right);

                    // If opposite direction (dot product < 0)
                    if (dotProduct < 0)
                    {
                        coroutineHost.StartCoroutine(RotationSequence(moonpoolAnimCache, dockingVehicle.transform, false));
                        return false; // Skip LateUpdate during rotation
                    }
                    // If same direction (dot product > 0)
                    else if (dotProduct > 0)
                    {
                        coroutineHost.StartCoroutine(RotationSequence(moonpoolAnimCache, dockingVehicle.transform, true));
                        return false; // Skip LateUpdate during rotation
                    }
                }
                else
                {
                    // Fallback
                    float dotProduct = Vector3.Dot(dockingVehicle.transform.right, moonpoolAnimCache.right);

                    if (dotProduct < 0)
                    {
                        coroutineHost.StartCoroutine(RotationSequence(moonpoolAnimCache, dockingVehicle.transform, false));
                        return false;
                    }
                    else if (dotProduct > 0)
                    {
                        coroutineHost.StartCoroutine(RotationSequence(moonpoolAnimCache, dockingVehicle.transform, true));
                        return false;
                    }
                }
            }

            return true;
        }

        private static IEnumerator RotationSequence(Transform moonpoolAnim, Transform vehicleTransform, bool sameDirection)
        {
            isRotating = true;

            // Store original rotation for return
            Quaternion originalRotation = moonpoolAnim.rotation;

            // Calculate target rotation to match vehicle orientation
            Vector3 vehicleForward = vehicleTransform.forward;
            vehicleForward.y = 0;
            vehicleForward.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(vehicleForward, Vector3.up);

            // Initial instant rotation
            moonpoolAnim.rotation = targetRotation;

            // Pause before returning
            yield return new WaitForSeconds(Main.Config.WaitBeforeRotation);

            Quaternion returnRotation;
            bool needRotation = false;

            if (launchbayCinematicCache != null)
            {
                // Use dot product relative to launchbay_cinematic
                float dotWithCinematic = Vector3.Dot(vehicleTransform.right, launchbayCinematicCache.right);
                needRotation = dotWithCinematic < 0;
            }
            else
            {
                // Fallback
                needRotation = !sameDirection;
            }

            if (needRotation)
            {
                // Need 180 degree rotation
                returnRotation = originalRotation * Quaternion.Euler(0, 180, 0);
            }
            else
            {
                // Return to original orientation
                returnRotation = originalRotation;
            }

            // Smoothly rotate to the chosen return position
            yield return RotateTransform(moonpoolAnim, returnRotation, Main.Config.ReturnRotationDuration);

            // Update launch positions to match the new orientation
            Transform launchbayParent = moonpoolAnim.parent;
            if (launchbayParent != null)
            {
                EnsureLaunchPositionsMatchMoonpoolOrientation(launchbayParent, moonpoolAnim);
            }

            isRotating = false;
        }

        private static IEnumerator RotateTransform(Transform moonpoolAnim, Quaternion targetRotation, float duration)
        {
            float elapsedTime = 0;
            Quaternion startRotation = moonpoolAnim.rotation;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsedTime / duration);
                moonpoolAnim.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
                yield return null;
            }
        }
    }
}