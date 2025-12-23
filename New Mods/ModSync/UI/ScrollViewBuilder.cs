using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.ModSync.UI
{
    public static class ScrollViewBuilder
    {
        public static GameObject CreateScrollView(Transform background, Transform textToReplace)
        {
            LayoutElement originalLayout = textToReplace.GetComponent<LayoutElement>();
            RectTransform textRect = textToReplace.GetComponent<RectTransform>();

            GameObject scrollView = new("Scroll View");
            scrollView.transform.SetParent(background, false);

            RectTransform scrollRect = scrollView.AddComponent<RectTransform>();
            if (textRect != null)
            {
                scrollRect.anchorMin = textRect.anchorMin;
                scrollRect.anchorMax = textRect.anchorMax;
                scrollRect.anchoredPosition = textRect.anchoredPosition;
                scrollRect.sizeDelta = textRect.sizeDelta;
                scrollRect.pivot = textRect.pivot;
            }

            LayoutElement layout = scrollView.AddComponent<LayoutElement>();
            if (originalLayout != null)
            {
                layout.minHeight = originalLayout.minHeight;
                layout.preferredHeight = originalLayout.preferredHeight;
                layout.flexibleHeight = originalLayout.flexibleHeight;
            }

            // Position and cleanup
            int siblingIndex = textToReplace.GetSiblingIndex();
            scrollView.transform.SetSiblingIndex(siblingIndex);
            Object.Destroy(textToReplace.gameObject);

            // ScrollRect
            ScrollRect scrollRectComponent = scrollView.AddComponent<ScrollRect>();
            scrollRectComponent.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;

            // Viewport
            CreateViewport(scrollView.transform);

            // Scrollbar
            GameObject scrollbar = CreateScrollbar(scrollView.transform);
            scrollRectComponent.verticalScrollbar = scrollbar.GetComponent<Scrollbar>();

            return scrollView;
        }

        private static GameObject CreateViewport(Transform parent)
        {
            GameObject viewport = new("Viewport");
            viewport.transform.SetParent(parent, false);

            RectTransform viewportRect = viewport.AddComponent<RectTransform>();
            viewportRect.anchorMin = new Vector2(0, 0);
            viewportRect.anchorMax = new Vector2(1, 1);
            viewportRect.anchoredPosition = Vector2.zero;
            viewportRect.sizeDelta = new Vector2(-47, 0);
            viewportRect.pivot = new Vector2(0, 1);

            RectMask2D mask = viewport.AddComponent<RectMask2D>();
            mask.padding = Vector4.zero;
            mask.softness = Vector2Int.zero;

            return viewport;
        }

        private static GameObject CreateScrollbar(Transform parent)
        {
            GameObject scrollbarTemplate = GameObject.Find("Menu canvas/Panel/MainMenu/RightSide/SavedGames/Scroll View/Scrollbar");
            GameObject scrollbar = Object.Instantiate(scrollbarTemplate, parent, false);
            scrollbar.name = "Scrollbar";

            RectTransform scrollbarRect = scrollbar.GetComponent<RectTransform>();
            scrollbarRect.anchorMin = new Vector2(1, 0);
            scrollbarRect.anchorMax = new Vector2(1, 1);
            scrollbarRect.pivot = new Vector2(1, 0.5f);
            scrollbarRect.anchoredPosition = Vector2.zero;
            scrollbarRect.sizeDelta = new Vector2(DialogConfig.SCROLLBAR_WIDTH, 0);

            return scrollbar;
        }

        public static void ConfigureContent(GameObject scrollView, string message)
        {
            Transform viewport = scrollView.transform.Find("Viewport");
            if (viewport == null) return;

            GameObject content = new("Content");
            content.transform.SetParent(viewport, false);

            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = Vector2.zero;

            VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childAlignment = TextAnchor.UpperLeft;
            vlg.spacing = 0;
            vlg.padding = new RectOffset(
                (int)DialogConfig.TEXT_PADDING,
                (int)DialogConfig.TEXT_PADDING,
                (int)DialogConfig.TEXT_PADDING,
                (int)DialogConfig.TEXT_PADDING
            );

            ContentSizeFitter contentFitter = content.AddComponent<ContentSizeFitter>();
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            // Text
            GameObject textObj = new("Text");
            textObj.transform.SetParent(content.transform, false);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 1);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.pivot = new Vector2(0.5f, 1);
            textRect.anchoredPosition = Vector2.zero;

            LayoutElement textLayout = textObj.AddComponent<LayoutElement>();
            textLayout.flexibleWidth = 1;
            textLayout.flexibleHeight = -1;

            TextMeshProUGUI textMesh = textObj.AddComponent<TextMeshProUGUI>();
            textMesh.text = message;
            textMesh.alignment = TextAlignmentOptions.TopLeft;
            textMesh.enableWordWrapping = true;
            textMesh.overflowMode = TextOverflowModes.Overflow;
            textMesh.fontSize = DialogConfig.TEXT_FONT_SIZE;
            textMesh.margin = Vector4.zero;

            ContentSizeFitter textFitter = textObj.AddComponent<ContentSizeFitter>();
            textFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            textFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            // Assign content to ScrollRect
            ScrollRect scrollRectComponent = scrollView.GetComponent<ScrollRect>();
            if (scrollRectComponent != null)
            {
                scrollRectComponent.content = contentRect;
            }
        }
    }
}