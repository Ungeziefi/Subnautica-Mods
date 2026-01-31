using RootMotion.FinalIK;
using System.Collections.Generic;
using UnityEngine;

namespace Ungeziefi.Cockpit_Free_Look
{
    public partial class CockpitFreeLook
    {
        private static List<AimIK> cachedAimIKComponents; // For the arms

        // State tracking
        private static bool isLooking = false;
        private static bool isReturning = false;
        private static Vector2 currentRotation = Vector2.zero;
        private static Quaternion originalRotation;
        private static float returnTime = 0f; // Time elapsed during return animation
    }
}