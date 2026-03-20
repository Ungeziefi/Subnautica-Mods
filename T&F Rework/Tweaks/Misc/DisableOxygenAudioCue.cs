using HarmonyLib;

namespace Ungeziefi.Tweaks.Misc
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