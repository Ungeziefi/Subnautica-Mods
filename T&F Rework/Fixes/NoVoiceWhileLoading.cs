using HarmonyLib;
using System;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoVoiceWhileLoading
    {
        [HarmonyPatch(typeof(VoiceNotification), nameof(VoiceNotification.Play), new Type[1] { typeof(object[]) }), HarmonyPrefix]
        public static bool VoiceNotification_Play(VoiceNotification __instance)
        {
            if (!Main.Config.NoVoiceWhileLoading) return true;

            return !WaitScreen.IsWaiting;
        }

        [HarmonyPatch(typeof(SoundQueue), nameof(SoundQueue.PlayQueued), new Type[2] { typeof(string), typeof(string) }), HarmonyPrefix]
        public static bool SoundQueue_PlayQueued(SoundQueue __instance)
        {
            if (!Main.Config.NoVoiceWhileLoading) return true;

            return !WaitScreen.IsWaiting;
        }

        [HarmonyPatch(typeof(FMOD_CustomEmitter), nameof(FMOD_CustomEmitter.Play)), HarmonyPrefix]
        public static bool FMOD_CustomEmitter_Play(FMOD_CustomEmitter __instance)
        {
            if (!Main.Config.NoVoiceWhileLoading) return true;

            return !WaitScreen.IsWaiting;
        }
    }
}