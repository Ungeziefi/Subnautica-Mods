using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoLowSpeedSplat
    {
        [HarmonyPatch(typeof(CollisionSound), nameof(CollisionSound.OnCollisionEnter)), HarmonyPrefix]
        public static bool CollisionSound_OnCollisionEnter(CollisionSound __instance, Collision col)
        {
            if (!Main.Config.NoLowSpeedSplat) return true;
            if (!__instance.GetComponent<SeaMoth>()) return true;
            if (col.contacts.Length == 0) return false;

            var rootRigidbody = UWE.Utils.GetRootRigidbody(col.gameObject);
            var magnitude = col.relativeVelocity.magnitude;
            FMODAsset asset;

            /* Vanilla behavior:
             * - Small mass (< 10f): Always fish splat sound regardless of object type
             * - Large mass (? 10f):
             *   > 8f: Fast hit sound
             *   > 4f: Medium hit sound
             *   ≤ 4f: Slow hit sound
             * 
             * Fixed behavior:
             * - Small mass (< 10f):
             *   Creatures: Fish splat sound
             *   Other objects: Slow hit sound
             * - Large mass (? 10f): [unchanged]
             *   > 8f: Fast hit sound
             *   > 4f: Medium hit sound
             *   ≤ 4f: Slow hit sound
             */

            // Small mass objects need special handling
            if (rootRigidbody && rootRigidbody.mass < 10f)
            {
                // Only use fish splat for actual creatures
                if (col.gameObject.GetComponent<Creature>())
                    asset = __instance.hitSoundSmall;
                else
                    asset = __instance.hitSoundSlow;
            }

            // Large mass handling remains the same as vanilla
            else if (magnitude > 8f)
                asset = __instance.hitSoundFast;
            else if (magnitude > 4f)
                asset = __instance.hitSoundMedium;
            else
                asset = __instance.hitSoundSlow;

            float volume = Mathf.Clamp01(magnitude / 8f);
            FMODUWE.PlayOneShot(asset, col.contacts[0].point, volume);
            return false;
        }
    }
}