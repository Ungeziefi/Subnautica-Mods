﻿using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch(typeof(Bench))]
    public class ChairSwivelling
    {
        private static float maxChairRotSpeed = 300f;
        private static float chairRotAcceleration = 80f;
        private static float chairRotDeceleration = 40f;
        private static float currentRotSpeed = 0f;
        // Current direction of rotation: 1 for right, -1 for left, 0 for none
        private static int currentDirection = 0;

        [HarmonyPatch(nameof(Bench.ExitSittingMode)), HarmonyPostfix]
        static void ExitSittingMode(Bench __instance)
        {
            currentRotSpeed = 0f;
            currentDirection = 0;
        }

        [HarmonyPatch(nameof(Bench.OnUpdate)), HarmonyPostfix]
        public static void OnUpdate(Bench __instance)
        {
            if (!Main.Config.ChairSwivelling || CraftData.GetTechType(__instance.gameObject) != TechType.StarshipChair)
            {
                return;
            }

            bool isRotating = false;

            // Rotate chair to the right
            if (GameInput.GetButtonHeld(GameInput.Button.MoveRight))
            {
                if (currentDirection != 1)
                {
                    // Decelerate in the opposite direction first
                    currentRotSpeed = Mathf.Max(currentRotSpeed - chairRotDeceleration * 2 * Time.deltaTime, 0f);
                    if (currentRotSpeed == 0f)
                    {
                        currentDirection = 1;
                    }
                }

                if (currentDirection == 1)
                {
                    currentRotSpeed = Mathf.Min(currentRotSpeed + chairRotAcceleration * Time.deltaTime, maxChairRotSpeed);
                    __instance.transform.Rotate(Vector3.up * currentRotSpeed * Time.deltaTime);
                    isRotating = true;
                }
            }

            // Rotate chair to the left
            else if (GameInput.GetButtonHeld(GameInput.Button.MoveLeft))
            {
                if (currentDirection != -1)
                {
                    // Decelerate in the opposite direction first
                    currentRotSpeed = Mathf.Max(currentRotSpeed - chairRotDeceleration * 2 * Time.deltaTime, 0f);
                    if (currentRotSpeed == 0f)
                    {
                        currentDirection = -1;
                    }
                }

                if (currentDirection == -1)
                {
                    currentRotSpeed = Mathf.Min(currentRotSpeed + chairRotAcceleration * Time.deltaTime, maxChairRotSpeed);
                    __instance.transform.Rotate(-Vector3.up * currentRotSpeed * Time.deltaTime);
                    isRotating = true;
                }
            }

            // Decelerate if there's no input
            if (!isRotating)
            {
                currentRotSpeed = Mathf.Max(currentRotSpeed - chairRotDeceleration * Time.deltaTime, 0f);
                if (currentRotSpeed > 0f)
                {
                    __instance.transform.Rotate(Vector3.up * currentRotSpeed * Time.deltaTime * currentDirection);
                }
                else
                {
                    currentDirection = 0;
                }
            }
        }
    }
}