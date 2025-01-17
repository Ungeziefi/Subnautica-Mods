using UnityEngine;

namespace Ungeziefi.Cockpit_Free_Look
{
    public partial class CockpitFreeLook
    {
        private static bool IsInFreeLook(Vehicle vehicle)
        {
            if (isLooking || isReturning)
            {
                // Transition speed
                float smoothSpeed = Time.deltaTime * (1f / Main.Config.FreeLookReturnDuration);

                vehicle.steeringWheelYaw = Mathf.Lerp(vehicle.steeringWheelYaw, 0f, smoothSpeed);
                vehicle.steeringWheelPitch = Mathf.Lerp(vehicle.steeringWheelPitch, 0f, smoothSpeed);

                if (vehicle.mainAnimator != null)
                {
                    float currentYaw = vehicle.mainAnimator.GetFloat("view_yaw");
                    float currentPitch = vehicle.mainAnimator.GetFloat("view_pitch");

                    // Reset steering wheel rotation
                    vehicle.mainAnimator.SetFloat("view_yaw", Mathf.Lerp(currentYaw, 0f, smoothSpeed));
                    vehicle.mainAnimator.SetFloat("view_pitch", Mathf.Lerp(currentPitch, 0f, smoothSpeed));
                }

                return true;
            }
            return false;
        }
    }
}