using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class DeadPeeperCloseEyeLOD
    {
        private static bool ShouldFixEyes(Creature creature) =>
            Main.Config.DeadPeeperCloseEyeLOD &&
            creature.GetComponent<LiveMixin>()?.IsAlive() == false &&
            creature.name.Contains("Peeper");

        private static void FixPeeperEyes(Creature creature)
        {
            if (creature.transform.Find("model/peeper") is Transform peeperModel)
            {
                // Disable LOD system to prevent eye state from changing at distance
                if (peeperModel.GetComponentInChildren<LODGroup>(true) is LODGroup lodGroup)
                    lodGroup.enabled = false;

                // Enable closed eye model and disable open eye model
                if (peeperModel.Find("aqua_bird") is Transform deadEyes)
                    deadEyes.gameObject.SetActive(true);

                if (peeperModel.Find("aqua_bird_LOD1") is Transform aliveEyes)
                    aliveEyes.gameObject.SetActive(false);
            }
        }

        // Already dead when spawning
        [HarmonyPatch(typeof(Creature), nameof(Creature.Start)), HarmonyPostfix]
        public static void Creature_Start_Postfix(Creature __instance)
        {
            if (ShouldFixEyes(__instance))
                FixPeeperEyes(__instance);
        }

        // Death during gameplay
        [HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.Kill)), HarmonyPostfix]
        public static void LiveMixin_Kill_Postfix(LiveMixin __instance)
        {
            if (__instance.GetComponent<Creature>() is Creature creature && ShouldFixEyes(creature))
                FixPeeperEyes(creature);
        }
    }
}