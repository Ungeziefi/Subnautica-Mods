using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch(typeof(uGUI_SunbeamCountdown))]
    public class CustomSunbeamCountdown
    {
        [HarmonyPatch(nameof(uGUI_SunbeamCountdown.ShowInterface)), HarmonyPostfix]
        static void ShowInterface(uGUI_SunbeamCountdown __instance)
        {
            if (!Main.Config.CustomSunbeamCountdown)
            {
                return;
            }

            var config = Main.Config;
            float xPos = config.CSCXPosition;
            float yPos = config.CSCYPosition;
            float scale = config.CSCScale;

            RectTransform rectTransform = __instance.countdownHolder.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // Small margin between the countdown message box and the screen edge
                float margin = 10f;

                rectTransform.anchorMin = new Vector2(xPos, yPos);
                rectTransform.anchorMax = new Vector2(xPos, yPos);
                rectTransform.pivot = new Vector2(xPos, yPos);
                rectTransform.anchoredPosition = new Vector2(-margin, -margin);
                rectTransform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }
}