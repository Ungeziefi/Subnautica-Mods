using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.PDA_Movement_and_Bobbing
{
    [HarmonyPatch]
    public class PDAMovementAndBobbing
    {
        private static Vector3 originalPosition;
        private static float bobTime;

        private static void ApplyBobbing(Player instance)
        {
            // Add the time passed since last frame
            bobTime += Time.deltaTime;

            // Sine between -1 and 1
            // - bobTime * PDABobbingSpeed = frequency
            // - PDABobbingAmount = amplitude
            float bobOffset = Mathf.Sin(bobTime * Main.Config.PDABobbingSpeed) * Main.Config.PDABobbingAmount;

            // New Y position
            Vector3 newPosition = originalPosition + new Vector3(0f, bobOffset, 0f);
            instance.transform.position = newPosition;
        }

        [HarmonyPatch(typeof(Player), nameof(Player.Update)), HarmonyPrefix]
        public static bool Player_Update(Player __instance)
        {
            if (!Player.main.GetPDA().isOpen)
            {
                originalPosition = Vector3.zero;
                bobTime = 0f;
                return true;
            }

            bool isSwimming = Player.main.IsSwimming();

            // Swimming
            if (Main.Config.NoSwimmingInPDA && isSwimming)
            {
                Vector3 currentVelocity = __instance.rigidBody.velocity;
                __instance.rigidBody.velocity = new Vector3(0f, currentVelocity.y, 0f);
                bool shouldBob = Main.Config.PDABobbing && isSwimming;

                if (shouldBob)
                {
                    if (originalPosition == Vector3.zero)
                    {
                        originalPosition = __instance.transform.position;
                    }
                    ApplyBobbing(__instance);
                }
            }

            // Walking
            if (Main.Config.NoWalkingInPDA && !isSwimming)
            {
                Vector3 currentVelocity = __instance.rigidBody.velocity;
                __instance.rigidBody.velocity = new Vector3(0f, currentVelocity.y, 0f);
            }

            return true;
        }

        [HarmonyPatch(typeof(GameInput), nameof(GameInput.GetMoveDirection)), HarmonyPrefix]
        public static bool GameInput_GetMoveDirection(ref Vector3 __result)
        {
            if (Player.main != null && Player.main.GetPDA().isOpen)
            {
                // Block movement input if:
                // swimming disabled and player swimming,
                // walking disabled and player not swimming
                bool isSwimming = Player.main.IsSwimming();
                if ((Main.Config.NoSwimmingInPDA && isSwimming) ||
                    (Main.Config.NoWalkingInPDA && !isSwimming))
                {
                    __result = Vector3.zero;
                    return false;
                }
            }

            return true;
        }
    }
}