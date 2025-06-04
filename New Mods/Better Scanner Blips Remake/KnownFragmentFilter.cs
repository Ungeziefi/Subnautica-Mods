using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace Ungeziefi.Better_Scanner_Blips_Remake
{
    [HarmonyPatch]
    public class KnownFragmentFilter
    {
        public static readonly Dictionary<string, TechType> FragmentTypeCache = new();

        #region Core Functions
        // Get fragment type from ID
        public static TechType GetFragmentType(string uniqueId)
        {
            // Use cache if available
            if (FragmentTypeCache.TryGetValue(uniqueId, out TechType type))
                return type;

            // No cache yet
            return TechType.None;
        }

        // Is fragment for known blueprint
        public static bool IsKnownFragment(string uniqueId)
        {
            TechType fragmentType = GetFragmentType(uniqueId);
            return fragmentType != TechType.None && IsFragmentForKnownBlueprint(fragmentType);
        }
        #endregion

        #region Helpers
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

        #region TechType Caching
        private static bool isRegistered = false;

        // Capture resource registration to cache fragment types
        [HarmonyPatch(typeof(ResourceTracker), nameof(ResourceTracker.Register)), HarmonyPostfix]
        public static void ResourceTracker_Register(ResourceTracker __instance)
        {
            if (__instance == null)
                return;

            var uniqueId = __instance.GetComponent<UniqueIdentifier>()?.Id;
            if (string.IsNullOrEmpty(uniqueId))
                return;

            var techType = CraftData.GetTechType(__instance.gameObject);

            // Only process fragments with valid types
            if (techType != TechType.None &&
                techType != TechType.Fragment &&
                !FragmentTypeCache.ContainsKey(uniqueId))
            {
                FragmentTypeCache[uniqueId] = techType;
            }
        }
        #endregion
    }
}