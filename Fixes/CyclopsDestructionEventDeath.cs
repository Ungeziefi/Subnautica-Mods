using HarmonyLib;

namespace Ungeziefi.Fixes
{
    // Prevent the player from respawning inside the Cyclops when it's destroyed
    [HarmonyPatch(typeof(CyclopsDestructionEvent))]
    public class FixCyclopsDestructionEventDeath
    {
        [HarmonyPatch(nameof(CyclopsDestructionEvent.DestroyCyclops)), HarmonyPrefix]
        public static void DestroyCyclops(CyclopsDestructionEvent __instance)
        {
            if (Main.FixesConfig.DeadlyCyclopsExplosion)
            {
                __instance.subLiveMixin.Kill();
            }
        }
    }
}