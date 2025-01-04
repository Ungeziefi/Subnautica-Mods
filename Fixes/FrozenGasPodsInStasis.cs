using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(StasisSphere))]
    public static class FrozenGasPodsInStasis_StasisSphere
    {
        public static List<Rigidbody> stasisTargets = new List<Rigidbody>();

        [HarmonyPatch(nameof(StasisSphere.Awake)), HarmonyPostfix]
        static void Awake(StasisSphere __instance)
        {
            stasisTargets = __instance.targets;
        }
    }

    [HarmonyPatch(typeof(GasPod))]
    public static class FrozenGasPodsInStasis
    {
        [HarmonyPatch(nameof(GasPod.Update)), HarmonyPrefix]
        static bool Update(GasPod __instance)
        {
            if (!Main.Config.FrozenGasPodsInStasis)
            {
                return true;
            }

            var rb = __instance.GetComponent<Rigidbody>();
            if (rb && FrozenGasPodsInStasis_StasisSphere.stasisTargets.Contains(rb))
            {
                Main.Logger.LogInfo("Gas Pod is in stasis.");
                return false;
            }
            return true;
        }
    }
}