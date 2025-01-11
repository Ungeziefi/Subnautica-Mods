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
            if (!Main.Config.SmoothCyclopsWheel)
            {
                return false;
            }

            float steeringWheelYaw = 0f;
            float steeringWheelPitch = 0f;

            // Get yaw and pitch throttle values
            float throttleX = __instance.throttle.x;
            float throttleY = __instance.throttle.y;

            // Handle yaw
            if (Mathf.Abs(throttleX) > 0.0001)
            {
                ShipSide useShipSide;
                if (throttleX > 0)
                {
                    useShipSide = ShipSide.Port;
                    steeringWheelYaw = throttleX;
                }
                else
                {
                    useShipSide = ShipSide.Starboard;
                    steeringWheelYaw = throttleX;
                }

                // Trigger turn handlers if the throttle is significant
                if (throttleX < -0.1 || throttleX > 0.1)
                {
                    for (int index = 0; index < __instance.turnHandlers.Length; ++index)
                        __instance.turnHandlers[index].OnSubTurn(useShipSide);
                }
            }

            // Handle pitch
            if (Mathf.Abs(throttleY) > 0.0001)
            {
                steeringWheelPitch = throttleY;
            }

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