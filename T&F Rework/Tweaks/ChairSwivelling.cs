﻿using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class ChairSwivelling
    {
        private static bool IsSitting = false;
        private static float maxChairRotSpeed = 300f;
        private static float chairRotAcceleration = 80f;
        private static float chairRotDeceleration = 40f;
        private static float currentRotSpeed = 0f;
        private static int currentDirection = 0; // 1 = right, -1 = left

        // Accelerate
        private static void HandleRotation(ref int direction, int targetDirection, Bench instance, ref bool isRotating)
        {
            // If current direction is not target, decelerate
            if (direction != targetDirection)
            {
                currentRotSpeed = Mathf.Max(currentRotSpeed - chairRotDeceleration * 2 * Time.deltaTime, 0f);
                if (currentRotSpeed == 0f) direction = targetDirection;
            }

            // If current direction is target, accelerate
            if (direction == targetDirection)
            {
                currentRotSpeed = Mathf.Min(currentRotSpeed + chairRotAcceleration * Time.deltaTime, maxChairRotSpeed);
                instance.transform.Rotate(Vector3.up * currentRotSpeed * Time.deltaTime * direction);
                isRotating = true;
            }
        }

        // Decelerate
        private static void DecelerateRotation(Bench instance)
        {
            currentRotSpeed = Mathf.Max(currentRotSpeed - chairRotDeceleration * Time.deltaTime, 0f);
            if (currentRotSpeed > 0f)
                instance.transform.Rotate(Vector3.up * currentRotSpeed * Time.deltaTime * currentDirection);
            else
                currentDirection = 0;
        }

        [HarmonyPatch(typeof(Bench), nameof(Bench.EnterSittingMode)), HarmonyPostfix]
        static void Bench_EnterSittingMode(Bench __instance) => IsSitting = true;

        [HarmonyPatch(typeof(Bench), nameof(Bench.ExitSittingMode)), HarmonyPostfix]
        static void Bench_ExitSittingMode(Bench __instance)
        {
            IsSitting = false;
            currentRotSpeed = 0f;
            currentDirection = 0;
        }

        [HarmonyPatch(typeof(Bench), nameof(Bench.OnUpdate)), HarmonyPostfix]
        public static void Bench_OnUpdate(Bench __instance)
        {
            if (!Main.Config.ChairSwivelling ||
                CraftData.GetTechType(__instance.gameObject) != TechType.StarshipChair ||
                !IsSitting)
                return;

            bool isRotating = false;

            // Rotate based on input
            if (GameInput.GetButtonHeld(GameInput.Button.MoveRight))
                HandleRotation(ref currentDirection, 1, __instance, ref isRotating);
            else if (GameInput.GetButtonHeld(GameInput.Button.MoveLeft))
                HandleRotation(ref currentDirection, -1, __instance, ref isRotating);

            // Decelerate when no input
            if (!isRotating) DecelerateRotation(__instance);
        }
    }
}