using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Ungeziefi.ModSync.UI;

public static class ButtonBuilder
{
    public static void CreateDualButtons(Transform originalButton, Action onContinue, Action onCancel)
    {
        var parent = originalButton.parent;
        var originalRect = originalButton.GetComponent<RectTransform>();
        var originalLayout = originalButton.GetComponent<LayoutElement>();

        GameObject container = new("ButtonContainer");
        container.transform.SetParent(parent, false);
        container.transform.SetSiblingIndex(originalButton.GetSiblingIndex());

        var containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = originalRect.anchorMin;
        containerRect.anchorMax = originalRect.anchorMax;
        containerRect.anchoredPosition = originalRect.anchoredPosition;
        containerRect.sizeDelta = originalRect.sizeDelta;
        containerRect.pivot = originalRect.pivot;

        var hlg = container.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = DialogConfig.DUAL_BUTTONS_SPACING;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = true;
        hlg.childForceExpandHeight = true;

        var containerLayout = container.AddComponent<LayoutElement>();
        if (originalLayout != null)
        {
            containerLayout.ignoreLayout = originalLayout.ignoreLayout;
            containerLayout.minHeight = originalLayout.minHeight;
            containerLayout.preferredHeight = originalLayout.preferredHeight;
            containerLayout.flexibleHeight = originalLayout.flexibleHeight;
            containerLayout.layoutPriority = originalLayout.layoutPriority;
        }

        ConfigureButton(Object.Instantiate(originalButton.gameObject, container.transform), "Continue", onContinue);
        ConfigureButton(Object.Instantiate(originalButton.gameObject, container.transform), "Cancel", onCancel);

        // Remove original
        Object.Destroy(originalButton.gameObject);
    }

    private static void ConfigureButton(GameObject buttonObj, string text, Action onClick)
    {
        buttonObj.name = $"{text}Button";

        // Button click
        var button = buttonObj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick.Invoke());
        }

        var textTransform = buttonObj.transform.Find("Text");
        if (textTransform != null)
        {
            var textMesh = textTransform.GetComponent<TextMeshProUGUI>();
            if (textMesh != null)
            {
                textMesh.text = text;
                textMesh.alignment = TextAlignmentOptions.Center;
                textMesh.fontSize = DialogConfig.BUTTON_FONT_SIZE;
                textMesh.fontStyle = FontStyles.Bold;
            }
        }

        var layout = buttonObj.GetComponent<LayoutElement>();
        if (layout != null)
        {
            layout.ignoreLayout = false;
            layout.minWidth = -1;
            layout.preferredWidth = -1;
            layout.flexibleWidth = 1;
        }
    }
}