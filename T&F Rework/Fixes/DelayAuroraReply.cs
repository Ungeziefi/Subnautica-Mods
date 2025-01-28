using HarmonyLib;
using Story;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class DelayAuroraReply
    {
        private const float REPAIR_VO_DURATION = 18f;
        private static float repairVOEndTime = -1f;

        // Checks if VO is playing
        private static bool IsPlayingRepairVO()
        {
            return Time.time < repairVOEndTime;
        }

        [HarmonyPatch(typeof(Radio), nameof(Radio.OnRepair))]
        public static class Radio_OnRepair_Patch
        {
            public static bool Prefix(Radio __instance)
            {
                if (!Main.Config.DelayAuroraReply) return true;

                // Set when the repair VO will finish
                repairVOEndTime = Time.time + REPAIR_VO_DURATION;
                __instance.Invoke("PlayRadioRepairVO", 2f);
                StoryGoalManager.main.Invoke("PulsePendingMessages", REPAIR_VO_DURATION);

                return false;
            }
        }

        [HarmonyPatch(typeof(Radio), nameof(Radio.OnHandClick))]
        public static class Radio_OnHandClick_Patch
        {
            public static bool Prefix(Radio __instance)
            {
                if (!Main.Config.DelayAuroraReply) return true;

                // Block interaction during VO
                return !IsPlayingRepairVO();
            }
        }

        [HarmonyPatch(typeof(Radio), nameof(Radio.OnHandHover))]
        public static class Radio_OnHandHover_Patch
        {
            public static bool Prefix(Radio __instance)
            {
                if (!Main.Config.DelayAuroraReply) return true;

                // Block hover UI during VO
                return !IsPlayingRepairVO();
            }
        }
    }
}