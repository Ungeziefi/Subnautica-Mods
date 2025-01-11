using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoInvisibleCyclopsButtons
    {
        private static void SetCollidersEnabled(Transform parent, bool isEnabled)
        {
            // Disable button and image raycast targets
            foreach (var button in parent.GetComponentsInChildren<Button>(true))
            {
                button.interactable = isEnabled;
            }
            foreach (var image in parent.GetComponentsInChildren<Image>(true))
            {
                image.raycastTarget = isEnabled;
            }
        }

        [HarmonyPatch(typeof(CyclopsHelmHUDManager), nameof(CyclopsHelmHUDManager.Update)), HarmonyPostfix]
        public static void CyclopsHelmHUDManager_Update(CyclopsHelmHUDManager __instance)
        {
            if (!Main.Config.NoInvisibleCyclopsButtons || !__instance.LOD.IsFull())
            {
                return;
            }

            bool hudActive = AccessTools.Field(typeof(CyclopsHelmHUDManager), "hudActive").GetValue(__instance) as bool? ?? false;
            float alpha = __instance.canvasGroup.alpha;

            // Disable interaction when inactive or nearly transparent
            SetCollidersEnabled(__instance.transform, hudActive && alpha > 0.01f);
        }
    }
}