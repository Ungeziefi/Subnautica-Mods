﻿using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(SubControl))]
    public class SmoothCyclopsWheel
    {
        [HarmonyPatch(nameof(SubControl.UpdateAnimation)), HarmonyPrefix]
        public static bool UpdateAnimation(SubControl __instance)
        {
            if (!Main.Config.SmoothCyclopsWheel)
            {
                return false;
            }

            float steeringWheelYaw = 0f;
            float steeringWheelPitch = 0f;

            // Get the throttle values for yaw and pitch
            float throttleX = __instance.throttle.x;
            float throttleY = __instance.throttle.y;

            // Handle yaw (left/right steering)
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

            // Handle pitch (up/down steering)
            if (Mathf.Abs(throttleY) > 0.0001)
            {
                steeringWheelPitch = throttleY;
            }

            // Smoothly interpolate the steering wheel's yaw and pitch
            __instance.steeringWheelYaw = Mathf.Lerp(__instance.steeringWheelYaw, steeringWheelYaw, Time.deltaTime * __instance.steeringReponsiveness);
            __instance.steeringWheelPitch = Mathf.Lerp(__instance.steeringWheelPitch, steeringWheelPitch, Time.deltaTime * __instance.steeringReponsiveness);

            // Update the animator with the new yaw and pitch values
            if (__instance.mainAnimator)
            {
                __instance.mainAnimator.SetFloat("view_yaw", __instance.steeringWheelYaw * 100f);
                __instance.mainAnimator.SetFloat("view_pitch", __instance.steeringWheelPitch * 100f);
            }

            return false;
        }
    }
}