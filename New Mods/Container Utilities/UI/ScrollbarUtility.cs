using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.Container_Utilities.UI
{
    public static class ScrollbarUtility
    {
        private static readonly Color BackgroundColor = new(0.1f, 0.1f, 0.12f, 0.7f);
        private static readonly Color HandleColor = new(0.75f, 0.75f, 0.75f, 0.8f);

        public static ScrollRect CreateScrollView(uGUI_ItemsContainer instance, ItemsContainer container, int vanillaHeight)
        {
            bool needsScrollbar = container.sizeY > vanillaHeight;

            List<Transform> corners = SaveCorners(instance);
            GameObject scrollObject = CreateScrollView(instance);
            GameObject mask = CreateMask(scrollObject, instance);

            float cellSize = 71f;
            ScrollRect scrollRect = ConfigureScrollRect(scrollObject, mask, instance.rectTransform, needsScrollbar);
            scrollRect.scrollSensitivity = cellSize;

            ConfigureViewportDimensions(scrollRect, cellSize, container, needsScrollbar, vanillaHeight);
            RestoreCorners(corners, scrollObject.transform);

            CreateScrollbar(scrollObject, scrollRect);
            InitializeScrollbar(scrollRect, needsScrollbar);

            return scrollRect;
        }

        private static void InitializeScrollbar(ScrollRect scrollRect, bool needsScrollbar)
        {
            if (scrollRect.verticalScrollbar == null || !needsScrollbar)
                return;

            Scrollbar scrollbar = scrollRect.verticalScrollbar;
            scrollbar.gameObject.SetActive(true);

            RectTransform scrollbarRect = scrollbar.GetComponent<RectTransform>();
            scrollbarRect.anchoredPosition = new Vector2(16, 0);

            scrollbar.size = 0.3f;
        }

        private static GameObject CreateScrollView(uGUI_ItemsContainer instance)
        {
            GameObject scrollObject = new() { name = "InventoryScrollView" };
            scrollObject.transform.SetParent(instance.transform.parent, false);
            scrollObject.AddComponent<RectTransform>();
            return scrollObject;
        }

        private static GameObject CreateMask(GameObject scrollObject, uGUI_ItemsContainer instance)
        {
            GameObject mask = new() { name = "Mask" };
            mask.transform.SetParent(scrollObject.transform, false);
            mask.AddComponent<RectTransform>();

            // Move inventory into mask
            instance.transform.SetParent(mask.transform, false);

            // Setup mask
            Image image = mask.AddComponent<Image>();
            image.sprite = Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), Vector2.zero);
            image.color = Color.white;
            mask.AddComponent<Mask>().showMaskGraphic = false;

            return mask;
        }

        private static ScrollRect ConfigureScrollRect(GameObject scrollObject, GameObject mask,
            RectTransform content, bool enableScrolling)
        {
            ScrollRect scrollRect = scrollObject.AddComponent<ScrollRect>();
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.horizontal = false;
            scrollRect.vertical = enableScrolling;
            scrollRect.viewport = mask.GetComponent<RectTransform>();
            scrollRect.content = content;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            return scrollRect;
        }

        private static void ConfigureViewportDimensions(ScrollRect scrollRect, float cellSize, ItemsContainer container,
            bool needsScrollbar, int vanillaHeight)
        {
            // Viewport size
            float viewportWidth = container.sizeX * cellSize;
            float viewportHeight = needsScrollbar ? vanillaHeight * cellSize : container.sizeY * cellSize;
            Vector2 viewportSize = new(viewportWidth, viewportHeight);
            Vector2 contentSize = new(container.sizeX * cellSize, container.sizeY * cellSize);

            // ScrollView
            RectTransform scrollViewRect = scrollRect.gameObject.GetComponent<RectTransform>();
            scrollViewRect.sizeDelta = viewportSize;
            scrollViewRect.localScale = Vector3.one;
            scrollViewRect.anchorMax = new Vector2(0.5f, 0.5f);
            scrollViewRect.anchorMin = new Vector2(0.5f, 0.5f);
            scrollViewRect.anchoredPosition = new Vector2(-284f, -4f);

            // Mask
            RectTransform maskRect = scrollRect.viewport;
            maskRect.anchoredPosition3D = Vector3.zero;
            maskRect.sizeDelta = viewportSize;
            maskRect.localScale = Vector3.one;

            // Content
            scrollRect.content.anchoredPosition3D = Vector3.zero;
            scrollRect.content.anchorMax = new Vector2(0, 1);
            scrollRect.content.anchorMin = new Vector2(0, 1);
            scrollRect.content.pivot = new Vector2(0, 1);
            scrollRect.content.sizeDelta = contentSize;
            scrollRect.content.localScale = Vector3.one;

            scrollRect.verticalNormalizedPosition = 1;
        }

        private static List<Transform> SaveCorners(uGUI_ItemsContainer instance)
        {
            List<Transform> corners = new();
            Transform gridTransform = instance.transform.Find("Grid");

            if (gridTransform != null)
            {
                string[] cornerNames = { "TL", "TR", "BL", "BR" };
                foreach (string corner in cornerNames)
                {
                    Transform cornerTransform = gridTransform.Find(corner);
                    if (cornerTransform != null)
                    {
                        corners.Add(cornerTransform);
                        cornerTransform.SetParent(instance.transform.parent, false);
                    }
                }
            }

            return corners;
        }

        private static void RestoreCorners(List<Transform> corners, Transform scrollViewTransform)
        {
            Dictionary<string, Vector2> cornerPositions = new()
            {
                { "TL", new Vector2(1, 0) },
                { "TR", new Vector2(-4, 0) },
                { "BL", new Vector2(1, 3) },
                { "BR", new Vector2(-4, 3) }
            };

            foreach (var corner in corners)
            {
                corner.SetParent(scrollViewTransform, false);
                RectTransform cornerRect = corner as RectTransform;

                if (cornerRect != null && cornerPositions.TryGetValue(corner.name, out Vector2 position))
                {
                    // Position corners based on their name
                    if (corner.name == "TL")
                    {
                        cornerRect.anchorMin = new Vector2(0, 1);
                        cornerRect.anchorMax = new Vector2(0, 1);
                    }
                    else if (corner.name == "TR")
                    {
                        cornerRect.anchorMin = new Vector2(1, 1);
                        cornerRect.anchorMax = new Vector2(1, 1);
                    }
                    else if (corner.name == "BL")
                    {
                        cornerRect.anchorMin = new Vector2(0, 0);
                        cornerRect.anchorMax = new Vector2(0, 0);
                    }
                    else if (corner.name == "BR")
                    {
                        cornerRect.anchorMin = new Vector2(1, 0);
                        cornerRect.anchorMax = new Vector2(1, 0);
                    }

                    cornerRect.anchoredPosition = position;
                }
            }
        }

        private static void CreateScrollbar(GameObject parent, ScrollRect scrollRect)
        {
            GameObject encyclopediaScrollbar = GameObject.Find("uGUI_PDAScreen(Clone)/Content/EncyclopediaTab/Content/Scrollbar");
            Image scrollbarImage = encyclopediaScrollbar.GetComponent<Image>();
            Sprite scrollbarSprite = scrollbarImage.sprite;

            GameObject origSlidingAreaObj = GameObject.Find("uGUI_PDAScreen(Clone)/Content/EncyclopediaTab/Content/Scrollbar/Sliding Area");
            Transform origHandle = origSlidingAreaObj.transform.Find("Handle");
            Image origHandleImage = origHandle.GetComponent<Image>();

            // Scrollbar
            GameObject scrollbar = new("Scrollbar");
            scrollbar.transform.SetParent(parent.transform, false);

            RectTransform scrollbarRect = scrollbar.AddComponent<RectTransform>();
            scrollbarRect.anchorMin = new Vector2(1, 0);
            scrollbarRect.anchorMax = new Vector2(1, 1);
            scrollbarRect.pivot = new Vector2(0, 0.5f);
            scrollbarRect.anchoredPosition = new Vector2(16, 0);
            scrollbarRect.sizeDelta = new Vector2(16, 0);

            // Scrollbar background
            Image scrollbarBg = scrollbar.AddComponent<Image>();
            if (scrollbarSprite != null)
            {
                scrollbarBg.sprite = scrollbarSprite;
                scrollbarBg.type = scrollbarImage.type;
                scrollbarBg.color = scrollbarImage.color;
            }
            else
            {
                scrollbarBg.color = BackgroundColor;
            }

            // Sliding Area
            GameObject slidingArea = new("Sliding Area");
            slidingArea.transform.SetParent(scrollbar.transform, false);
            RectTransform slidingAreaRect = slidingArea.AddComponent<RectTransform>();
            slidingAreaRect.anchorMin = Vector2.zero;
            slidingAreaRect.anchorMax = Vector2.one;
            slidingAreaRect.sizeDelta = Vector2.zero;

            // Handle
            GameObject handle = new("Handle");
            handle.transform.SetParent(slidingArea.transform, false);
            RectTransform handleRect = handle.AddComponent<RectTransform>();
            handleRect.sizeDelta = Vector2.zero;

            Image handleImage = handle.AddComponent<Image>();
            if (origHandleImage.sprite != null)
            {
                handleImage.sprite = origHandleImage.sprite;
                handleImage.type = origHandleImage.type;
                handleImage.color = origHandleImage.color;
            }
            else
            {
                handleImage.color = HandleColor;
            }

            Scrollbar scrollbarComponent = scrollbar.AddComponent<Scrollbar>();
            scrollbarComponent.handleRect = handleRect;
            scrollbarComponent.targetGraphic = handleImage;
            scrollbarComponent.direction = Scrollbar.Direction.BottomToTop;

            // Link to ScrollRect
            scrollRect.verticalScrollbar = scrollbarComponent;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;

            scrollbar.SetActive(true);
        }
    }
}