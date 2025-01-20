using System;
using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoVoiceWhileLoading
    {
        [HarmonyPatch(typeof(VoiceNotification), "Play", new Type[] { typeof(object[]) })]
        class VoiceNotification_Play_Patch
        {
            public static bool Prefix(VoiceNotification __instance)
            {
                if (!Main.Config.NoVoiceWhileLoading) return true;

                return !WaitScreen.IsWaiting;
            }
        }
    }
}