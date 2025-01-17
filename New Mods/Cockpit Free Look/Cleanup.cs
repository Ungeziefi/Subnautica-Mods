using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Cockpit_Free_Look
{
    [HarmonyPatch]
    public partial class CockpitFreeLook
    {
        [HarmonyPatch(typeof(Player), nameof(Player.ExitLockedMode)), HarmonyPrefix]
        public static void OnExitLockedMode()
        {
            if (isLooking || isReturning)
            {
                isLooking = false;
                isReturning = false;

                if (mainCamera != null)
                {
                    mainCamera.transform.localRotation = originalRotation;
                }

                currentRotation = Vector2.zero;

                if (Player.main?.currentMountedVehicle is Exosuit exosuit)
                {
                    EnableExosuitArms(exosuit);
                }
            }

            wasKeyPressed = false;
            cachedAimIKComponents = null;
        }
    }
}