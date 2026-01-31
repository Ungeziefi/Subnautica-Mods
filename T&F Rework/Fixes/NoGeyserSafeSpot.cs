using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoGeyserSafeSpot
    {
        private static readonly HashSet<(int x, int z)> FixedGeyserPositions = new()
        {
            (961, 470),
            (965, 625),
            (-175, 1024),
            (-153, 956),
            (-70, 1024),
            (-67, 1066),
            (-80, 968),
            (-78, 930),
            (-32, 973)
        };

        private const float OriginalColliderHeight = 24f;
        private const float FixedColliderHeight = 30f;

        [HarmonyPatch(typeof(Geyser), nameof(Geyser.Start)), HarmonyPrefix]
        public static void Geyser_Start(Geyser __instance)
        {
            var position = __instance.transform.position;
            var coords = ((int)position.x, (int)position.z);

            if (FixedGeyserPositions.Contains(coords))
            {
                var capsuleCollider = __instance.GetComponent<CapsuleCollider>();
                if (capsuleCollider != null)
                {
                    capsuleCollider.height = FixedColliderHeight;
                }
            }
        }
    }
}