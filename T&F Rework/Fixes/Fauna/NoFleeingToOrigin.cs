using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes.Fauna;

[HarmonyPatch]
internal class NoFleeingToOrigin
{
    [HarmonyPatch(typeof(FleeOnDamage), nameof(FleeOnDamage.OnTakeDamage))]
    [HarmonyPostfix]
    public static void FleeOnDamage_OnTakeDamage(FleeOnDamage __instance, DamageInfo damageInfo)
    {
        if (!Main.Config.NFTOEnableFeature) return;

        if (__instance.accumulatedDamage <= __instance.damageThreshold) return;

        // Use player position if damage source has none
        var fleeFromPosition = damageInfo.position == Vector3.zero
            ? Player.main.transform.position
            : damageInfo.position;

        var currentPosition = __instance.transform.position;

        // Damage-based distance calculation
        var damageBasedDistance = Mathf.Min(
            damageInfo.damage * Main.Config.DamageToDistanceRatio,
            Main.Config.MaxDamageBasedDistance
        );

        // Fleeing direction
        var fleeDirection = currentPosition - fleeFromPosition;

        // If too close to damage source
        if (fleeDirection.magnitude < 0.001f)
        {
            // Random safe direction
            var randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            fleeDirection = new Vector3(
                Mathf.Cos(randomAngle), // Random X
                0.5f, // Slight upward movement
                Mathf.Sin(randomAngle) // Random Z
            );
        }

        fleeDirection.Normalize();

        // Calculate how far to flee
        var totalFleeDistance = __instance.minFleeDistance + damageBasedDistance;

        // Calculate destination
        var destination = currentPosition + fleeDirection * totalFleeDistance;

        // Land creature check and ocean clamp
        var isLandCreature = __instance.GetComponent<MoveOnSurface>() != null;
        var yPosition = isLandCreature
            ? destination.y
            : Mathf.Min(destination.y, Ocean.GetOceanLevel());

        // Apply final position
        __instance.moveTo = new Vector3(
            destination.x,
            yPosition,
            destination.z
        );

        // Update timer
        __instance.timeToFlee = Time.time + __instance.fleeDuration;
    }
}