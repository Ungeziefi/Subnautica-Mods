using HarmonyLib;
using System;
using System.Collections.Generic;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class CreatureSizeRandomizer
    {
        private static readonly HashSet<TechType> targetCreatures = new HashSet<TechType>();
        private static bool listInitialized = false;

        private static void LoadTargetCreatures()
        {
            // Clear the set in case it's being reloaded
            targetCreatures.Clear();

            // Get creatures from the Config list
            foreach (string creatureName in Main.Config.CSRTargetCreatures)
            {
                if (!string.IsNullOrEmpty(creatureName) &&
                    Enum.TryParse(creatureName, true, out TechType techType))
                {
                    targetCreatures.Add(techType);
                }
            }

            listInitialized = true;
        }

        [HarmonyPatch(typeof(Creature), nameof(Creature.Start)), HarmonyPostfix]
        public static void Creature_Start(Creature __instance)
        {
            if (!Main.Config.CSREnableFeature) return;

            // Load the list only once
            if (!listInitialized) LoadTargetCreatures();

            // If list is empty
            if (targetCreatures.Count == 0) return;

            var tt = CraftData.GetTechType(__instance.gameObject);
            if (targetCreatures.Contains(tt))
            {
                // Check if the localScale is not already custom (not equal to Vector3.one)
                if (__instance.transform.localScale == UnityEngine.Vector3.one)
                {
                    __instance.transform.localScale *= UnityEngine.Random.Range(
                        Main.Config.MinCreatureSize,
                        Main.Config.MaxCreatureSize);
                }
            }
        }
    }
}