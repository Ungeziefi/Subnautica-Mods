using HarmonyLib;

namespace Ungeziefi.Tweaks.Misc
{
    [HarmonyPatch]
    public class DisableOxygenWarning
    {
        [HarmonyPatch(typeof(HintSwimToSurface), nameof(HintSwimToSurface.Update)), HarmonyPrefix]
        public static bool HintSwimToSurface_Update(HintSwimToSurface __instance)
        {
            return !Main.Config.DisableOxygenWarning;
        }
    }
}