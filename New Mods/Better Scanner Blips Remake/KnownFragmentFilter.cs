using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace Ungeziefi.Better_Scanner_Blips_Remake
{
    [HarmonyPatch]
    public class KnownFragmentFilter
    {
        public static readonly Dictionary<string, TechType> FragmentTypeCache = new ();

        #region Core Functions
        // Get fragment type from ID
        public static TechType GetFragmentType(string uniqueId)
        {
            // Use cache if available
            if (FragmentTypeCache.TryGetValue(uniqueId, out TechType type))
                return type;

            // Try to find in world
            FindObjectByID(uniqueId);
            return FragmentTypeCache.TryGetValue(uniqueId, out type) ? type : TechType.None;
        }

        // Is fragment for known blueprint
        public static bool IsKnownFragment(string uniqueId)
        {
            TechType fragmentType = GetFragmentType(uniqueId);
            return fragmentType != TechType.None && IsFragmentForKnownBlueprint(fragmentType);
        }
        #endregion

        #region Helpers
        // Find object and cache its type
        public static GameObject FindObjectByID(string uniqueId)
        {
            // Scanned fragments have the ResourceTracker component
            var resources = Object.FindObjectsOfType<ResourceTracker>();

            foreach (var resource in resources)
            {
                // Get their UniqueIdentifier component
                var identifier = resource.GetComponent<UniqueIdentifier>();
                if (identifier != null && identifier.Id == uniqueId)
                {
                    var go = identifier.gameObject;
                    CacheFragmentType(go, uniqueId);
                    return go;
                }
            }
            return null;
        }

        // Cache type if valid
        private static void CacheFragmentType(GameObject go, string uniqueId)
        {
            TechType type = CraftData.GetTechType(go);
            if (type != TechType.None && type != TechType.Fragment)
                FragmentTypeCache[uniqueId] = type;
        }

        // Check if fragment unlocks known blueprint
        private static bool IsFragmentForKnownBlueprint(TechType fragmentType)
        {
            if (!PDAScanner.IsFragment(fragmentType))
                return false;

            TechType blueprint = PDAScanner.GetEntryUnlockable(fragmentType, out PDAScanner.EntryData entryData);
            return KnownTech.Contains(blueprint);
        }
        #endregion

        #region OnResourceDiscovered Listener
        private static bool isRegistered = false;

        [HarmonyPatch(typeof(ResourceTracker), nameof(ResourceTracker.Start)), HarmonyPostfix]
        public static void ResourceTracker_Start()
        {
            if (!isRegistered)
            {
                ResourceTrackerDatabase.onResourceDiscovered += OnResourceDiscovered;
                isRegistered = true;
            }
        }

        private static void OnResourceDiscovered(ResourceTrackerDatabase.ResourceInfo info)
        {
            if (info.techType != TechType.Fragment || FragmentTypeCache.ContainsKey(info.uniqueId))
                return;

            FindObjectByID(info.uniqueId);
        }
        #endregion
    }
}