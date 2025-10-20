using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.ModSync.UI
{
    public static class ButtonBuilder
    {
        public static void CreateDualButtons(Transform originalButton, Action onContinue, Action onCancel)
        {
            Transform parent = originalButton.parent;
            RectTransform originalRect = originalButton.GetComponent<RectTransform>();
            LayoutElement originalLayout = originalButton.GetComponent<LayoutElement>();

            GameObject container = new("ButtonContainer");
            container.transform.SetParent(parent, false);
            container.transform.SetSiblingIndex(originalButton.GetSiblingIndex());

            RectTransform containerRect = container.AddComponent<RectTransform>();
            containerRect.anchorMin = originalRect.anchorMin;
            containerRect.anchorMax = originalRect.anchorMax;
            containerRect.anchoredPosition = originalRect.anchoredPosition;
            containerRect.sizeDelta = originalRect.sizeDelta;
            containerRect.pivot = originalRect.pivot;

            HorizontalLayoutGroup hlg = container.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = DialogConfig.DUAL_BUTTONS_SPACING;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = true;

            LayoutElement containerLayout = container.AddComponent<LayoutElement>();
            if (originalLayout != null)
            {
                containerLayout.ignoreLayout = originalLayout.ignoreLayout;
                containerLayout.minHeight = originalLayout.minHeight;
                containerLayout.preferredHeight = originalLayout.preferredHeight;
                containerLayout.flexibleHeight = originalLayout.flexibleHeight;
                containerLayout.layoutPriority = originalLayout.layoutPriority;
            }

            ConfigureButton(UnityEngine.Object.Instantiate(originalButton.gameObject, container.transform), "Continue", onContinue);
            ConfigureButton(UnityEngine.Object.Instantiate(originalButton.gameObject, container.transform), "Cancel", onCancel);

            // Remove original
            UnityEngine.Object.Destroy(originalButton.gameObject);
        }

        private static void ConfigureButton(GameObject buttonObj, string text, Action onClick)
        {
            buttonObj.name = $"{text}Button";

            // Button click
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => onClick?.Invoke());
            }

            Transform textTransform = buttonObj.transform.Find("Text");
            if (textTransform != null)
            {
                TextMeshProUGUI textMesh = textTransform.GetComponent<TextMeshProUGUI>();
                if (textMesh != null)
                {
                    textMesh.text = text;
                    textMesh.alignment = TextAlignmentOptions.Center;
                    textMesh.fontSize = DialogConfig.BUTTON_FONT_SIZE;
                    textMesh.fontStyle = FontStyles.Bold;
                }
            }

            LayoutElement layout = buttonObj.GetComponent<LayoutElement>();
            if (layout != null)
            {
                layout.ignoreLayout = false;
                layout.minWidth = -1;
                layout.preferredWidth = -1;
                layout.flexibleWidth = 1;
            }
        }
    }
}