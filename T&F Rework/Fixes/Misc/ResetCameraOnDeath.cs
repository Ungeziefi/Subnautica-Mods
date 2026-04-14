using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes.Misc
{
    [HarmonyPatch]
    public class ResetCameraOnDeath
    {
        [HarmonyPatch(typeof(Player), nameof(Player.ResetPlayerOnDeath)), HarmonyPostfix]
        public static void Player_ResetPlayerOnDeath(Player __instance)
        {
            MainCameraControl camera = MainCameraControl.main;
            camera.ResetCamera();
        }
    }
}