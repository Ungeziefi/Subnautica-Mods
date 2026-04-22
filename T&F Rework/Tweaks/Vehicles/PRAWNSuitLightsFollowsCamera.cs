using HarmonyLib;

namespace Ungeziefi.Tweaks.Vehicles;

[HarmonyPatch]
public class PRAWNSuitLightsFollowsCamera
{
    [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.Start))]
    [HarmonyPostfix]
    public static void Exosuit_Start(Exosuit __instance)
    {
        if (!Main.Config.PRAWNSuitLightsFollowCamera) return;

        var lightsParent = __instance.transform.Find("lights_parent");
        if (lightsParent != null) lightsParent.SetParent(__instance.leftArmAttach);
    }
}