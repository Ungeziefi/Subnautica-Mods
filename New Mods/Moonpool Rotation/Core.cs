using System.Runtime.CompilerServices;
using DG.Tweening;
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

        if (Main.Config.UseAdvancedRotation)
        {
            Rotate(moonpoolAnim, dockingVehicle.transform, true);
            return false;
        }

        // If facing the other way
        if (Vector3.Dot(dockingVehicle.transform.right, moonpoolAnim.right) < 0)
        {
            Rotate(moonpoolAnim, null, false);
            return false;
        }

        return true;
    }

    private static void Rotate(Transform moonpoolAnim, Transform vehicleTransform, bool isAdvanced)
    {
        isRotatingAny = true;

        var originalLocalEuler = moonpoolAnim.localEulerAngles;
        Quaternion targetRotation;

        if (isAdvanced)
        {
            var targetForward = new Vector3(vehicleTransform.forward.x, 0, vehicleTransform.forward.z).normalized;
            targetRotation = Quaternion.LookRotation(targetForward, Vector3.up);
        }
        else
        {
            targetRotation = moonpoolAnim.rotation * Quaternion.Euler(0f, 180f, 0f);
        }

        moonpoolAnim.rotation = targetRotation;

        // Calculate duration
        var angle = Quaternion.Angle(moonpoolAnim.rotation, Quaternion.Euler(originalLocalEuler));
        var duration = isAdvanced
            ? Mathf.Max(Main.Config.MinReturnRotationDuration,
                Mathf.Lerp(0.2f, Main.Config.MaxReturnRotationDuration, angle / 180f))
            : Main.Config.MaxReturnRotationDuration;

        DOTween.Sequence()
            .AppendInterval(Main.Config.WaitBeforeRotation)
            .Append(moonpoolAnim.DOLocalRotate(originalLocalEuler, duration).SetEase(Ease.InOutQuad))
            .OnComplete(() => { isRotatingAny = false; });
    }
}