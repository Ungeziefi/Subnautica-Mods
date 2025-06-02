using System.Collections.Generic;
using UnityEngine;

namespace Ungeziefi.Better_Scanner_Blips_Remake
{
    public partial class BetterScannerBlipsRemake
    {
        private static void GroupResourcesToPool(
            HashSet<ResourceTrackerDatabase.ResourceInfo> resources,
            Vector3 playerPosition,
            List<(ResourceTrackerDatabase.ResourceInfo, int)> resultPool)
        {
            var processed = new HashSet<ResourceTrackerDatabase.ResourceInfo>();

            float groupingDistanceSquared = Main.Config.GroupingDistance * Main.Config.GroupingDistance;
            float breakDistanceSquared = Main.Config.GroupBreakingDistance * Main.Config.GroupBreakingDistance;
            bool shouldBreakGroups = Main.Config.GroupNearbyResources && Main.Config.BreakGroupsWhenNearby;

            foreach (var currentResource in resources)
            {
                if (processed.Contains(currentResource))
                    continue;

                processed.Add(currentResource);

                // Don't group resources near player
                if (shouldBreakGroups)
                {
                    float distToPlayerSquared = Vector3.SqrMagnitude(currentResource.position - playerPosition);
                    if (distToPlayerSquared <= breakDistanceSquared)
                    {
                        resultPool.Add((currentResource, 1));
                        continue;
                    }
                }

                var groupMembers = new List<ResourceTrackerDatabase.ResourceInfo> { currentResource };

                foreach (var otherResource in resources)
                {
                    if (otherResource == currentResource || processed.Contains(otherResource))
                        continue;

                    // Group only matching types
                    if (currentResource.techType == otherResource.techType)
                    {
                        float distSquared = Vector3.SqrMagnitude(currentResource.position - otherResource.position);

                        if (distSquared <= groupingDistanceSquared)
                        {
                            // Skip if too close to player
                            if (shouldBreakGroups)
                            {
                                float otherDistToPlayerSquared = Vector3.SqrMagnitude(otherResource.position - playerPosition);
                                if (otherDistToPlayerSquared <= breakDistanceSquared)
                                    continue;
                            }

                            groupMembers.Add(otherResource);
                            processed.Add(otherResource);
                        }
                    }
                }

                // Add as group or individual
                if (groupMembers.Count > 1)
                {
                    var primaryResource = FindClosestResourceToPlayer(groupMembers, playerPosition);
                    resultPool.Add((primaryResource, groupMembers.Count));
                }
                else
                {
                    resultPool.Add((currentResource, 1));
                }
            }
        }

        private static ResourceTrackerDatabase.ResourceInfo FindClosestResourceToPlayer(
            List<ResourceTrackerDatabase.ResourceInfo> resources,
            Vector3 playerPosition)
        {
            float closestDistanceSqr = float.MaxValue;
            var closestResource = resources[0];

            foreach (var resource in resources)
            {
                float distSqr = Vector3.SqrMagnitude(resource.position - playerPosition);
                if (distSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distSqr;
                    closestResource = resource;
                }
            }

            return closestResource;
        }
    }
}