using System.Collections.Generic;
using UnityEngine;

namespace Ungeziefi.Better_Scanner_Blips_Remake
{
    public partial class BetterScannerBlipsRemake
    {
        private static readonly Dictionary<string, TechType> GroupingFragmentTypeCache = new();
        private static readonly List<ResourceTrackerDatabase.ResourceInfo> filteredResourcesPool = new(256);

        private static void GroupResourcesToPool(
            HashSet<ResourceTrackerDatabase.ResourceInfo> resources,
            Vector3 playerPosition,
            List<(ResourceTrackerDatabase.ResourceInfo, int)> resultPool)
        {
            // Clear cache
            GroupingFragmentTypeCache.Clear();

            var processed = new HashSet<ResourceTrackerDatabase.ResourceInfo>();
            resultPool.Clear(); // Clear pool

            // Squared distances for performance
            float groupingDistanceSquared = Main.Config.GroupingDistance * Main.Config.GroupingDistance;
            float breakDistanceSquared = Main.Config.GroupBreakingDistance * Main.Config.GroupBreakingDistance;
            bool shouldBreakGroups = Main.Config.GroupNearbyResources && Main.Config.BreakGroupsWhenNearby;

            bool needsFiltering = ShouldApplyFiltering();
            List<ResourceTrackerDatabase.ResourceInfo> activeResources;
            if (needsFiltering)
            {
                // Apply filtering
                activeResources = FilterActiveResources(resources, playerPosition);
            }
            else
            {
                // No filtering
                activeResources = new List<ResourceTrackerDatabase.ResourceInfo>(resources);
            }

            // Nothing to process
            if (activeResources.Count == 0)
                return;

            // Group resources
            foreach (var currentResource in activeResources)
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

                // Get TechType for fragments and cache it
                TechType currentSpecificType = TechType.None;
                if (currentResource.techType == TechType.Fragment)
                {
                    if (!GroupingFragmentTypeCache.TryGetValue(currentResource.uniqueId, out currentSpecificType))
                    {
                        currentSpecificType = KnownFragmentFilter.GetFragmentType(currentResource.uniqueId);
                        GroupingFragmentTypeCache[currentResource.uniqueId] = currentSpecificType;
                    }
                }

                var groupMembers = new List<ResourceTrackerDatabase.ResourceInfo>(16) { currentResource };
                foreach (var otherResource in activeResources)
                {
                    if (otherResource == currentResource || processed.Contains(otherResource))
                        continue;

                    // Only group same TechTypes
                    if (currentResource.techType == otherResource.techType)
                    {
                        bool canGroup = true;

                        // Fragments
                        if (currentResource.techType == TechType.Fragment && currentSpecificType != TechType.None)
                        {
                            // Get specific TechType and cache
                            TechType otherSpecificType;
                            if (!GroupingFragmentTypeCache.TryGetValue(otherResource.uniqueId, out otherSpecificType))
                            {
                                otherSpecificType = KnownFragmentFilter.GetFragmentType(otherResource.uniqueId);
                                GroupingFragmentTypeCache[otherResource.uniqueId] = otherSpecificType;
                            }

                            // Only group same specific TechTypes for fragments
                            canGroup = (otherSpecificType != TechType.None &&
                                       currentSpecificType == otherSpecificType);
                        }

                        if (canGroup)
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
                }

                // Find primary resource in group (closest to player)
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

        #region Filtering Helpers
        private static List<ResourceTrackerDatabase.ResourceInfo> FilterActiveResources(
            HashSet<ResourceTrackerDatabase.ResourceInfo> resources,
            Vector3 playerPosition)
        {
            // Clear pool
            filteredResourcesPool.Clear();

            foreach (var resource in resources)
            {
                float distance = Vector3.Distance(resource.position, playerPosition);

                // Skip if general hide conditions apply
                if (ShouldHideBlip(distance))
                    continue;

                // Skip if it's a known fragment that should be hidden
                if (Main.Config.HideKnownFragmentBlips &&
                    resource.techType == TechType.Fragment &&
                    KnownFragmentFilter.IsKnownFragment(resource.uniqueId))
                    continue;

                filteredResourcesPool.Add(resource);
            }

            return filteredResourcesPool;
        }

        // Check if filtering is needed
        private static bool ShouldApplyFiltering()
        {
            return ShouldHideBlip(0f) || Main.Config.HideKnownFragmentBlips;
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
        #endregion
    }
}