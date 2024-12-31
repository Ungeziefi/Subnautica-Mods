using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    // Swivel chairs can now swivel
    [HarmonyPatch(typeof(Bench))]
    public class TweakChairSwivelling
    {
        private static float maxChairRotSpeed = 100f;
        private static float chairRotAcceleration = 60f;
        private static float chairRotDeceleration = 80f;
        private static float currentRotSpeed = 0f;
        private static Bench swivelChair;
        // Current direction of rotation: 1 for right, -1 for left, 0 for none
        private static int currentDirection = 0;

        [HarmonyPatch(nameof(Bench.EnterSittingMode)), HarmonyPostfix]
        static void EnterSittingMode(Bench __instance)
        {
            var tt = CraftData.GetTechType(__instance.gameObject);
            // Main.Logger.LogInfo("Sitting on " + tt);

            // Check if the bench is a swivel chair
            if (tt == TechType.StarshipChair)
            {
                swivelChair = __instance;
            }
        }

        [HarmonyPatch(nameof(Bench.ExitSittingMode)), HarmonyPostfix]
        static void ExitSittingMode(Bench __instance)
        {
            // Reset rotation speed and direction when exiting the chair
            currentRotSpeed = 0f;
            currentDirection = 0;
        }

        [HarmonyPatch(nameof(Bench.OnUpdate)), HarmonyPrefix]
        public static bool OnUpdate(Bench __instance)
        {
            var tt = CraftData.GetTechType(__instance.gameObject);

            // Exit if no player is sitting on the chair
            if (__instance.currentPlayer == null)
            {
                return false;
            }

            // Handle chair rotation if the player is sitting
            if (__instance.isSitting)
            {
                // Exit if the player is using the PDA
                if (__instance.currentPlayer.GetPDA().isInUse)
                {
                    return false;
                }

                // Handle exiting sitting mode
                if (GameInput.GetButtonDown(GameInput.Button.Exit))
                {
                    __instance.ExitSittingMode(__instance.currentPlayer);
                }

                HandReticle.main.SetText(HandReticle.TextType.Use, "StandUp", true, GameInput.Button.Exit);

                // Handle chair rotation if the swivel tweak is enabled
                if (Language.main.GetCurrentLanguage() == "English" && Main.TweaksConfig.ChairSwivelling && __instance == swivelChair)
                {
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

                    // Decelerate chair rotation if no rotation input
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
            else
            {
                __instance.Subscribe(__instance.currentPlayer, false);
                __instance.currentPlayer = null;
            }
            return false;
        }
    }
}