using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    // This class handles the poison damage restoration fix
    [HarmonyPatch]
    public class PoisonHealthFix_Patch
    {
        // Patch the LiveMixin.Start method to reset tempDamage when the game loads
        [HarmonyPatch(typeof(LiveMixin), "Start")]
        [HarmonyPostfix]
        public static void StartPostfix(LiveMixin __instance)
        {
            // Only apply to the player's LiveMixin component
            if (__instance.gameObject == Player.mainObject)
            {
                // Reset temporary damage that might have been caused by poison
                __instance.tempDamage = 0f;
                // Ensure the health state is properly synchronized
                __instance.SyncUpdatingState();
            }
        }

        // Patch the LiveMixin.TakeDamage method to properly handle poison damage
        [HarmonyPatch(typeof(LiveMixin), "TakeDamage")]
        [HarmonyPrefix]
        public static bool TakeDamagePrefix(LiveMixin __instance, ref bool __result, float originalDamage,
            Vector3 position = default(Vector3), DamageType type = DamageType.Normal, GameObject dealer = null)
        {
            // Only handle poison damage for the player
            if (type == DamageType.Poison && __instance.gameObject == Player.mainObject)
            {
                // If the damage is from the temporary damage system (self-inflicted poison tick)
                if (dealer == __instance.gameObject && position == Vector3.one)
                {
                    // Apply the damage directly to health
                    float damage = DamageSystem.CalculateDamage(originalDamage, type, __instance.gameObject, dealer);
                    __instance.health = Mathf.Max(0f, __instance.health - damage);
                }
                else
                {
                    // For new poison applications, add to temporary damage
                    float damage = DamageSystem.CalculateDamage(originalDamage, type, __instance.gameObject, dealer);
                    __instance.tempDamage += damage;
                    __instance.SyncUpdatingState();
                }

                // Skip the original method
                __result = false;
                return false;
            }

            // Continue with original method for non-poison damage
            return true;
        }
    }
}