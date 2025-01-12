using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Custom_Sunbeam_Countdown
{
    [HarmonyPatch]
    public class CustomSunbeamCountdown
    {
        [HarmonyPatch(typeof(uGUI_SunbeamCountdown), nameof(uGUI_SunbeamCountdown.ShowInterface)), HarmonyPostfix]
        static void uGUI_SunbeamCountdown_ShowInterface(uGUI_SunbeamCountdown __instance)
        {
            if (!Main.Config.EnableFeature) return;

            float xPos = Main.Config.XPosition;
            float yPos = Main.Config.YPosition;
            float scale = Main.Config.Scale;

            RectTransform rectTransform = __instance.countdownHolder.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
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