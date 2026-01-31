using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class UnmoveableProps
    {
        static readonly HashSet<TechType> techTypesToMakeUnmovable = new() {
            TechType.FarmingTray,
            TechType.BulboTree,
            TechType.PurpleBrainCoral,
            TechType.HangingFruitTree,
            TechType.SpikePlant};

        private static void MakeUnmovable(LargeWorldEntity lwe)
        {
            Rigidbody rb = lwe.GetComponent<Rigidbody>();
            if (rb != null) Object.Destroy(rb);

            // For instances already affected by a cannon
            WorldForces wf = lwe.GetComponent<WorldForces>();
            if (wf != null) Object.Destroy(wf);
        }

        [HarmonyPatch(typeof(LargeWorldEntity), nameof(LargeWorldEntity.Awake)), HarmonyPostfix]
        private static void LargeWorldEntity_Awake(LargeWorldEntity __instance)
        {
            if (!Main.Config.UnmoveableProps) return;

            TechType tt = CraftData.GetTechType(__instance.gameObject);
            if (techTypesToMakeUnmovable.Contains(tt))
            {
                MakeUnmovable(__instance);
            }
        }
    }
}