using System.Collections;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Moonpool_Rotation;

[HarmonyPatch]
public class MoonpoolRotation
{
    private static bool isRotatingAny;
    private static readonly ConditionalWeakTable<VehicleDockingBay, Transform> animCache = new();

    [HarmonyPatch(typeof(VehicleDockingBay), nameof(VehicleDockingBay.LateUpdate))]
    [HarmonyPrefix]
    public static bool VehicleDockingBay_LateUpdate(VehicleDockingBay __instance)
    {
        if (!Main.Config.EnableFeature || isRotatingAny) return true;

        var dockingVehicle = __instance.interpolatingVehicle;
        if (dockingVehicle == null) return true;

        if (!animCache.TryGetValue(__instance, out var moonpoolAnim))
        {
            // DockBottom -> Launchbay_cinematic -> moon_pool_anim
            moonpoolAnim = __instance.transform.parent != null
                ? __instance.transform.parent.Find("moon_pool_anim")
                : null;

            animCache.Add(__instance, moonpoolAnim);
        }

        if (moonpoolAnim == null) return true;

        // Match vehicle rotation then flip accordingly
        if (Main.Config.UseAdvancedRotation)
        {
            isRotatingAny = true;
            __instance.StartCoroutine(RotationSequenceWrapper(moonpoolAnim, dockingVehicle.transform, true));
            return false;
        }

        // Flip 180 after automatic alignment to either end
        if (Vector3.Dot(dockingVehicle.transform.right, moonpoolAnim.right) < 0)
        {
            isRotatingAny = true;
            __instance.StartCoroutine(RotationSequenceWrapper(moonpoolAnim, null, false));
            return false;
        }

        return true;
    }

    private static IEnumerator RotationSequenceWrapper(Transform moonpoolAnim, Transform vehicleTransform,
        bool isAdvanced)
    {
        var originalRotation = moonpoolAnim.rotation;
        Quaternion targetRotation;

        if (isAdvanced)
        {
            // Advanced: Match vehicle's forward direction
            var targetForward = new Vector3(vehicleTransform.forward.x, 0, vehicleTransform.forward.z).normalized;
            targetRotation = Quaternion.LookRotation(targetForward, Vector3.up);
        }
        else
        {
            // Simple: 180-degree flip
            targetRotation = originalRotation * Quaternion.Euler(0f, 180f, 0f);
        }

        // 1. Instant jump to target
        moonpoolAnim.rotation = targetRotation;

        // 2. Wait
        yield return new WaitForSeconds(Main.Config.WaitBeforeRotation);

        // 3. Calculate duration for the return
        var angle = Quaternion.Angle(targetRotation, originalRotation);
        var duration = isAdvanced
            ? Mathf.Max(Main.Config.MinReturnRotationDuration,
                Mathf.Lerp(0.2f, Main.Config.MaxReturnRotationDuration, angle / 180f))
            : Main.Config.MaxReturnRotationDuration;

        // 4. Return
        yield return RotateTransform(moonpoolAnim, originalRotation, duration);

        isRotatingAny = false;
    }

    private static IEnumerator RotateTransform(Transform moonpoolAnim, Quaternion targetRotation, float duration)
    {
        float elapsedTime = 0;
        var startRotation = moonpoolAnim.rotation;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            var t = Mathf.SmoothStep(0, 1, elapsedTime / duration);

            moonpoolAnim.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            yield return null;
        }

        moonpoolAnim.rotation = targetRotation;
    }
}