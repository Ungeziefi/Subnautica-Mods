using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Custom_Sunbeam_Countdown
{
    [HarmonyPatch]
    public class CustomSunbeamCountdown
    {
        private static Vector2 originalAnchorMin;
        private static Vector2 originalAnchorMax;
        private static Vector2 originalPivot;
        private static Vector2 originalAnchoredPosition;
        private static Vector3 originalLocalScale;
        private static bool originalValuesStored = false;

        [HarmonyPatch(typeof(uGUI_SunbeamCountdown), nameof(uGUI_SunbeamCountdown.UpdateInterface)), HarmonyPostfix]
        static void uGUI_SunbeamCountdown_UpdateInterface(uGUI_SunbeamCountdown __instance)
        {
            RectTransform rectTransform = __instance.countdownHolder.GetComponent<RectTransform>();
            if (rectTransform == null) return;

            if (Main.Config.EnableFeature)
            {
                if (!originalValuesStored)
                {
                    // Store original values
                    originalAnchorMin = rectTransform.anchorMin;
                    originalAnchorMax = rectTransform.anchorMax;
                    originalPivot = rectTransform.pivot;
                    originalAnchoredPosition = rectTransform.anchoredPosition;
                    originalLocalScale = rectTransform.localScale;
                    originalValuesStored = true;
                }

                // Apply settings
                float xPos = Main.Config.XPosition;
                float yPos = Main.Config.YPosition;
                float scale = Main.Config.Scale;
                float margin = 10f;

                rectTransform.anchorMin = new Vector2(xPos, yPos);
                rectTransform.anchorMax = new Vector2(xPos, yPos);
                rectTransform.pivot = new Vector2(xPos, yPos);
                rectTransform.anchoredPosition = new Vector2(-margin, -margin);
                rectTransform.localScale = new Vector3(scale, scale, scale);
            }
            else if (originalValuesStored)
            {
                // Restore original values
                rectTransform.anchorMin = originalAnchorMin;
                rectTransform.anchorMax = originalAnchorMax;
                rectTransform.pivot = originalPivot;
                rectTransform.anchoredPosition = originalAnchoredPosition;
                rectTransform.localScale = originalLocalScale;
            }
        }
    }
}