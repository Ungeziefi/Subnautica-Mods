using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class FrozenGasPodsInStasis
    {
        [HarmonyPatch(typeof(GasPod), nameof(GasPod.Update)), HarmonyPrefix]
        static bool GasPod_Update(GasPod __instance)
        {
            if (!Main.Config.FrozenGasPodsInStasis) return true;

            // Stasis makes the Rigidbody kinematic
            var rb = __instance.GetComponent<Rigidbody>();
            return !rb.isKinematic;
        }
    }
}