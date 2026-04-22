using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks;

[HarmonyPatch]
public class MoveableMobileVehicleBay
{
    [HarmonyPatch(typeof(Constructor), nameof(Constructor.OnEnable))]
    [HarmonyPostfix]
    public static void Constructor_OnEnable(Constructor __instance)
    {
        if (!Main.Config.MoveableMobileVehicleBay) return;

        var immuneComponent = __instance.GetComponent<ImmuneToPropulsioncannon>();
        if (immuneComponent) Object.Destroy(immuneComponent);
    }
}