using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(FleeOnDamage))]
    class NoFleeingToOrigin
    {
        [HarmonyPatch(nameof(FleeOnDamage.OnTakeDamage)), HarmonyPostfix]
        public static void OnTakeDamage(FleeOnDamage __instance, DamageInfo damageInfo)
        {
            if (!Main.Config.NFTODEnableFeature)
            {
                return;
            }

            if (__instance.accumulatedDamage <= __instance.damageThreshold)
            {
                // Main.Logger.LogInfo($"NoFleeingToOrigin: Damage threshold not met - Current: {__instance.accumulatedDamage}, Required: {__instance.damageThreshold}");
                return;
            }

            // Use player position if damage source has none
            Vector3 fleeFromPosition = (damageInfo.position == Vector3.zero)
                ? Player.main.transform.position
                : damageInfo.position;
            // Main.Logger.LogInfo($"NoFleeingToOrigin: Fleeing from position {fleeFromPosition}, Damage source was {(damageInfo.position == Vector3.zero ? "zero (using player)" : "valid")}");

            Vector3 currentPosition = __instance.transform.position;
            // Main.Logger.LogInfo($"NoFleeingToOrigin: Current creature position {currentPosition}");

            // Damage-based distance calculation
            float damageBasedDistance = Mathf.Min(
                damageInfo.damage * Main.Config.DamageToDistanceRatio,
                Main.Config.MaxDamageBasedDistance
            );
            // Main.Logger.LogInfo($"NoFleeingToOrigin: Flee distance calculation - Damage: {damageInfo.damage}, Ratio: {Main.Config.DamageToDistanceRatio}, Result: {damageBasedDistance}");

            // Has issues with zero vectors
            //Vector3 destination = currentPosition +
            //    (currentPosition - fleeFromPosition).normalized *
            //    (__instance.minFleeDistance + damageBasedDistance);

            // Fleeing direction
            Vector3 fleeDirection = (currentPosition - fleeFromPosition);

            // If too close to damage source
            if (fleeDirection.magnitude < 0.001f)
            {
                // Random safe direction
                float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                fleeDirection = new Vector3(
                    Mathf.Cos(randomAngle),  // Random X
                    0.5f,                    // Slight upward movement
                    Mathf.Sin(randomAngle)   // Random Z
                );
            }
            fleeDirection.Normalize();

            // Calculate how far to flee
            float totalFleeDistance = __instance.minFleeDistance + damageBasedDistance;

            // Calculate destination
            Vector3 destination = currentPosition + (fleeDirection * totalFleeDistance);

            // Check if creature has MoveOnSurface component
            bool isLandCreature = __instance.GetComponent<MoveOnSurface>() != null;

            // The destination for land creatures is the same as the y position, for water creatures it's the ocean level
            float yPosition = isLandCreature
                ? destination.y
                : Mathf.Min(destination.y, Ocean.GetOceanLevel());

            // Apply final position with movement type check
            __instance.moveTo = new Vector3(
                destination.x,
                yPosition,
                destination.z
            );
            // Main.Logger.LogInfo($"NoFleeingToOrigin: Final destination {__instance.moveTo}, Is land creature: {isLandCreature}");

            // Update timer
            __instance.timeToFlee = Time.time + __instance.fleeDuration;
            // Main.Logger.LogInfo($"NoFleeingToOrigin: Flee timer set - Duration: {__instance.fleeDuration}, Until: {__instance.timeToFlee}");
        }
    }
}