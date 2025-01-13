using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Cockpit_Free_Look
{
    [HarmonyPatch]
    public class CockpitFreeLook
    {
        private static bool isLooking = false;
        private static bool isReturning = false;
        private static Vector2 currentRotation = Vector2.zero;
        private static Quaternion originalRotation;
        private static bool wasKeyPressed = false;
        private static float returnTime = 0f;

        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.Update)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Vehicle_Update(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var matcher = new CodeMatcher(instructions, generator);
            matcher.MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Vehicle), "GetPilotingMode")));

            var endLabel = matcher.Advance(2).Instruction.operand;

            matcher.Start().Insert(
                new CodeInstruction(OpCodes.Ldarg_0),
                Transpilers.EmitDelegate(IsInFreeLook),
                new CodeInstruction(OpCodes.Brtrue, endLabel)
            );

            return matcher.InstructionEnumeration();
        }

        private static bool IsInFreeLook(Vehicle vehicle)
        {
            if (isLooking || isReturning)
            {
                // Smoothly interpolate steering values to zero
                float smoothSpeed = Time.deltaTime * (1f / Main.Config.FreeLookReturnDuration) * 2f;

                // Smoothly reduce yaw
                vehicle.steeringWheelYaw = Mathf.Lerp(vehicle.steeringWheelYaw, 0f, smoothSpeed);

                // Smoothly reduce pitch
                vehicle.steeringWheelPitch = Mathf.Lerp(vehicle.steeringWheelPitch, 0f, smoothSpeed);

                // Reset animator values if they exist, using the same smooth interpolation
                if (vehicle.mainAnimator != null)
                {
                    float currentYaw = vehicle.mainAnimator.GetFloat("view_yaw");
                    float currentPitch = vehicle.mainAnimator.GetFloat("view_pitch");

                    vehicle.mainAnimator.SetFloat("view_yaw", Mathf.Lerp(currentYaw, 0f, smoothSpeed));
                    vehicle.mainAnimator.SetFloat("view_pitch", Mathf.Lerp(currentPitch, 0f, smoothSpeed));
                }

                return true;
            }
            return false;
        }

        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.Update)), HarmonyPostfix]
        public static void Vehicle_Update(Vehicle __instance)
        {
            // Check vehicle type and corresponding config
            bool isExosuit = __instance is Exosuit;
            if (isExosuit && !Main.Config.PRAWNEnableFeature) return;
            if (!isExosuit && !Main.Config.SeamothEnableFeature) return;

            if (__instance != Player.main?.currentMountedVehicle) return;

            Transform cameraTransform = MainCamera.camera.transform;
            if (cameraTransform == null) return;

            if (isReturning)
            {
                returnTime += Time.deltaTime;
                float t = returnTime / Main.Config.FreeLookReturnDuration;
                t = Mathf.Sin(t * Mathf.PI * 0.5f);

                cameraTransform.localRotation = Quaternion.Slerp(
                    cameraTransform.localRotation,
                    originalRotation,
                    t
                );

                if (returnTime >= Main.Config.FreeLookReturnDuration)
                {
                    isReturning = false;
                    cameraTransform.localRotation = originalRotation;
                    currentRotation = Vector2.zero;
                }
                return;
            }

            if (!isLooking) return;

            Vector2 lookDelta = GameInput.GetLookDelta() * Main.Config.FreeLookSensitivity * Time.deltaTime * 60f;
            currentRotation.y += lookDelta.x;

            if (isExosuit)
            {
                // PRAWN Suit - horizontal only
                currentRotation.y = Mathf.Clamp(currentRotation.y, -Main.Config.ExosuitAngleLimit, Main.Config.ExosuitAngleLimit);

                Quaternion yawRotation = Quaternion.Euler(0f, currentRotation.y, 0f);
                float tiltAmount = Mathf.Sin(currentRotation.y * Mathf.Deg2Rad) * 5f;
                Quaternion tiltRotation = Quaternion.Euler(0f, 0f, -tiltAmount);

                cameraTransform.localRotation = originalRotation * yawRotation * tiltRotation;
            }
            else
            {
                // Seamoth
                currentRotation.x -= lookDelta.y;
                currentRotation.x = Mathf.Clamp(currentRotation.x, -Main.Config.SeamothVerticalLimit, Main.Config.SeamothVerticalLimit);
                currentRotation.y = Mathf.Clamp(currentRotation.y, -Main.Config.SeamothHorizontalLimit, Main.Config.SeamothHorizontalLimit);

                Quaternion yawRotation = Quaternion.Euler(0f, currentRotation.y, 0f);
                Quaternion pitchRotation = Quaternion.Euler(currentRotation.x, 0f, 0f);
                float tiltAmount = Mathf.Sin(currentRotation.y * Mathf.Deg2Rad) * 5f;
                Quaternion tiltRotation = Quaternion.Euler(0f, 0f, -tiltAmount);

                cameraTransform.localRotation = originalRotation * yawRotation * pitchRotation * tiltRotation;
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.UpdateRotation)), HarmonyPostfix]
        public static void Player_UpdateRotation()
        {
            if (!Main.Config.SeamothEnableFeature && !Main.Config.PRAWNEnableFeature) return;

            Player player = Player.main;
            if (player == null || player.mode != Player.Mode.LockedPiloting) return;

            // Check vehicle type and corresponding config
            Vehicle vehicle = player.currentMountedVehicle;
            if (vehicle == null) return;

            bool isExosuit = vehicle is Exosuit;
            if (isExosuit && !Main.Config.PRAWNEnableFeature) return;
            if (!isExosuit && !Main.Config.SeamothEnableFeature) return;

            // Check both primary and secondary keys
            bool isPrimaryKeyPressed = GameInput.GetKey(Main.Config.FreeLookKey);
            bool isSecondaryKeyPressed = GameInput.GetKey(Main.Config.SecondaryFreeLookKey);
            bool isAnyKeyPressed = isPrimaryKeyPressed || isSecondaryKeyPressed;

            if (isAnyKeyPressed && !wasKeyPressed)
            {
                if (!isLooking)
                {
                    isLooking = true;
                    isReturning = false;
                    originalRotation = MainCamera.camera.transform.localRotation;
                    currentRotation = Vector2.zero;
                }
                else
                {
                    isLooking = false;
                    isReturning = true;
                    returnTime = 0f;
                }
            }
            wasKeyPressed = isAnyKeyPressed;
        }

        [HarmonyPatch(typeof(Player), nameof(Player.ExitLockedMode)), HarmonyPrefix]
        public static void OnExitLockedMode()
        {
            if (isLooking || isReturning)
            {
                isLooking = false;
                isReturning = false;
                if (MainCamera.camera != null)
                {
                    MainCamera.camera.transform.localRotation = originalRotation;
                }
                currentRotation = Vector2.zero;
                Main.Logger.LogInfo("Free look disabled.");
            }
            wasKeyPressed = false;
        }
    }
}