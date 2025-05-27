using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.Creature_Healthbars
{
    public partial class CreatureHealthbars
    {
        private static void GetBarDimensions(Creature creature, out float width, out float height)
        {
            Bounds bounds = GetCreatureBounds(creature.gameObject);

            // Calculate base size factor from creature dimensions
            float creatureSizeFactor = (bounds.size.y + Mathf.Max(bounds.size.x, bounds.size.z)) * 0.5f;

            // Minimum size for small creatures
            float normalizedSizeFactor = Mathf.Max(0.5f, creatureSizeFactor);

            // Apply creature size factor and user config to width
            width = baseWidth * normalizedSizeFactor * Main.Config.SizeMultiplier;

            // Calculate height based on width and config ratio
            height = baseHeight * normalizedSizeFactor * (4.0f / Main.Config.BarRatio) * Main.Config.SizeMultiplier;
        }

        // Create/update a health bar
        private static void ShowHealthBar(Creature creature, string id, float healthPercent)
        {
            GameObject bar;

            // Calculate bar dimensions
            float barWidth, barHeight;
            GetBarDimensions(creature, out barWidth, out barHeight);

            // Get actual health values from LiveMixin
            LiveMixin liveMixin = creature.GetComponent<LiveMixin>();
            float currentHealth = liveMixin.health;
            float maxHealth = liveMixin.maxHealth;

            lock (healthbars)
            {
                if (!healthbars.TryGetValue(id, out bar) || bar == null)
                {
                    // Create new health bar
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

                    RectTransform bgRect = bgObj.AddComponent<RectTransform>();
                    bgRect.sizeDelta = new Vector2(barWidth * worldToUIScale, barHeight * worldToUIScale);
                    bgRect.anchoredPosition = Vector2.zero;

                    Image bgImage = bgObj.AddComponent<Image>();
                    bgImage.sprite = roundedSprite;
                    bgImage.type = Image.Type.Sliced;
                    bgImage.color = Main.Config.BackgroundColor;

                    // Health fill bar
                    GameObject healthObj = new GameObject("Health");
                    healthObj.transform.SetParent(bgObj.transform, false);

                    RectTransform healthRect = healthObj.AddComponent<RectTransform>();
                    healthRect.sizeDelta = new Vector2(barWidth * worldToUIScale * healthPercent, barHeight * worldToUIScale);

                    // Anchor fill bar to left side
                    healthRect.anchorMin = new Vector2(0, 0);
                    healthRect.anchorMax = new Vector2(0, 1);
                    healthRect.pivot = new Vector2(0, 0.5f);
                    healthRect.anchoredPosition = Vector2.zero;

                    Image healthImage = healthObj.AddComponent<Image>();
                    healthImage.sprite = roundedSprite;
                    healthImage.type = Image.Type.Sliced;
                    healthImage.color = Main.Config.HealthColor;

                    // Add mask to clip health bar
                    Mask mask = bgObj.AddComponent<Mask>();
                    mask.showMaskGraphic = true;

                    // Add health text if enabled
                    if (Main.Config.ShowHealthNumbers)
                    {
                        GameObject textObj = new GameObject("HealthText");
                        textObj.transform.SetParent(bar.transform, false);

                        // Position well above the health bar (increased from 0.7f to 1.5f)
                        textObj.transform.localPosition = new Vector3(0, barHeight, 0);

                        // Use TextMeshPro for better rendering in world space
                        TMPro.TextMeshPro healthText = textObj.AddComponent<TMPro.TextMeshPro>();
                        healthText.text = $"{Mathf.RoundToInt(currentHealth)}/{Mathf.RoundToInt(maxHealth)}";
                        healthText.color = Main.Config.HealthNumbersColor;
                        healthText.alignment = TMPro.TextAlignmentOptions.Center;
                        healthText.fontSize = Mathf.Max(4f, barHeight * 6f);
                        healthText.enableWordWrapping = false;
                        healthText.isOrthographic = true;
                        healthText.sortingOrder = 1;
                    }

                    healthbars[id] = bar;
                }
                else
                {
                    // Update existing health bar
                    RectTransform bgRect = bar.transform.Find("Background")?.GetComponent<RectTransform>();
                    RectTransform healthRect = bar.transform.Find("Background/Health")?.GetComponent<RectTransform>();

                    // Update background size
                    if (bgRect != null)
                    {
                        bgRect.sizeDelta = new Vector2(barWidth * worldToUIScale, barHeight * worldToUIScale);
                    }

                    // Update health fill size
                    if (healthRect != null)
                    {
                        healthRect.sizeDelta = new Vector2(barWidth * worldToUIScale * healthPercent, barHeight * worldToUIScale);
                    }

                    // Refresh colors from config
                    Image bgImage = bar.transform.Find("Background")?.GetComponent<Image>();
                    Image healthImage = bar.transform.Find("Background/Health")?.GetComponent<Image>();

                    if (bgImage != null)
                        bgImage.color = Main.Config.BackgroundColor;

                    if (healthImage != null)
                        healthImage.color = Main.Config.HealthColor;

                    // Update health text if enabled
                    TMPro.TextMeshPro healthText = bar.transform.Find("HealthText")?.GetComponent<TMPro.TextMeshPro>();

                    if (Main.Config.ShowHealthNumbers)
                    {
                        if (healthText == null)
                        {
                            // Create text if it doesn't exist
                            GameObject textObj = new GameObject("HealthText");
                            textObj.transform.SetParent(bar.transform, false);

                            // Above the health bar
                            textObj.transform.localPosition = new Vector3(0, barHeight, 0);

                            healthText = textObj.AddComponent<TMPro.TextMeshPro>();
                            healthText.color = Main.Config.HealthNumbersColor;
                            healthText.alignment = TMPro.TextAlignmentOptions.Center;
                            healthText.fontSize = Mathf.Max(4f, barHeight * 6f);
                            healthText.enableWordWrapping = false;
                            healthText.isOrthographic = true;
                            healthText.sortingOrder = 1;
                        }
                        else
                        {
                            // Update position to stay above the bar (increased from 0.7f to 1.5f)
                            healthText.transform.localPosition = new Vector3(0, barHeight * 1.5f, 0);
                        }

                        if (healthText != null)
                        {
                            healthText.text = $"{Mathf.RoundToInt(currentHealth)}/{Mathf.RoundToInt(maxHealth)}";
                            healthText.gameObject.SetActive(true);
                        }
                    }
                    else if (healthText != null)
                    {
                        // Hide text if feature is disabled
                        healthText.gameObject.SetActive(false);
                    }

                    // Update position
                    bar.transform.localPosition = CalculateHealthBarPosition(creature.gameObject);
                }
            }
        }
    }
}