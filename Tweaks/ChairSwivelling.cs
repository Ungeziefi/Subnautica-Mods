using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch(typeof(Bench))]
    public class ChairSwivelling
    {
        private static bool IsSitting = false;
        private static float maxChairRotSpeed = 300f;
        private static float chairRotAcceleration = 80f;
        private static float chairRotDeceleration = 40f;
        private static float currentRotSpeed = 0f;
        // Current direction of rotation: 1 for right, -1 for left, 0 for none
        private static int currentDirection = 0;

        [HarmonyPatch(nameof(Bench.EnterSittingMode)), HarmonyPostfix]
        static void EnterSittingMode(Bench __instance)
        {
            IsSitting = true;
        }

        [HarmonyPatch(nameof(Bench.ExitSittingMode)), HarmonyPostfix]
        static void ExitSittingMode(Bench __instance)
        {
            IsSitting = false;
            currentRotSpeed = 0f;
            currentDirection = 0;
        }

        [HarmonyPatch(nameof(Bench.OnUpdate)), HarmonyPostfix]
        public static void OnUpdate(Bench __instance)
        {
            if (!Main.Config.ChairSwivelling || CraftData.GetTechType(__instance.gameObject) != TechType.StarshipChair || !IsSitting)
            {
                return;
            }

            bool isRotating = false;

            if (GameInput.GetButtonHeld(GameInput.Button.MoveRight))
            {
                HandleRotation(ref currentDirection, 1, __instance, ref isRotating);
            }
            else if (GameInput.GetButtonHeld(GameInput.Button.MoveLeft))
            {
                HandleRotation(ref currentDirection, -1, __instance, ref isRotating);
            }

            if (!isRotating)
            {
                DecelerateRotation(__instance);
            }
        }

        private static void HandleRotation(ref int direction, int targetDirection, Bench instance, ref bool isRotating)
        {
            if (direction != targetDirection)
            {
                currentRotSpeed = Mathf.Max(currentRotSpeed - chairRotDeceleration * 2 * Time.deltaTime, 0f);
                if (currentRotSpeed == 0f)
                {
                    direction = targetDirection;
                }
            }

            if (direction == targetDirection)
            {
                currentRotSpeed = Mathf.Min(currentRotSpeed + chairRotAcceleration * Time.deltaTime, maxChairRotSpeed);
                instance.transform.Rotate(Vector3.up * currentRotSpeed * Time.deltaTime * direction);
                isRotating = true;
            }
        }

        private static void DecelerateRotation(Bench instance)
        {
            currentRotSpeed = Mathf.Max(currentRotSpeed - chairRotDeceleration * Time.deltaTime, 0f);
            if (currentRotSpeed > 0f)
            {
                instance.transform.Rotate(Vector3.up * currentRotSpeed * Time.deltaTime * currentDirection);
            }
            else
            {
                currentDirection = 0;
            }
        }
    }
}