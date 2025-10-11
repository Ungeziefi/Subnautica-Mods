using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class FrozenGasPodsInStasis
    {
        private static bool IsFrozen(Rigidbody rb) => rb != null && StasisRifle.sphere.targets.Contains(rb);

        [HarmonyPatch(typeof(GasPod), nameof(GasPod.Update)), HarmonyPrefix]
        static bool GasPod_Update(GasPod __instance)
        {
            if (!Main.Config.FrozenGasPodsInStasis) return true;

            var rb = __instance.GetComponent<Rigidbody>();
            return !IsFrozen(rb);
        }
    }
}