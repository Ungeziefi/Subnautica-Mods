using System.Collections.Generic;
using HarmonyLib;
using RootMotion.FinalIK;
using UnityEngine;

namespace Ungeziefi.Cockpit_Free_Look
{
    [HarmonyPatch]
    public partial class CockpitFreeLook
    {
        // Caches
        private static Camera mainCamera;
        private static List<AimIK> cachedAimIKComponents; // For the arms

        // State tracking
        private static bool isLooking = false;
        private static bool isReturning = false;
        private static Vector2 currentRotation = Vector2.zero;
        private static Quaternion originalRotation;
        private static bool wasKeyPressed = false; // Toggle detection
        private static float returnTime = 0f; // Time elapsed during return animation
    }
}