using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    // Randomizes the size of creatures
    [HarmonyPatch(typeof(Creature))]
    public class TweakCreatureSizeRandomizer
    {
        private static readonly HashSet<TechType> targetCreatures = new HashSet<TechType>
                        {
                            TechType.CaveCrawler,
                            TechType.LavaLarva,
                            TechType.Bleeder,
                            TechType.Rockgrub,
                            TechType.Blighter,
                            TechType.Floater,
                        };

        [HarmonyPatch(nameof(Creature.Start))]
        public static void Postfix(Creature __instance)
        {
            var tt = CraftData.GetTechType(__instance.gameObject);
            // Main.Logger.LogInfo($"Creature TechType: {tt}");

            if (targetCreatures.Contains(tt))
            {
                float size = Random.Range(Main.Config.MinCreatureSize / 10f, Main.Config.MaxCreatureSize / 10f);
                __instance.transform.localScale *= size;
                // Main.Logger.LogInfo($"Applied size for {tt}: {__instance.transform.localScale}");
            }
        }
    }
}