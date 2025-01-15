using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class FrozenGasPodsInStasis
    {
        public static List<Rigidbody> stasisTargets = new List<Rigidbody>();

        [HarmonyPatch(typeof(StasisSphere), nameof(StasisSphere.Awake)), HarmonyPostfix]
        static void StasisSphere_Awake(StasisSphere __instance) => stasisTargets = __instance.targets;

        [HarmonyPatch(typeof(GasPod), nameof(GasPod.Update)), HarmonyPrefix]
        static bool GasPod_Update(GasPod __instance)
        {
            if (!Main.Config.FrozenGasPodsInStasis) return true;

            var rb = __instance.GetComponent<Rigidbody>();
            return rb == null || !stasisTargets.Contains(rb);
        }
    }
}