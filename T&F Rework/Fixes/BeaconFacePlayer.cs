using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class BeaconFacePlayer
    {
        [HarmonyPatch(typeof(Beacon), nameof(Beacon.Throw)), HarmonyPostfix]
        public static void Beacon_Throw(Beacon __instance)
        {
            if (!Main.Config.BeaconFacePlayer) return;

            var cameraRotation = Camera.main.transform.rotation;
            __instance.transform.rotation = cameraRotation;
            __instance.transform.Rotate(0f, 180f, 0f);
        }
    }
}