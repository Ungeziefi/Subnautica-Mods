using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class SmoothCyclopsWheel
    {
        [HarmonyPatch(typeof(SubControl), nameof(SubControl.UpdateAnimation)), HarmonyPrefix]
        public static bool SubControl_UpdateAnimation(SubControl __instance)
        {
            if (!Main.Config.SmoothCyclopsWheel) return false;

            float steeringWheelYaw = 0f, steeringWheelPitch = 0f;
            float throttleX = __instance.throttle.x, throttleY = __instance.throttle.y;

            // Handle yaw
            if (Mathf.Abs(throttleX) > 0.0001)
            {
                ShipSide useShipSide = throttleX > 0 ? ShipSide.Port : ShipSide.Starboard;
                steeringWheelYaw = throttleX;

                if (Mathf.Abs(throttleX) > 0.1)
                {
                    foreach (var handler in __instance.turnHandlers)
                        handler.OnSubTurn(useShipSide);
                }
            }

            // Handle pitch
            if (Mathf.Abs(throttleY) > 0.0001)
                steeringWheelPitch = throttleY;

            // Interpolate yaw and pitch
            __instance.steeringWheelYaw = Mathf.Lerp(__instance.steeringWheelYaw, steeringWheelYaw, Time.deltaTime * __instance.steeringReponsiveness);
            __instance.steeringWheelPitch = Mathf.Lerp(__instance.steeringWheelPitch, steeringWheelPitch, Time.deltaTime * __instance.steeringReponsiveness);

            // Update animator
            if (__instance.mainAnimator)
            {
                __instance.mainAnimator.SetFloat("view_yaw", __instance.steeringWheelYaw * 100f);
                __instance.mainAnimator.SetFloat("view_pitch", __instance.steeringWheelPitch * 100f);
            }

            return false;
        }
    }
}