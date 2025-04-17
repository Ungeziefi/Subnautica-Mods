using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class CritterSizeRandomizer
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
                __instance.transform.localScale *= UnityEngine.Random.Range(
                    Main.Config.MinCreatureSize,
                    Main.Config.MaxCreatureSize);
            }
        }
    }
}