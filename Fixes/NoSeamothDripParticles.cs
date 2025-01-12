using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoSeamothDripParticles
    {
        [HarmonyPatch(typeof(VFXSeamothDamages), nameof(VFXSeamothDamages.Start)), HarmonyPostfix]
        public static void VFXSeamothDamages_Start(VFXSeamothDamages __instance)
        {
            if (Main.Config.NoSeamothDripParticles)
            {
                __instance.dripsParticles = null;
            }
        }
    }
}