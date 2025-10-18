using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class CyclopsSoftCollisionSoundMinimumMass
    {
        [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.OnCollisionEnter)), HarmonyPrefix]
        public static bool SubRoot_OnCollisionEnter(SubRoot __instance, Collision col)
        {
            if (!Main.Config.SoftCollisionSoundMinimumMass) return true;

            var rb = col.gameObject.GetComponent<Rigidbody>();
            if (rb != null && rb.mass < 1f)
            {
                return false;
            }

            return true;
        }
    }
}