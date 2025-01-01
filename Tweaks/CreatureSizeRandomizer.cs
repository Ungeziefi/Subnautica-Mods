﻿using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch(typeof(Creature))]
    public class TweakCreatureSizeRandomizer
    {
        // Should probably serialize this
        private static readonly HashSet<TechType> targetCreatures = new HashSet<TechType>
                        {
                            TechType.CaveCrawler,
                            TechType.LavaLarva,
                            TechType.Bleeder,
                            TechType.Rockgrub,
                            TechType.Blighter,
                            TechType.Floater,
                        };

        [HarmonyPatch(nameof(Creature.Start)), HarmonyPostfix]
        public static void Start(Creature __instance)
        {
            if (!Main.TweaksConfig.CreatureSizeRandomizer)
            {
                return;
            }

            var tt = CraftData.GetTechType(__instance.gameObject);
            // Main.Logger.LogInfo($"Creature TechType: {tt}");

            if (targetCreatures.Contains(tt))
            {
                float size = Random.Range(Main.TweaksConfig.MinCreatureSize / 10f, Main.TweaksConfig.MaxCreatureSize / 10f);
                __instance.transform.localScale *= size;
                // Main.Logger.LogInfo($"Applied size for {tt}: {__instance.transform.localScale}");
            }
        }
    }
}