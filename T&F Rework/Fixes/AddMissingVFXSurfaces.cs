using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class AddMissingVFXSurfaces
    {
        [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.Start)), HarmonyPostfix]
        private static void SubRoot_Start(SubRoot __instance)
        {
            if (!Main.Config.AddMissingVFXSurfaces) return;

            // Add metal to main body
            if (!__instance.gameObject.GetComponent<VFXSurface>())
            {
                VFXSurface vfxSurface = __instance.gameObject.AddComponent<VFXSurface>();
                vfxSurface.surfaceType = VFXSurfaceTypes.metal;
            }

            // Add glass to helm windows
            Transform helmGroup = __instance.transform.Find("CyclopsCollision/helmGroup");
            if (helmGroup && !helmGroup.gameObject.GetComponent<VFXSurface>())
            {
                VFXSurface vfxSurface = helmGroup.gameObject.AddComponent<VFXSurface>();
                vfxSurface.surfaceType = VFXSurfaceTypes.glass;
            }
        }
    }
}