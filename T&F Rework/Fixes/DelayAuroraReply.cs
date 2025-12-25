using System.Collections;
using HarmonyLib;
using Story;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class DelayAuroraReply
    {
        private static bool isPlayingRepairVO = false;
        private const float AURORA_REPLY_DELAY = 20f;

        [HarmonyPatch(typeof(Radio), nameof(Radio.OnRepair)), HarmonyPrefix]
        public static bool Radio_OnRepair(Radio __instance)
        {
            if (!Main.Config.DelayAuroraReply) return true;

            __instance.StartCoroutine(DelayedRadioRepair(__instance));
            return false;
        }

        private static IEnumerator DelayedRadioRepair(Radio radio)
        {
            isPlayingRepairVO = true;

            radio.Invoke("PlayRadioRepairVO", 2f);

            yield return new WaitForSeconds(AURORA_REPLY_DELAY);

            StoryGoalManager.main.PulsePendingMessages();

            isPlayingRepairVO = false;
        }

        [HarmonyPatch(typeof(Radio), nameof(Radio.OnHandClick)), HarmonyPrefix]
        public static bool BlockRadioInteraction() => !Main.Config.DelayAuroraReply || !isPlayingRepairVO;

        [HarmonyPatch(typeof(Radio), nameof(Radio.OnHandHover)), HarmonyPrefix]
        public static bool BlockRadioHover() => !Main.Config.DelayAuroraReply || !isPlayingRepairVO;
    }
}