using HarmonyLib;

namespace Ungeziefi.Fixes.Misc;

[HarmonyPatch]
public class AddMissingVFXSurfaces
{
    [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.Start))]
    [HarmonyPostfix]
    private static void SubRoot_Start(SubRoot __instance)
    {
        if (!Main.Config.AddMissingVFXSurfaces) return;

        // Add metal to main body
        if (!__instance.gameObject.GetComponent<VFXSurface>())
        {
            var vfxSurface = __instance.gameObject.AddComponent<VFXSurface>();
            vfxSurface.surfaceType = VFXSurfaceTypes.metal;
        }

        // Add glass to helm windows
        var helmGroup = __instance.transform.Find("CyclopsCollision/helmGroup");
        if (helmGroup && !helmGroup.gameObject.GetComponent<VFXSurface>())
        {
            var vfxSurface = helmGroup.gameObject.AddComponent<VFXSurface>();
            vfxSurface.surfaceType = VFXSurfaceTypes.glass;
        }
    }
}