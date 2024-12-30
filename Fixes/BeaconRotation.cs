using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    // Fix beacon rotation when thrown
    [HarmonyPatch(typeof(Beacon))]
    public class FixBeaconRotation
    {
        [HarmonyPatch(nameof(Beacon.Throw))]
        public static void Postfix(Beacon __instance)
        {
            var cameraRotation = Camera.main.transform.rotation;
            __instance.transform.rotation = cameraRotation;
            __instance.transform.Rotate(0f, 180f, 0f);
        }
    }
}