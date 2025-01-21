using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class CyclopsHelmHUDFixes
    {
        [HarmonyPatch(typeof(CyclopsHelmHUDManager), nameof(CyclopsHelmHUDManager.Update)), HarmonyPrefix]
        public static bool CyclopsHelmHUDManager_Update(CyclopsHelmHUDManager __instance)
        {
            if (!Main.Config.CyclopsHelmHUDFixes || !__instance.LOD.IsFull()) return true;

            bool isPowered = __instance.GetComponentInParent<PowerRelay>()?.IsPowered() ?? false;

            // Set HUD state to inactive if not powered
            if (!isPowered)
            {
                AccessTools.Field(typeof(CyclopsHelmHUDManager), "hudActive").SetValue(__instance, false);
            }

            return true;
        }

        [HarmonyPatch(typeof(CyclopsHelmHUDManager), nameof(CyclopsHelmHUDManager.Update)), HarmonyPostfix]
        public static void CyclopsHelmHUDManager_UpdatePostfix(CyclopsHelmHUDManager __instance)
        {
            if (!Main.Config.CyclopsHelmHUDFixes) return;

            var helmHUDVisuals = __instance.transform.Find("HelmHUDVisuals")?.gameObject;
            if (helmHUDVisuals != null)
            {
                bool hudActive = (bool)AccessTools.Field(typeof(CyclopsHelmHUDManager), "hudActive")
                    .GetValue(__instance);

                // Start fade coroutine
                if (!hudActive && helmHUDVisuals.activeSelf)
                {
                    __instance.StartCoroutine(DelayedHUDDisable(helmHUDVisuals));
                }
                else if (hudActive)
                {
                    helmHUDVisuals.SetActive(true);
                }
            }
        }

        // Preserve fade out
        private static IEnumerator DelayedHUDDisable(GameObject hud)
        {
            yield return new WaitForSeconds(1f);
            hud.SetActive(false);
        }
    }
}