using System.Collections.Generic;
using UnityEngine;

namespace Ungeziefi.Better_Scanner_Blips_Remake
{
    public partial class BetterScannerBlipsRemake
    {
        internal static List<(ResourceTrackerDatabase.ResourceInfo primaryResource, int count, float distance, int blipIndex)>
            GroupResources(List<(ResourceTrackerDatabase.ResourceInfo resource, float distance, int blipIndex)> resources)
        {
            var result = new List<(ResourceTrackerDatabase.ResourceInfo, int, float, int)>();
            var processedIndices = new HashSet<int>();

            // GroupingDistance is squared to avoid doing square root calculations repeatedly
            float groupingDistanceSquared = Main.Config.GroupingDistance * Main.Config.GroupingDistance;

            // Player position (for group breaking)
            Vector3 playerPosition = Player.main.transform.position;
            bool shouldBreakGroups = Main.Config.GroupNearbyResources && Main.Config.BreakGroupsWhenNearby;
            float breakDistanceSquared = Main.Config.GroupBreakingDistance * Main.Config.GroupBreakingDistance;

            for (int i = 0; i < resources.Count; i++)
            {
                // Skip resources already processed as part of a group
                if (processedIndices.Contains(i)) continue;

                var primaryResource = resources[i];
                int count = 1;
                processedIndices.Add(i);

                // Check if too close to player and groups should be broken
                bool tooCloseToPlayer = false;
                if (shouldBreakGroups)
                {
                    float distanceToPlayerSquared = Vector3.SqrMagnitude(primaryResource.resource.position - playerPosition);
                    tooCloseToPlayer = distanceToPlayerSquared <= breakDistanceSquared;
                }

                // Don't group if too close to player
                if (tooCloseToPlayer)
                {
                    result.Add((primaryResource.resource, 1, primaryResource.distance, primaryResource.blipIndex));
                    continue;
                }

                // Find other resources of the same type within the grouping distance
                for (int j = i + 1; j < resources.Count; j++)
                {
                    if (processedIndices.Contains(j)) continue;

                    var otherResource = resources[j];

                    // Check if the resources are of the same type
                    if (primaryResource.resource.techType == otherResource.resource.techType)
                    {
                        // Calculate squared distance between resources to avoid sqrt
                        float distSquared = Vector3.SqrMagnitude(
                            primaryResource.resource.position - otherResource.resource.position);

                        // If the distance is less than the grouping distance, add to group
                        if (distSquared <= groupingDistanceSquared)
                        {
                            // If we should break groups near player, check if this resource is too close
                            if (shouldBreakGroups)
                            {
                                float otherDistToPlayerSquared = Vector3.SqrMagnitude(otherResource.resource.position - playerPosition);
                                bool otherTooClose = otherDistToPlayerSquared <= breakDistanceSquared;

                                // If this resource is too close to player, don't group it
                                if (otherTooClose)
                                {
                                    // Add this resource individually later (don't mark as processed)
                                    continue;
                                }
                            }

                            count++;
                            processedIndices.Add(j);
                        }
                    }
                }

                // Add the resource or resource group to the result list
                result.Add((primaryResource.resource, count, primaryResource.distance, primaryResource.blipIndex));
            }

            // Add any remaining resources that weren't processed (they were skipped due to being near the player)
            for (int i = 0; i < resources.Count; i++)
            {
                if (!processedIndices.Contains(i))
                {
                    var resource = resources[i];
                    result.Add((resource.resource, 1, resource.distance, resource.blipIndex));
                }
            }

            return result;
        }
    }
}