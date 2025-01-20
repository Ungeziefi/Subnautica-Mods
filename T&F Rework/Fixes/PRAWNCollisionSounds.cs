using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class PRAWNCollisionSounds
    {
        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.Start)), HarmonyPostfix]
        public static void Exosuit_Start(Exosuit __instance)
        {
            if (!Main.Config.PRAWNCollisionSounds) return;

            // Check CollisionSound component
            CollisionSound collisionSound = __instance.gameObject.EnsureComponent<CollisionSound>();

            // Create and assign FMOD assets
            collisionSound.hitSoundSmall = CreateFMODAsset("event:/sub/common/fishsplat", "{0e47f1c6-6178-41bd-93bf-40bfca179cb6}");
            collisionSound.hitSoundFast = CreateFMODAsset("event:/sub/seamoth/impact_solid_hard", "{ed65a390-2e80-4005-b31b-56380500df33}");
            collisionSound.hitSoundMedium = CreateFMODAsset("event:/sub/seamoth/impact_solid_medium", "{cb2927bf-3f8d-45d8-afe2-c82128f39062}");
            collisionSound.hitSoundSlow = CreateFMODAsset("event:/sub/seamoth/impact_solid_soft", "{15dc7344-7b0a-4ffd-9b5c-c40f923e4f4d}");

            Main.Logger.LogInfo("PRAWN Suit collision sounds set up.");
        }

        private static FMODAsset CreateFMODAsset(string path, string id)
        {
            FMODAsset asset = ScriptableObject.CreateInstance<FMODAsset>();
            asset.path = path;
            asset.id = id;
            return asset;
        }

        [HarmonyPatch(typeof(CollisionSound), nameof(CollisionSound.OnCollisionEnter)), HarmonyPrefix]
        public static bool CollisionSound_OnCollisionEnter(CollisionSound __instance, Collision col)
        {
            if (!Main.Config.PRAWNCollisionSounds) return true;

            // Only handle PRAWN Suit collisions
            if (__instance.gameObject.GetComponent<Exosuit>() != null)
            {
                // No sounds when colliding with static objects (like terrain)
                if (col.rigidbody == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
