// To-Do: Fix CreateOrUpdateTextElement affecting the player needs UI

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.Creature_Healthbars
{
    public partial class CreatureHealthbars
    {
        private static readonly Dictionary<string, GameObject> healthBars = new();
        private static readonly Dictionary<string, float> timers = new();
        private static Sprite roundedSprite;
        private static readonly float worldToUIScale = 10f; // Conversion factor
        private static readonly float baseWidth = 4.0f;

        #region Utility Methods
        private static Bounds GetCreatureBounds(GameObject creature)
        {
            Bounds bounds = new(Vector3.zero, Vector3.one);

            Collider collider = creature.GetComponent<Collider>();
            if (collider != null)
            {
                bounds = collider.bounds;
                // Convert to local space
                bounds.center = creature.transform.InverseTransformPoint(bounds.center);
                return bounds;
            }

            // Fallback
            Main.Logger.LogWarning($"Creature '{creature.name}' (ID: {GetCreatureId(creature)}) has no collider - this is abnormal and should be fixed! Using default bounds as fallback.");
            bounds.center = Vector3.zero;
            bounds.size = new Vector3(1f, 1f, 1f);
            return bounds;
        }

        private static string GetCreatureId(GameObject creature)
        {
            UniqueIdentifier id = creature.GetComponent<UniqueIdentifier>();
            return id != null && !string.IsNullOrEmpty(id.Id) ? id.Id : creature.GetInstanceID().ToString();
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

        private static Vector3 CalculateHealthBarPosition(GameObject creature)
        {
            Bounds bounds = GetCreatureBounds(creature);

            // Above creature's center + padding
            float padding = bounds.size.y * Main.Config.HeightPadding;
            return new Vector3(0, bounds.size.y + padding, 0);
        }

        private static void GetBarDimensions(Creature creature, out float width, out float height)
        {
            Bounds bounds = GetCreatureBounds(creature.gameObject);

            // Average size based on bounds
            float creatureSize = (bounds.size.y + bounds.size.x + bounds.size.z) / 3f;

            // Linear scaling with multiplier and minimum size
            float scaledSize = creatureSize * Main.Config.SizeMultiplier;
            scaledSize = Mathf.Max(Main.Config.MinimumSize, scaledSize);

            // Set dimensions (width and height with aspect ratio)
            width = baseWidth * scaledSize;
            height = width / Main.Config.BarRatio;
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
                GameObject textObj = new(name);
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
        #endregion

        private static void ShowHealthBar(Creature creature, string id, float healthPercent)
        {
            // Calculate bar dimensions
            GetBarDimensions(creature, out float barWidth, out float barHeight);

            // Get health
            LiveMixin liveMixin = creature.GetComponent<LiveMixin>();
            float currentHealth = liveMixin.health;
            float maxHealth = liveMixin.maxHealth;

            // Get name if enabled
            string creatureName = Main.Config.ShowName ? GetCreatureName(creature) : null;

            lock (healthBars)
            {
                if (!healthBars.TryGetValue(id, out GameObject bar) || bar == null)
                {
                    // Create health bar
                    Vector3 position = CalculateHealthBarPosition(creature.gameObject);

                    // Game object
                    bar = new GameObject("CHB_HealthBar");
                    bar.transform.SetParent(creature.transform, false);
                    bar.transform.localPosition = position;
                    bar.AddComponent<FaceCamera>();

                    // Canvas
                    Canvas canvas = bar.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.WorldSpace;
                    canvas.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

                    // Background
                    GameObject bgObj = new("CHB_Background");
                    bgObj.transform.SetParent(bar.transform, false);

                    RectTransform bgRectTransform = bgObj.AddComponent<RectTransform>();
                    bgRectTransform.sizeDelta = new Vector2(barWidth * worldToUIScale, barHeight * worldToUIScale);
                    bgRectTransform.anchoredPosition = Vector2.zero;

                    Image bgImageComponent = bgObj.AddComponent<Image>();
                    bgImageComponent.sprite = roundedSprite;
                    bgImageComponent.type = Image.Type.Sliced;
                    bgImageComponent.color = Main.Config.BackgroundColor;

                    // Health fill bar
                    GameObject healthObj = new("CHB_Health");
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

                    healthBars[id] = bar;
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
                        "CHB_HealthText",
                        combinedText,
                        textPosition,
                        fontSize,
                        Main.Config.TextColor,
                        true);

                    // Hide the name text object if it exists
                    Transform nameTransform = bar.transform.Find("CHB_CreatureName");
                    nameTransform?.gameObject.SetActive(false);
                }
                else if (Main.Config.ShowHealthNumbers)
                {
                    // Only health numbers
                    string healthNumbersText = $"{Mathf.RoundToInt(currentHealth)}/{Mathf.RoundToInt(maxHealth)}";

                    CreateOrUpdateTextElement(
                        bar,
                        "CHB_HealthText",
                        healthNumbersText,
                        textPosition,
                        fontSize,
                        Main.Config.TextColor,
                        true);

                    // Hide the name text object if it exists
                    Transform nameTransform = bar.transform.Find("CHB_CreatureName");
                    nameTransform?.gameObject.SetActive(false);
                }
                else if (Main.Config.ShowName)
                {
                    // Only name
                    CreateOrUpdateTextElement(
                        bar,
                        "CHB_CreatureName",
                        creatureName,
                        textPosition,
                        fontSize,
                        Main.Config.TextColor,
                        true);

                    // Hide the health text object if it exists
                    Transform healthTextTransform = bar.transform.Find("CHB_HealthText");
                    healthTextTransform?.gameObject.SetActive(false);
                }
                else
                {
                    // Neither, just hide both
                    Transform healthTextTransform = bar.transform.Find("CHB_HealthText");
                    healthTextTransform?.gameObject.SetActive(false);

                    Transform nameTransform = bar.transform.Find("CHB_CreatureName");
                    nameTransform?.gameObject.SetActive(false);
                }

                // Update position
                bar.transform.localPosition = CalculateHealthBarPosition(creature.gameObject);
            }
        }
    }
}