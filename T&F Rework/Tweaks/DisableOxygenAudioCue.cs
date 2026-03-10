using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class DisableOxygenAudioCue
    {
        [HarmonyPatch(typeof(LowOxygenAlert), nameof(LowOxygenAlert.Update)), HarmonyPrefix]
        public static bool LowOxygenAlert_Update(LowOxygenAlert __instance)
        {
            return !Main.Config.DisableOxygenAudioCue;
        }
    }
}