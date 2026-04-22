using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks;

[HarmonyPatch]
public class SeaglideLightAngle
{
    [HarmonyPatch(typeof(Seaglide), nameof(Seaglide.OnDraw))]
    [HarmonyPostfix]
    public static void Seaglide_OnDraw(Seaglide __instance)
    {
        if (Main.Config.SeaglideLightAngle == 0f) return;

        if (__instance.toggleLights == null) return;

        var lightsParent = __instance.gameObject.transform.Find("lights_parent");
        if (lightsParent == null) return;

        lightsParent.localRotation = Quaternion.Euler(-Main.Config.SeaglideLightAngle, 0f, 0f);
    }
}