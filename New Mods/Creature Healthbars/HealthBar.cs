using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.Creature_Healthbars
{
    public partial class CreatureHealthbars
    {
        private static void GetBarDimensions(Creature creature, out float width, out float height)
        {
            // Average of height and largest horizontal dimension
            Bounds bounds = GetCreatureBounds(creature.gameObject);
            float creatureSize = (bounds.size.y + Mathf.Max(bounds.size.x, bounds.size.z));

            // Size multiplier
            float scaledSize = creatureSize * Main.Config.SizeMultiplier;

            // Absolute minimum size
            scaledSize = Mathf.Max(Main.Config.MinimumSize, scaledSize);

            // Set dimensions (width and height with aspect ratio)
            width = baseWidth * scaledSize;
            height = width / Main.Config.BarRatio;
        }

        private static string GetCreatureName(Creature creature)
        {
            TechType techType = CraftData.GetTechType(creature.gameObject);
            if (techType != TechType.None)
            {
                return Language.main.Get(techType.AsString(false));
            }
            return "Unknown";
        }

        private static TMPro.TextMeshPro CreateOrUpdateTextElement(
            GameObject parent,
            string name,
            string text,
            Vector3 position,
            float fontSize,
            Color color,
            bool isActive)
        {
            Transform textTransform = parent.transform.Find(name);
            TMPro.TextMeshPro textComponent;

            if (textTransform == null)
            {
                // Create new text object if it doesn't exist
                GameObject textObj = new GameObject(name);
                textObj.transform.SetParent(parent.transform, false);
                textObj.transform.localPosition = position;

                textComponent = textObj.AddComponent<TMPro.TextMeshPro>();
                textComponent.alignment = TMPro.TextAlignmentOptions.Center;
                textComponent.enableWordWrapping = false;
                textComponent.isOrthographic = true;
                textComponent.sortingOrder = 1;
            }
            else
            {
                // Update existing text object
                textComponent = textTransform.GetComponent<TMPro.TextMeshPro>();
                textTransform.localPosition = position;
            }

            // Update properties
            textComponent.text = text;
            textComponent.color = color;
            textComponent.fontSize = fontSize;
            textComponent.gameObject.SetActive(isActive);

            return textComponent;
        }

        // Create/update a health bar
        private static void ShowHealthBar(Creature creature, string id, float healthPercent)
        {
            GameObject bar;

            // Calculate bar dimensions
            float barWidth, barHeight;
            GetBarDimensions(creature, out barWidth, out barHeight);

            // Get health
            LiveMixin liveMixin = creature.GetComponent<LiveMixin>();
            float currentHealth = liveMixin.health;
            float maxHealth = liveMixin.maxHealth;

            // Get name if enabled
            string creatureName = Main.Config.ShowName ? GetCreatureName(creature) : null;

            lock (healthbars)
            {
                bool newBar = false;

                if (!healthbars.TryGetValue(id, out bar) || bar == null)
                {
                    newBar = true;

                    // Create health bar
                    Vector3 position = CalculateHealthBarPosition(creature.gameObject);

                    // Game object
                    bar = new GameObject("HealthBar");
                    bar.transform.SetParent(creature.transform, false);
                    bar.transform.localPosition = position;
                    bar.AddComponent<FaceCamera>();

                    // Canvas
                    Canvas canvas = bar.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.WorldSpace;
                    canvas.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

                    // Background
                    GameObject bgObj = new GameObject("Background");
                    bgObj.transform.SetParent(bar.transform, false);

                    RectTransform bgRectTransform = bgObj.AddComponent<RectTransform>();
                    bgRectTransform.sizeDelta = new Vector2(barWidth * worldToUIScale, barHeight * worldToUIScale);
                    bgRectTransform.anchoredPosition = Vector2.zero;

                    Image bgImageComponent = bgObj.AddComponent<Image>();
                    bgImageComponent.sprite = roundedSprite;
                    bgImageComponent.type = Image.Type.Sliced;
                    bgImageComponent.color = Main.Config.BackgroundColor;

                    // Health fill bar
                    GameObject healthObj = new GameObject("Health");
                    healthObj.transform.SetParent(bgObj.transform, false);

                    RectTransform healthRectTransform = healthObj.AddComponent<RectTransform>();
                    healthRectTransform.sizeDelta = new Vector2(barWidth * worldToUIScale * healthPercent, barHeight * worldToUIScale);

                    // Anchor fill bar to left side
                    healthRectTransform.anchorMin = new Vector2(0, 0);
                    healthRectTransform.anchorMax = new Vector2(0, 1);
                    healthRectTransform.pivot = new Vector2(0, 0.5f);
                    healthRectTransform.anchoredPosition = Vector2.zero;

                    Image healthImageComponent = healthObj.AddComponent<Image>();
                    healthImageComponent.sprite = roundedSprite;
                    healthImageComponent.type = Image.Type.Sliced;
                    healthImageComponent.color = Main.Config.HealthColor;

                    // Add mask to clip health bar
                    Mask mask = bgObj.AddComponent<Mask>();
                    mask.showMaskGraphic = true;

                    healthbars[id] = bar;
                }

                // Always update the existing health bar
                Transform backgroundTransform = bar.transform.Find("Background");
                Transform healthTransform = backgroundTransform?.Find("Health");

                if (backgroundTransform != null)
                {
                    // Update background size
                    RectTransform backgroundRect = backgroundTransform.GetComponent<RectTransform>();
                    if (backgroundRect != null)
                    {
                        backgroundRect.sizeDelta = new Vector2(barWidth * worldToUIScale, barHeight * worldToUIScale);
                    }

                    // Update background color
                    Image backgroundImage = backgroundTransform.GetComponent<Image>();
                    if (backgroundImage != null)
                    {
                        backgroundImage.color = Main.Config.BackgroundColor;
                    }

                    // Update health fill
                    if (healthTransform != null)
                    {
                        // Update size
                        RectTransform healthRect = healthTransform.GetComponent<RectTransform>();
                        if (healthRect != null)
                        {
                            healthRect.sizeDelta = new Vector2(barWidth * worldToUIScale * healthPercent, barHeight * worldToUIScale);
                        }

                        // Update color
                        Image healthImage = healthTransform.GetComponent<Image>();
                        if (healthImage != null)
                        {
                            healthImage.color = Main.Config.HealthColor;
                        }
                    }
                }

                // Calculate proper text positions based on the health bar height
                float fontSize = Mathf.Max(3f, barHeight * 6f);

                // Text position (centered in bar)
                Vector3 textPosition = Vector3.zero;

                // Text display options
                if (Main.Config.ShowHealthNumbers && Main.Config.ShowName)
                {
                    // Combine name and health numbers
                    string combinedText = $"{creatureName}: {Mathf.RoundToInt(currentHealth)}/{Mathf.RoundToInt(maxHealth)}";

                    CreateOrUpdateTextElement(
                        bar,
                        "HealthText",
                        combinedText,
                        textPosition,
                        fontSize,
                        Main.Config.TextColor,
                        true);

                    // Hide the name text object if it exists
                    Transform nameTransform = bar.transform.Find("CreatureName");
                    if (nameTransform != null)
                    {
                        nameTransform.gameObject.SetActive(false);
                    }
                }
                else if (Main.Config.ShowHealthNumbers)
                {
                    // Only health numbers
                    string healthNumbersText = $"{Mathf.RoundToInt(currentHealth)}/{Mathf.RoundToInt(maxHealth)}";

                    CreateOrUpdateTextElement(
                        bar,
                        "HealthText",
                        healthNumbersText,
                        textPosition,
                        fontSize,
                        Main.Config.TextColor,
                        true);

                    // Hide the name text object if it exists
                    Transform nameTransform = bar.transform.Find("CreatureName");
                    if (nameTransform != null)
                    {
                        nameTransform.gameObject.SetActive(false);
                    }
                }
                else if (Main.Config.ShowName)
                {
                    // Only name
                    CreateOrUpdateTextElement(
                        bar,
                        "CreatureName",
                        creatureName,
                        textPosition,
                        fontSize,
                        Main.Config.TextColor,
                        true);

                    // Hide the health text object if it exists
                    Transform healthTextTransform = bar.transform.Find("HealthText");
                    if (healthTextTransform != null)
                    {
                        healthTextTransform.gameObject.SetActive(false);
                    }
                }
                else
                {
                    // Neither, just hide both
                    Transform healthTextTransform = bar.transform.Find("HealthText");
                    if (healthTextTransform != null)
                    {
                        healthTextTransform.gameObject.SetActive(false);
                    }

                    Transform nameTransform = bar.transform.Find("CreatureName");
                    if (nameTransform != null)
                    {
                        nameTransform.gameObject.SetActive(false);
                    }
                }

                // Update position
                bar.transform.localPosition = CalculateHealthBarPosition(creature.gameObject);
            }
        }
    }
}