using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(Beacon))]
    public class FixBeaconFacesPlayer
    {
        [HarmonyPatch(nameof(Beacon.Throw)), HarmonyPostfix]
        public static void Throw(Beacon __instance)
        {
            if (Main.FixesConfig.BeaconFacePlayer)
            {
                var cameraRotation = Camera.main.transform.rotation;
                __instance.transform.rotation = cameraRotation;
                __instance.transform.Rotate(0f, 180f, 0f);
            }
        }
    }
}