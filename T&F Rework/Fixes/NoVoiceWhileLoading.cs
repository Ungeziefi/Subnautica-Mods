using System;
using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoVoiceWhileLoading
    {
        [HarmonyPatch(typeof(VoiceNotification), "Play", new Type[] { typeof(object[]) })]
        public static bool VoiceNotification_Play(VoiceNotification __instance)
        {
            if (!Main.Config.NoVoiceWhileLoading) return true;

            return !WaitScreen.IsWaiting;
        }
    }
}