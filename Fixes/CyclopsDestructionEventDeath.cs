using HarmonyLib;

namespace Ungeziefi.Fixes
{
    // Prevent the player from respawning inside the Cyclops when it's destroyed
    [HarmonyPatch(typeof(CyclopsDestructionEvent))]
    public class FixCyclopsDestructionEventDeath
    {
        [HarmonyPatch(nameof(CyclopsDestructionEvent.DestroyCyclops))]
        public static void Prefix(CyclopsDestructionEvent __instance)
        {
            __instance.subLiveMixin.Kill();
        }
    }
}