using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.PDA_Movement_and_Bobbing
{
    [HarmonyPatch]
    public class Core
    {
        private static Vector3 originalPosition;
        private static float bobTime;

        private static void ApplyBobbing(Player instance)
        {
            bobTime += Time.deltaTime;
            float bobOffset = Mathf.Sin(bobTime * Main.Config.PDABobbingSpeed) * Main.Config.PDABobbingAmount;
            instance.transform.position = originalPosition + new Vector3(0f, bobOffset, 0f);
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

            // Movement blocking
            bool isSwimming = Player.main.IsSwimming();
            if ((Main.Config.NoSwimmingInPDA && isSwimming) || (Main.Config.NoWalkingInPDA && !isSwimming))
            {
                __instance.rigidBody.velocity = new Vector3(0f, __instance.rigidBody.velocity.y, 0f);
                if (Main.Config.PDABobbing && isSwimming)
                {
                    if (originalPosition == Vector3.zero)
                        originalPosition = __instance.transform.position;
                    ApplyBobbing(__instance);
                }
            }

            return true;
        }

        [HarmonyPatch(typeof(GameInput), nameof(GameInput.GetMoveDirection)), HarmonyPrefix]
        public static bool GameInput_GetMoveDirection(ref Vector3 __result)
        {
            if (Player.main != null && Player.main.GetPDA().isOpen)
            {
                bool isSwimming = Player.main.IsSwimming();
                if ((Main.Config.NoSwimmingInPDA && isSwimming) || (Main.Config.NoWalkingInPDA && !isSwimming))
                {
                    __result = Vector3.zero;
                    return false;
                }
            }

            return true;
        }
    }
}