using HarmonyLib;
using System.Collections.Generic;

namespace Ungeziefi.Better_Scanner_Blips_Remake
{
    [HarmonyPatch]
    public class KnownFragmentFilter
    {
        public static readonly Dictionary<string, TechType> FragmentTypeCache = new();

        // Get fragment type from ID
        public static TechType GetFragmentType(string uniqueId)
        {
            return !string.IsNullOrEmpty(uniqueId) && FragmentTypeCache.TryGetValue(uniqueId, out TechType type)
                ? type
                : TechType.None;
        }

        // Is fragment for known blueprint
        public static bool IsKnownFragment(string uniqueId)
        {
            TechType fragmentType = GetFragmentType(uniqueId);
            return fragmentType != TechType.None && IsFragmentForKnownBlueprint(fragmentType);
        }

        // Check if fragment unlocks known blueprint
        private static bool IsFragmentForKnownBlueprint(TechType fragmentType)
        {
            if (!PDAScanner.IsFragment(fragmentType))
                return false;

            TechType blueprint = PDAScanner.GetEntryUnlockable(fragmentType, out _);
            return KnownTech.Contains(blueprint);
        }

        // Capture resource registration to cache fragment types
        [HarmonyPatch(typeof(ResourceTracker), nameof(ResourceTracker.Register)), HarmonyPostfix]
        public static void ResourceTracker_Register(ResourceTracker __instance)
        {
            if (__instance == null)
                return;

            var uniqueId = __instance.GetComponent<UniqueIdentifier>().Id;
            if (string.IsNullOrEmpty(uniqueId) || FragmentTypeCache.ContainsKey(uniqueId))
                return;

            var techType = CraftData.GetTechType(__instance.gameObject);

            // Only process fragments with valid types
            if (techType != TechType.None && techType != TechType.Fragment)
            {
                FragmentTypeCache[uniqueId] = techType;
            }
        }
    }
}