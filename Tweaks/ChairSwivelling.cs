using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    public class TweakChairSwivelling
    {
        // Swivel chairs can now swivel
        [HarmonyPatch(typeof(Bench))]
        public class TweakBenchSwivel
        {
            private static float maxChairRotSpeed = 100f;
            private static float chairRotAcceleration = 40f;
            private static float chairRotDeceleration = 10f;
            private static float currentRotSpeed = 0f;
            private static Bench swivelChair;
            // Current direction of rotation: 1 for right, -1 for left, 0 for none
            private static int currentDirection = 0;

            [HarmonyPatch(nameof(Bench.EnterSittingMode))]
            static void Postfix(Bench __instance)
            {
                var tt = CraftData.GetTechType(__instance.gameObject);
                // Main.Logger.LogInfo("Sitting on " + tt);

                // Check if the bench is a swivel chair
                if (tt == TechType.StarshipChair)
                {
                    swivelChair = __instance;
                }
            }

            [HarmonyPatch(nameof(Bench.OnUpdate))]
            public static bool Prefix(Bench __instance)
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
                    if (Language.main.GetCurrentLanguage() == "English" && Main.Config.ChairSwivelling && __instance == swivelChair)
                    {
                        // Rotate chair to the right
                        if (GameInput.GetButtonHeld(GameInput.Button.MoveRight))
                        {
                            if (currentDirection != 1)
                            {
                                currentRotSpeed = 0f;
                                currentDirection = 1;
                            }
                            currentRotSpeed = Mathf.Min(currentRotSpeed + chairRotAcceleration * Time.deltaTime, maxChairRotSpeed);
                            __instance.transform.Rotate(Vector3.up * currentRotSpeed * Time.deltaTime);
                        }
                        // Rotate chair to the left
                        else if (GameInput.GetButtonHeld(GameInput.Button.MoveLeft))
                        {
                            if (currentDirection != -1)
                            {
                                currentRotSpeed = 0f;
                                currentDirection = -1;
                            }
                            currentRotSpeed = Mathf.Min(currentRotSpeed + chairRotAcceleration * Time.deltaTime, maxChairRotSpeed);
                            __instance.transform.Rotate(-Vector3.up * currentRotSpeed * Time.deltaTime);
                        }
                        // Decelerate chair rotation
                        else
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
}
