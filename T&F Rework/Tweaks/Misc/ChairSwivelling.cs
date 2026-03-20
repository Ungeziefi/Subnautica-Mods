using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class ChairSwivelling
    {
        private static Bench currentBench = null;
        private static readonly float maxChairRotSpeed = 300f;
        private static readonly float chairRotAcceleration = 80f;
        private static readonly float chairRotDeceleration = 40f;
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
        static void Bench_EnterSittingMode(Bench __instance) => currentBench = __instance;

        [HarmonyPatch(typeof(Bench), nameof(Bench.ExitSittingMode)), HarmonyPostfix]
        static void Bench_ExitSittingMode(Bench __instance)
        {
            if (currentBench == __instance)
            {
                currentBench = null;
                currentRotSpeed = 0f;
                currentDirection = 0;
            }
        }

        [HarmonyPatch(typeof(Bench), nameof(Bench.OnUpdate)), HarmonyPostfix]
        public static void Bench_OnUpdate(Bench __instance)
        {
            if (!Main.Config.ChairSwivelling ||
                CraftData.GetTechType(__instance.gameObject) != TechType.StarshipChair ||
                currentBench != __instance)
                return;

            bool isRotating = false;

            if (GameInput.moveDirection.x > 0f) // Move to the right
                HandleRotation(ref currentDirection, 1, __instance, ref isRotating);
            else if (GameInput.moveDirection.x < 0f) // Move to the left
                HandleRotation(ref currentDirection, -1, __instance, ref isRotating);

            // Subscript
            string moveRightText = GameInput.FormatButton(GameInput.Button.MoveRight, false);
            string moveLeftText = GameInput.FormatButton(GameInput.Button.MoveLeft, false);
            string gamepadMoveText = GameInput.FormatButton(GameInput.Button.Move, false);
            bool isGamepad = GameInput.IsPrimaryDeviceGamepad();
            HandReticle.main.SetText(HandReticle.TextType.UseSubscript, isGamepad
                ? $"{gamepadMoveText} to swivel chair" // Gamepad
                : $"Hold {moveLeftText} and {moveRightText} to swivel chair", false); // Keyboard

            // Decelerate when no input
            if (!isRotating) DecelerateRotation(__instance);
        }
    }
}