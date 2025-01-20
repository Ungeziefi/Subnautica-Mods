using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class KeepRawFishStored
    {
        // Track corpses
        private static readonly HashSet<CreatureDeath> creatureDeathsToDestroy = new HashSet<CreatureDeath>();

        public static void TryRemoveCorpses()
        {
            if (!Main.Config.KeepDeadRawFishStored || creatureDeathsToDestroy.Count == 0)
                return;

            foreach (var cd in creatureDeathsToDestroy)
            {
                // Check if stored
                Pickupable pickupable = cd.GetComponent<Pickupable>();
                if (pickupable && (pickupable._isInSub || pickupable.inventoryItem != null))
                {
                    // Reset decay state
                    var eatable = pickupable.GetComponent<Eatable>();
                    if (eatable != null && eatable.timeDecayStart == 0f)
                    {
                        eatable.SetDecomposes(true);
                    }
                    continue; // Keep it
                }
                // Not stored - clean it up
                Object.Destroy(cd.gameObject);
            }
            creatureDeathsToDestroy.Clear(); // Reset for next load
        }

        [HarmonyPatch(typeof(CreatureDeath), nameof(CreatureDeath.RemoveCorpse)), HarmonyPrefix]
        static bool RemoveCorpsePrefix(CreatureDeath __instance)
        {
            if (!Main.Config.KeepDeadRawFishStored) return true;

            // During loading: track corpse without destroying
            if (WaitScreen.IsWaiting)
            {
                creatureDeathsToDestroy.Add(__instance);
                return false; // Prevent destruction
            }
            return true; // Outside loading: let vanilla handle it
        }
    }
}