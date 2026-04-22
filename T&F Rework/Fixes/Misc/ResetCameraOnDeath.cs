using HarmonyLib;

namespace Ungeziefi.Fixes.Misc;

[HarmonyPatch]
public class ResetCameraOnDeath
{
    [HarmonyPatch(typeof(Player), nameof(Player.ResetPlayerOnDeath))]
    [HarmonyPostfix]
    public static void Player_ResetPlayerOnDeath(Player __instance)
    {
        var camera = MainCameraControl.main;
        camera.ResetCamera();
    }
}