using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Creature_Healthbars
{
    [HarmonyPatch]
    public class Debug
    {
        private static bool freezeActive = false;

        [HarmonyPatch(typeof(Player), nameof(Player.Update)), HarmonyPostfix]
        public static void Player_Update()
        {
            if (!Main.Config.EnableFeature || !Main.Config.EnableFreezeCreaturesToggle) return;

            if (GameInput.GetButtonDown(Main.FreezeCreaturesToggleButton))
            {
                freezeActive = !freezeActive;

                if (freezeActive)
                {
                    SetCreaturesFrozen(true);
                    ErrorMessage.AddMessage("Creatures frozen");
                }
                else
                {
                    SetCreaturesFrozen(false);
                    ErrorMessage.AddMessage("Creatures unfrozen");
                }
            }
        }

        private static void SetCreaturesFrozen(bool freeze)
        {
            Creature[] creatures = Object.FindObjectsOfType<Creature>();

            foreach (Creature creature in creatures)
            {
                Rigidbody rb = creature.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = freeze;

                    if (freeze)
                    {
                        rb.velocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }
                }
            }
        }
    }
}