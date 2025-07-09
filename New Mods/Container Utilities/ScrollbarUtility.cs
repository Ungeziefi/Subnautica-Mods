using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.Container_Utilities
{
    public static class ScrollbarUtility
    {
        private static readonly Color BackgroundColor = new Color(0.1f, 0.1f, 0.12f, 0.7f);
        private static readonly Color HandleColor = new Color(0.75f, 0.75f, 0.75f, 0.8f);

        #region Public Methods
        public static ScrollRect SetupScrollView(
            uGUI_ItemsContainer instance,
            ItemsContainer container,
            bool needsHorizontalScroll,
            bool needsVerticalScroll,
            int vanillaWidth,
            int vanillaHeight)
        {
            // Create components and configure the scrollview
            List<Transform> cornerObjects = SaveCornerObjects(instance);
            GameObject scrollObject = CreateScrollViewObject(instance);
            GameObject mask = CreateMaskObject(scrollObject, instance);

            // Configure scroll behavior
            float cellSize = 71f;
            ScrollRect scrollRect = ConfigureScrollRect(scrollObject, mask, instance.rectTransform, needsHorizontalScroll, needsVerticalScroll);
            scrollRect.scrollSensitivity = cellSize;

            // Setup dimensions and position scrollbars
            ConfigureViewportDimensions(scrollRect, cellSize, container, needsHorizontalScroll, needsVerticalScroll, vanillaWidth, vanillaHeight);
            RestoreCornerObjects(cornerObjects, scrollObject.transform);

            // Add scrollbars and set visibility
            AddScrollbars(scrollObject, scrollRect);
            InitializeScrollbarVisibility(scrollRect, needsHorizontalScroll, needsVerticalScroll);

            // Force layout rebuild and update scrollbar states
            Canvas.ForceUpdateCanvases();
            UpdateScrollbarState(scrollRect);

            return scrollRect;
        }

        // Add scrollbars to a ScrollRect
        public static void AddScrollbars(GameObject parent, ScrollRect scrollRect)
        {
            // Try to reuse existing game style, fall back to custom style
            if (!TryFindScrollbarsFromEncyclopedia(parent, scrollRect))
                CreateStylizedScrollbars(parent, scrollRect);
        }

        // Update scrollbar visibility and size based on content
        public static void UpdateScrollbarState(ScrollRect scrollRect)
        {
            if (scrollRect == null)
                return;

            // Update vertical scrollbar visibility
            Scrollbar verticalScrollbar = scrollRect.verticalScrollbar;
            if (verticalScrollbar != null)
            {
                verticalScrollbar.gameObject.SetActive(true);

                // Set the handle size based on viewport/content ratio
                if (scrollRect.content.rect.height > 0)
                    verticalScrollbar.size = Mathf.Clamp01(scrollRect.viewport.rect.height / scrollRect.content.rect.height);

                // If content doesn't need scrolling, make handle fill the entire bar
                if (scrollRect.content.rect.height <= scrollRect.viewport.rect.height)
                    verticalScrollbar.size = 1f;
            }

            // Update horizontal scrollbar visibility
            Scrollbar horizontalScrollbar = scrollRect.horizontalScrollbar;
            if (horizontalScrollbar != null)
            {
                horizontalScrollbar.gameObject.SetActive(true);

                // Set the handle size based on viewport/content ratio
                if (scrollRect.content.rect.width > 0)
                    horizontalScrollbar.size = Mathf.Clamp01(scrollRect.viewport.rect.width / scrollRect.content.rect.width);

                // If content doesn't need scrolling, make handle fill the entire bar
                if (scrollRect.content.rect.width <= scrollRect.viewport.rect.width)
                    horizontalScrollbar.size = 1f;
            }
        }

        // Initialize scrollbars with proper visibility and initial size
        public static void InitializeScrollbarVisibility(ScrollRect scrollRect, bool needsHorizontalScroll, bool needsVerticalScroll)
        {
            // Make sure scrollbars stay visible
            if (scrollRect.verticalScrollbar != null && needsVerticalScroll)
            {
                // Extra guarantee the scrollbar is visible and in the right place
                scrollRect.verticalScrollbar.gameObject.SetActive(true);

                // Position fine-tuning
                RectTransform scrollbarRect = scrollRect.verticalScrollbar.GetComponent<RectTransform>();
                scrollbarRect.anchoredPosition = new Vector2(16, 0);

                // Set a visible initial size
                scrollRect.verticalScrollbar.size = 0.3f;
            }

            if (scrollRect.horizontalScrollbar != null && needsHorizontalScroll)
            {
                // Extra guarantee the scrollbar is visible and in the right place
                scrollRect.horizontalScrollbar.gameObject.SetActive(true);

                // Position fine-tuning
                RectTransform scrollbarRect = scrollRect.horizontalScrollbar.GetComponent<RectTransform>();
                scrollbarRect.anchoredPosition = new Vector2(0, -16);

                // Set a visible initial size
                scrollRect.horizontalScrollbar.size = 0.3f;
            }
        }
        #endregion

        #region Private Helper Methods
        private static GameObject CreateScrollViewObject(uGUI_ItemsContainer instance)
        {
            GameObject scrollObject = new GameObject() { name = "InventoryScrollView" };
            scrollObject.transform.SetParent(instance.transform.parent, false); // worldPositionStays = false is important
            scrollObject.AddComponent<RectTransform>();
            return scrollObject;
        }

        private static GameObject CreateMaskObject(GameObject scrollObject, uGUI_ItemsContainer instance)
        {
            GameObject mask = new GameObject { name = "Mask" };
            mask.transform.SetParent(scrollObject.transform, false);
            mask.AddComponent<RectTransform>();

            // Move inventory into mask
            instance.transform.SetParent(mask.transform, false);

            // Setup mask components
            Image image = mask.AddComponent<Image>();
            image.sprite = Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), Vector2.zero);
            image.color = Color.white; // Make sure the image is visible
            mask.AddComponent<Mask>().showMaskGraphic = false;

            return mask;
        }

        private static ScrollRect ConfigureScrollRect(GameObject scrollObject, GameObject mask,
            RectTransform content, bool horizontal, bool vertical)
        {
            ScrollRect sr = scrollObject.AddComponent<ScrollRect>();
            sr.movementType = ScrollRect.MovementType.Clamped;
            sr.horizontal = horizontal;
            sr.vertical = vertical;
            sr.viewport = mask.GetComponent<RectTransform>();
            sr.content = content;
            sr.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            sr.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            return sr;
        }

        private static void ConfigureViewportDimensions(ScrollRect scrollRect, float cellSize, ItemsContainer container,
            bool needsHorizontalScroll, bool needsVerticalScroll, int vanillaWidth, int vanillaHeight)
        {
            // Calculate viewport size
            float viewportWidth = needsHorizontalScroll ? vanillaWidth * cellSize : container.sizeX * cellSize;
            float viewportHeight = needsVerticalScroll ? vanillaHeight * cellSize : container.sizeY * cellSize;
            Vector2 viewportSize = new Vector2(viewportWidth, viewportHeight);
            Vector2 contentSize = new Vector2(container.sizeX * cellSize, container.sizeY * cellSize);

            // Configure scroll view
            RectTransform scrollViewRect = scrollRect.gameObject.GetComponent<RectTransform>();
            scrollViewRect.sizeDelta = viewportSize;
            scrollViewRect.localScale = Vector3.one;
            scrollViewRect.anchorMax = new Vector2(0.5f, 0.5f);
            scrollViewRect.anchorMin = new Vector2(0.5f, 0.5f);
            scrollViewRect.anchoredPosition = new Vector2(-284f, -4f);

            // Configure mask
            RectTransform maskRect = scrollRect.viewport;
            maskRect.anchoredPosition3D = Vector3.zero;
            maskRect.sizeDelta = viewportSize;
            maskRect.localScale = Vector3.one;

            // Configure content area
            scrollRect.content.anchoredPosition3D = Vector3.zero;
            scrollRect.content.anchorMax = new Vector2(0, 1); // Change anchors to top-left
            scrollRect.content.anchorMin = new Vector2(0, 1); // Change anchors to top-left
            scrollRect.content.pivot = new Vector2(0, 1);     // Change pivot to top-left
            scrollRect.content.sizeDelta = contentSize;
            scrollRect.content.localScale = Vector3.one;

            // Default scroll position
            scrollRect.verticalNormalizedPosition = 1;
            scrollRect.horizontalNormalizedPosition = 0;
        }

        private static List<Transform> SaveCornerObjects(uGUI_ItemsContainer instance)
        {
            List<Transform> cornerObjects = new List<Transform>();
            Transform gridTransform = instance.transform.Find("Grid");

            if (gridTransform != null)
            {
                // Find all corner objects
                string[] cornerNames = { "TL", "TR", "BL", "BR" };
                foreach (string corner in cornerNames)
                {
                    Transform cornerTransform = gridTransform.Find(corner);
                    if (cornerTransform != null)
                    {
                        cornerObjects.Add(cornerTransform);
                        cornerTransform.SetParent(instance.transform.parent, false);
                    }
                }
            }

            return cornerObjects;
        }

        private static void RestoreCornerObjects(List<Transform> cornerObjects, Transform scrollViewTransform)
        {
            Dictionary<string, Vector2> cornerPositions = new Dictionary<string, Vector2>
            {
                { "TL", new Vector2(1, 0) },
                { "TR", new Vector2(-4, 0) },
                { "BL", new Vector2(1, 3) },
                { "BR", new Vector2(-4, 3) }
            };

            foreach (var corner in cornerObjects)
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

        private static bool TryFindScrollbarsFromEncyclopedia(GameObject parent, ScrollRect scrollRect)
        {
            // Look for existing scrollbar to copy style
            GameObject scrollbarObj = GameObject.Find("uGUI_PDAScreen(Clone)/Content/EncyclopediaTab/Content/Scrollbar");
            if (scrollbarObj == null)
                return false;

            Image scrollbarImage = scrollbarObj.GetComponent<Image>();
            Sprite scrollbarSprite = scrollbarImage?.sprite;

            // Create scrollbars with game's style
            GameObject verticalScrollbar = CreateScrollbar(parent, scrollRect, scrollbarSprite, scrollbarImage, true);
            GameObject horizontalScrollbar = CreateScrollbar(parent, scrollRect, scrollbarSprite, scrollbarImage, false);

            return true;
        }

        private static GameObject CreateScrollbar(GameObject parent, ScrollRect scrollRect, Sprite scrollbarSprite, Image scrollbarImage, bool isVertical)
        {
            string name = isVertical ? "VerticalScrollbar" : "HorizontalScrollbar";
            GameObject customScrollbar = new GameObject(name);
            customScrollbar.transform.SetParent(parent.transform, false); // Set worldPositionStays to false

            RectTransform scrollbarRect = customScrollbar.AddComponent<RectTransform>();

            if (isVertical)
            {
                // Vertical scrollbar positioning
                scrollbarRect.anchorMin = new Vector2(1, 0);
                scrollbarRect.anchorMax = new Vector2(1, 1);
                scrollbarRect.pivot = new Vector2(0, 0.5f);
                scrollbarRect.anchoredPosition = new Vector2(16, 0);
                scrollbarRect.sizeDelta = new Vector2(16, 0);
            }
            else
            {
                // Horizontal scrollbar positioning
                scrollbarRect.anchorMin = new Vector2(0, 0);
                scrollbarRect.anchorMax = new Vector2(1, 0);
                scrollbarRect.pivot = new Vector2(0.5f, 1);
                scrollbarRect.anchoredPosition = new Vector2(0, -16);
                scrollbarRect.sizeDelta = new Vector2(0, 16);
            }

            // Setup scrollbar visuals
            Image customImage = customScrollbar.AddComponent<Image>();
            if (scrollbarSprite != null)
            {
                customImage.sprite = scrollbarSprite;
                customImage.type = scrollbarImage.type;
                customImage.color = scrollbarImage.color;
            }
            else
            {
                customImage.color = BackgroundColor;
            }

            // Create sliding area for the scrollbar handle
            GameObject slidingArea = new GameObject("Sliding Area");
            slidingArea.transform.SetParent(customScrollbar.transform, false);
            RectTransform slidingAreaRect = slidingArea.AddComponent<RectTransform>();
            slidingAreaRect.anchorMin = Vector2.zero;
            slidingAreaRect.anchorMax = Vector2.one;
            slidingAreaRect.pivot = new Vector2(0.5f, 0.5f);
            slidingAreaRect.anchoredPosition = Vector2.zero;
            slidingAreaRect.sizeDelta = Vector2.zero;

            // Create handle that user drags
            GameObject handle = new GameObject("Handle");
            handle.transform.SetParent(slidingArea.transform, false);
            RectTransform handleRect = handle.AddComponent<RectTransform>();

            if (isVertical)
            {
                handleRect.anchorMin = new Vector2(0, 0);
                handleRect.anchorMax = new Vector2(1, 0.2f);
            }
            else
            {
                handleRect.anchorMin = new Vector2(0, 0);
                handleRect.anchorMax = new Vector2(0.2f, 1);
            }

            handleRect.pivot = new Vector2(0.5f, 0.5f);
            handleRect.anchoredPosition = Vector2.zero;
            handleRect.sizeDelta = Vector2.zero;

            Image handleImage = handle.AddComponent<Image>();

            // Try to copy handle style from existing UI
            GameObject origSlidingAreaObj = GameObject.Find("uGUI_PDAScreen(Clone)/Content/EncyclopediaTab/Content/Scrollbar/Sliding Area");
            Transform origSlidingArea = origSlidingAreaObj?.transform;
            Transform origHandle = origSlidingArea?.Find("Handle");

            if (origHandle != null)
            {
                Image origHandleImage = origHandle.GetComponent<Image>();
                if (origHandleImage != null && origHandleImage.sprite != null)
                {
                    handleImage.sprite = origHandleImage.sprite;
                    handleImage.type = origHandleImage.type;
                    handleImage.color = origHandleImage.color;
                }
                else
                {
                    handleImage.color = HandleColor;
                    handleImage.sprite = Resources.FindObjectsOfTypeAll<Sprite>()
                        .FirstOrDefault(s => s.name == "UISprite" || s.name == "Background" || s.name == "Knob");
                }
            }
            else
            {
                handleImage.color = HandleColor;
            }

            // Configure scrollbar behavior
            Scrollbar scrollbar = customScrollbar.AddComponent<Scrollbar>();
            scrollbar.handleRect = handleRect;
            scrollbar.targetGraphic = handleImage;

            if (isVertical)
            {
                scrollbar.direction = Scrollbar.Direction.BottomToTop;
                scrollRect.verticalScrollbar = scrollbar;
                scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
            }
            else
            {
                scrollbar.direction = Scrollbar.Direction.LeftToRight;
                scrollRect.horizontalScrollbar = scrollbar;
                scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
            }

            // Set initial size to make it visible
            scrollbar.size = 0.3f;

            customScrollbar.SetActive(true);

            return customScrollbar;
        }

        private static void CreateStylizedScrollbars(GameObject parent, ScrollRect scrollRect)
        {
            CreateStylizedScrollbar(parent, scrollRect, true);  // Vertical
            CreateStylizedScrollbar(parent, scrollRect, false); // Horizontal
        }

        private static void CreateStylizedScrollbar(GameObject parent, ScrollRect scrollRect, bool isVertical)
        {
            string name = isVertical ? "VerticalScrollbar" : "HorizontalScrollbar";
            GameObject scrollbarObject = new GameObject(name);
            scrollbarObject.transform.SetParent(parent.transform, false);
            RectTransform scrollbarRect = scrollbarObject.AddComponent<RectTransform>();
            Image scrollbarImage = scrollbarObject.AddComponent<Image>();

            // Dark semi-transparent background
            scrollbarImage.color = BackgroundColor;

            // Create sliding area
            GameObject slidingArea = new GameObject("SlidingArea");
            slidingArea.transform.SetParent(scrollbarObject.transform, false);
            RectTransform slidingAreaRect = slidingArea.AddComponent<RectTransform>();
            slidingAreaRect.anchorMin = Vector2.zero;
            slidingAreaRect.anchorMax = Vector2.one;
            slidingAreaRect.sizeDelta = Vector2.zero;
            slidingAreaRect.pivot = new Vector2(0.5f, 0.5f);

            // Create handle
            GameObject handle = new GameObject("Handle");
            handle.transform.SetParent(slidingArea.transform, false);
            RectTransform handleRect = handle.AddComponent<RectTransform>();
            Image handleImage = handle.AddComponent<Image>();

            // Light semi-transparent handle
            handleImage.color = HandleColor;
            handleRect.sizeDelta = Vector2.zero;

            // Configure based on orientation
            if (isVertical)
            {
                // Vertical handle sizing
                handleRect.anchorMin = new Vector2(0, 0);
                handleRect.anchorMax = new Vector2(1, 0.2f);

                // Vertical scrollbar positioning
                scrollbarRect.anchorMin = new Vector2(1, 0);
                scrollbarRect.anchorMax = new Vector2(1, 1);
                scrollbarRect.pivot = new Vector2(0, 0.5f);
                scrollbarRect.sizeDelta = new Vector2(16, 0);
                scrollbarRect.anchoredPosition = new Vector2(20, 0);
            }
            else
            {
                // Horizontal handle sizing
                handleRect.anchorMin = new Vector2(0, 0);
                handleRect.anchorMax = new Vector2(0.2f, 1);

                // Horizontal scrollbar positioning
                scrollbarRect.anchorMin = new Vector2(0, 0);
                scrollbarRect.anchorMax = new Vector2(1, 0);
                scrollbarRect.pivot = new Vector2(0.5f, 1);
                scrollbarRect.sizeDelta = new Vector2(0, 16);
                scrollbarRect.anchoredPosition = new Vector2(0, -20);
            }

            // Configure scrollbar component
            Scrollbar scrollbar = scrollbarObject.AddComponent<Scrollbar>();
            scrollbar.handleRect = handleRect;
            scrollbar.targetGraphic = handleImage;

            if (isVertical)
            {
                scrollbar.direction = Scrollbar.Direction.BottomToTop;
                scrollRect.verticalScrollbar = scrollbar;
                scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
            }
            else
            {
                scrollbar.direction = Scrollbar.Direction.LeftToRight;
                scrollRect.horizontalScrollbar = scrollbar;
                scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
            }

            // Set initial size to make it visible
            scrollbar.size = 0.3f;

            scrollbarObject.SetActive(true);
        }
        #endregion
    }
}