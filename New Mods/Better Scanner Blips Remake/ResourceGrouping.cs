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

            float groupingDistanceSquared = Main.Config.GroupingDistance * Main.Config.GroupingDistance;
            float breakDistanceSquared = Main.Config.GroupBreakingDistance * Main.Config.GroupBreakingDistance;

            Vector3 playerPosition = Player.main.transform.position;
            bool shouldBreakGroups = Main.Config.GroupNearbyResources && Main.Config.BreakGroupsWhenNearby;

            for (int i = 0; i < resources.Count; i++)
            {
                if (processedIndices.Contains(i)) continue;

                var currentResource = resources[i];
                processedIndices.Add(i);

                // Check if near player (for group breaking)
                bool tooCloseToPlayer = false;
                if (shouldBreakGroups)
                {
                    float distanceToPlayerSquared = Vector3.SqrMagnitude(currentResource.resource.position - playerPosition);
                    tooCloseToPlayer = distanceToPlayerSquared <= breakDistanceSquared;
                }

                // Add individual resource if too close to player
                if (tooCloseToPlayer)
                {
                    result.Add((currentResource.resource, 1, currentResource.distance, currentResource.blipIndex));
                    continue;
                }

                // Collect resources for this group
                var groupMembers = new List<int> { i };

                // Find resources of the same type within grouping distance
                for (int j = 0; j < resources.Count; j++)
                {
                    if (j == i || processedIndices.Contains(j)) continue;

                    var otherResource = resources[j];

                    // Only group same types
                    if (currentResource.resource.techType == otherResource.resource.techType)
                    {
                        float distSquared = Vector3.SqrMagnitude(
                            currentResource.resource.position - otherResource.resource.position);

                        if (distSquared <= groupingDistanceSquared)
                        {
                            // Skip resources too close to player
                            if (shouldBreakGroups)
                            {
                                float otherDistToPlayerSquared = Vector3.SqrMagnitude(otherResource.resource.position - playerPosition);
                                if (otherDistToPlayerSquared <= breakDistanceSquared)
                                    continue;
                            }

                            groupMembers.Add(j);
                            processedIndices.Add(j);
                        }
                    }
                }

                // Process found group
                if (groupMembers.Count > 1)
                {
                    // Find closest resource to player for group representation
                    int closestIndex = FindClosestResourceToPlayer(resources, groupMembers, playerPosition);
                    var primaryResource = resources[closestIndex];
                    result.Add((primaryResource.resource, groupMembers.Count, primaryResource.distance, primaryResource.blipIndex));
                }
                else
                {
                    // Single resource
                    result.Add((currentResource.resource, 1, currentResource.distance, currentResource.blipIndex));
                }
            }

            // Add any resources that were skipped (near player)
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

        private static int FindClosestResourceToPlayer(
            List<(ResourceTrackerDatabase.ResourceInfo resource, float distance, int blipIndex)> resources,
            List<int> indices,
            Vector3 playerPosition)
        {
            float closestDistanceSqr = float.MaxValue;
            int closestIndex = indices[0];

            foreach (int index in indices)
            {
                var resource = resources[index];
                float distSqr = Vector3.SqrMagnitude(resource.resource.position - playerPosition);

                if (distSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distSqr;
                    closestIndex = index;
                }
            }

            return closestIndex;
        }
    }
}