using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Cockpit_Free_Look
{
    [HarmonyPatch]
    public class CockpitFreeLook
    {
        #region Constants and Cached Camera
        private static readonly float SMOOTH_MULTIPLIER = 2f;     // Multiplier for interpolation speed
        private static readonly float TIME_MULTIPLIER = 60f;      // Fixed time multiplier
        private static readonly float TILT_MULTIPLIER = 5f;       // Vehicle tilt effect strength

        private static Camera mainCamera;
        #endregion

        #region State Tracking
        private static bool isLooking = false;
        private static bool isReturning = false;
        private static Vector2 currentRotation = Vector2.zero;
        private static Quaternion originalRotation;
        private static bool wasKeyPressed = false; // Toggle detection
        private static float returnTime = 0f; // Time elapsed during return animation
        #endregion

        #region Vehicle Update Transpiler
        // Prevents rotation input while in free look
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
        #endregion

        #region Steering Wheel Reset
        private static bool IsInFreeLook(Vehicle vehicle)
        {
            if (isLooking || isReturning)
            {
                // Interpolate steering
                float smoothSpeed = Time.deltaTime * (1f / Main.Config.FreeLookReturnDuration) * SMOOTH_MULTIPLIER;
                vehicle.steeringWheelYaw = Mathf.Lerp(vehicle.steeringWheelYaw, 0f, smoothSpeed);
                vehicle.steeringWheelPitch = Mathf.Lerp(vehicle.steeringWheelPitch, 0f, smoothSpeed);

                // Update animator
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
        #endregion

        #region Camera Movement
        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.Update)), HarmonyPostfix]
        public static void Vehicle_Update(Vehicle __instance)
        {
            bool isExosuit = __instance is Exosuit;
            bool isValidVehicle = (isExosuit && Main.Config.PRAWNEnableFeature) ||
                                (!isExosuit && Main.Config.SeamothEnableFeature);
            if (!isValidVehicle) return;

            if (__instance != Player.main?.currentMountedVehicle) return;

            // Cache and validate camera
            if (mainCamera == null)
                mainCamera = MainCamera.camera;
            if (mainCamera == null) return;

            Transform cameraTransform = mainCamera.transform;

            if (isReturning)
            {
                returnTime += Time.deltaTime;
                float t = returnTime / Main.Config.FreeLookReturnDuration;
                t = Mathf.Sin(t * Mathf.PI * 0.5f); // Smooth easing

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

            Vector2 lookDelta = GameInput.GetLookDelta() * Main.Config.FreeLookSensitivity * Time.deltaTime * TIME_MULTIPLIER;
            currentRotation.y += lookDelta.x;

            float tiltAmount = Mathf.Sin(currentRotation.y * Mathf.Deg2Rad) * TILT_MULTIPLIER;
            Quaternion yawRotation = Quaternion.Euler(0f, currentRotation.y, 0f);
            Quaternion tiltRotation = Quaternion.Euler(0f, 0f, -tiltAmount);

            if (isExosuit)
            {
                // PRAWN Suit - horizontal only
                currentRotation.y = Mathf.Clamp(currentRotation.y, -Main.Config.PRAWNAngleLimit, Main.Config.PRAWNAngleLimit);
                cameraTransform.localRotation = originalRotation * yawRotation * tiltRotation;
            }
            else
            {
                // Seamoth
                currentRotation.x -= lookDelta.y;
                currentRotation.x = Mathf.Clamp(currentRotation.x, -Main.Config.SeamothVerticalLimit, Main.Config.SeamothVerticalLimit);
                currentRotation.y = Mathf.Clamp(currentRotation.y, -Main.Config.SeamothHorizontalLimit, Main.Config.SeamothHorizontalLimit);

                Quaternion pitchRotation = Quaternion.Euler(currentRotation.x, 0f, 0f);
                cameraTransform.localRotation = originalRotation * yawRotation * pitchRotation * tiltRotation;
            }
        }
        #endregion

        #region Input Handling
        [HarmonyPatch(typeof(Player), nameof(Player.UpdateRotation)), HarmonyPostfix]
        public static void Player_UpdateRotation()
        {
            if (!Main.Config.SeamothEnableFeature && !Main.Config.PRAWNEnableFeature) return;

            Player player = Player.main;
            if (player == null || player.mode != Player.Mode.LockedPiloting) return;

            Vehicle vehicle = player.currentMountedVehicle;
            if (vehicle == null) return;

            bool isExosuit = vehicle is Exosuit;
            bool isValidVehicle = (isExosuit && Main.Config.PRAWNEnableFeature) ||
                                (!isExosuit && Main.Config.SeamothEnableFeature);
            if (!isValidVehicle) return;

            // Handle both keyboard/mouse and controller inputs
            bool isAnyKeyPressed = GameInput.GetKey(Main.Config.FreeLookKey) ||
                                 GameInput.GetKey(Main.Config.SecondaryFreeLookKey);

            if (isAnyKeyPressed && !wasKeyPressed)
            {
                if (!isLooking)
                {
                    isLooking = true;
                    isReturning = false;
                    originalRotation = mainCamera.transform.localRotation;
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
        #endregion

        #region Cleanup
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
            }
            wasKeyPressed = false;
        }
        #endregion
    }
}