using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class DeadlyCyclopsExplosion
    {
        [HarmonyPatch(typeof(CyclopsDestructionEvent), nameof(CyclopsDestructionEvent.DestroyCyclops)), HarmonyPrefix]
        public static void CyclopsDestructionEvent_DestroyCyclops(CyclopsDestructionEvent __instance)
        {
            if (!Main.Config.DeadlyCyclopsExplosion) return;

            __instance.subLiveMixin.Kill();
        }
    }
}