using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks;

[HarmonyPatch]
public class MobileVehicleBayBeacon
{
    [HarmonyPatch(typeof(Constructor), nameof(Constructor.OnEnable))]
    [HarmonyPostfix]
    public static void Constructor_OnEnable(Constructor __instance)
    {
        if (!Main.Config.MobileVehicleBayBeacon) return;

        var pi = __instance.gameObject.EnsureComponent<PingInstance>();
        pi.pingType = PingType.Signal;
        pi.origin = __instance.transform;
        pi.SetLabel(Language.main.Get("Constructor"));
    }

    [HarmonyPatch(typeof(PingInstance), nameof(PingInstance.Initialize))]
    [HarmonyPostfix]
    public static void PingInstance_Initialize(PingInstance __instance)
    {
        var constructor = __instance.GetComponent<Constructor>();
        if (constructor == null) return;

        if (!Main.Config.MobileVehicleBayBeacon) Object.Destroy(__instance);
    }
}