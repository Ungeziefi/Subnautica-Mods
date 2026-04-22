using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.Creature_Healthbars;

[HarmonyPatch]
public partial class CreatureHealthbars
{
    private const float WorldToUIScale = 10f;
    private const float BaseWidth = 4.0f;
    private static readonly Dictionary<string, GameObject> healthBars = new();
    private static readonly Dictionary<string, float> timers = new();
    private static Sprite roundedSprite;
    private static readonly List<string> expiredBars = new();

    private static void ShowHealthBar(Creature creature, string id, float healthPercent)
    {
        GetBarDimensions(creature, out var barWidth, out var barHeight);
        var liveMixin = creature.GetComponent<LiveMixin>();

        // 1. Get or create bar
        if (!healthBars.TryGetValue(id, out var bar) || bar == null)
        {
            bar = CreateHealthBarObject(creature.gameObject, barWidth, barHeight);
            healthBars[id] = bar;
        }

        // 2. Reset alpha if the bar is already visible
        var canvasGroup = bar.GetComponent<CanvasGroup>();
        if (canvasGroup != null) canvasGroup.alpha = 1f;

        // 3. Update fill width
        var healthRect = bar.transform.Find("CHB_Background/CHB_Health")?.GetComponent<RectTransform>();
        if (healthRect != null)
            healthRect.sizeDelta = new Vector2(barWidth * WorldToUIScale * healthPercent, barHeight * WorldToUIScale);

        // 4. Update text
        var parts = new List<string>();
        if (Main.Config.ShowName) parts.Add(GetCreatureName(creature));
        if (Main.Config.ShowHealthNumbers)
            parts.Add($"{Mathf.RoundToInt(liveMixin.health)}/{Mathf.RoundToInt(liveMixin.maxHealth)}");

        if (parts.Count > 0)
            UpdateOrCreateTextElement(
                bar,
                "CHB_HealthText",
                string.Join(": ", parts),
                Mathf.Max(3f, barHeight * 6f),
                Main.Config.TextColor);

        // 5. Update position
        bar.transform.localPosition = CalculateHealthBarPosition(creature.gameObject);
    }

    private static GameObject CreateHealthBarObject(GameObject creatureObj, float barWidth, float barHeight)
    {
        var bar = new GameObject("CHB_HealthBar");
        bar.transform.SetParent(creatureObj.transform, false);
        bar.AddComponent<FaceCamera>();
        bar.AddComponent<CanvasGroup>();

        var canvas = bar.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.transform.localScale = Vector3.one * 0.01f;
        // Background
        var bgObj = new GameObject("CHB_Background");
        bgObj.transform.SetParent(bar.transform, false);

        var bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(barWidth * WorldToUIScale, barHeight * WorldToUIScale);
        bgRect.anchoredPosition = Vector2.zero;

        var bgImage = bgObj.AddComponent<Image>();
        bgImage.sprite = roundedSprite;
        bgImage.type = Image.Type.Sliced;
        bgImage.color = Main.Config.BackgroundColor;

        var mask = bgObj.AddComponent<Mask>();
        mask.showMaskGraphic = true;

        // Health fill
        var healthObj = new GameObject("CHB_Health");
        healthObj.transform.SetParent(bgObj.transform, false);

        var healthRect = healthObj.AddComponent<RectTransform>();
        healthRect.anchorMin = Vector2.zero;
        healthRect.anchorMax = Vector2.up;
        healthRect.pivot = new Vector2(0, 0.5f);
        healthRect.anchoredPosition = Vector2.zero;

        var healthImage = healthObj.AddComponent<Image>();
        healthImage.sprite = roundedSprite;
        healthImage.type = Image.Type.Sliced;
        healthImage.color = Main.Config.HealthColor;

        return bar;
    }

    #region Harmony Patches

    [HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.TakeDamage))]
    [HarmonyPrefix]
    public static void LiveMixin_TakeDamage_Prefix(LiveMixin __instance, out bool __state)
    {
        // Check if target was alive before taking this hit
        // Fixes the last hit not spawning a health bar
        // Also prevents health bars from spawning when hitting an already dead target
        __state = __instance.health > 0;
    }

    [HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.TakeDamage))]
    [HarmonyPostfix]
    public static void LiveMixin_TakeDamage_Postfix(LiveMixin __instance, float originalDamage, GameObject dealer,
        bool __state)
    {
        if (!Main.Config.EnableFeature || originalDamage <= 0 || !__state) return;

        if (!__instance.TryGetComponent<Creature>(out var creature)) return;

        // Bleeder check
        if (__instance.TryGetComponent<AttachAndSuck>(out var bleeder) &&
            (bleeder.attached || bleeder.timeDetached + 4f < Time.time)) return;

        if (Main.Config.OnlyShowForPlayerDamage && dealer != null) return;

        // Predator check
        var isPredator = creature.GetComponent<AggressiveWhenSeeTarget>() != null;
        if (Main.Config.CreatureFilter == CreatureFilterOption.OnlyPredators && !isPredator) return;
        if (Main.Config.CreatureFilter == CreatureFilterOption.OnlyNonPredators && isPredator) return;

        var id = GetCreatureId(creature.gameObject);
        if (string.IsNullOrEmpty(id)) return;

        if (roundedSprite == null) CreateSprite();

        timers[id] = Main.Config.DisplayDuration;

        ShowHealthBar(creature, id, __instance.GetHealthFraction());
    }

    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    [HarmonyPostfix]
    public static void Player_Update()
    {
        if (!Main.Config.EnableFeature) return;

        expiredBars.Clear();

        var activeIds = new List<string>(timers.Keys);

        foreach (var id in activeIds)
        {
            var timeRemaining = timers[id] - Time.deltaTime;
            timers[id] = timeRemaining;

            if (timeRemaining <= 0)
            {
                expiredBars.Add(id);
                continue;
            }

            // Fade out
            if (timeRemaining < 1.0f && healthBars.TryGetValue(id, out var bar) && bar != null)
            {
                var canvasGroup = bar.GetComponent<CanvasGroup>();
                if (canvasGroup != null) canvasGroup.alpha = timeRemaining;
            }
        }

        // Cleanup
        foreach (var id in expiredBars)
        {
            if (healthBars.TryGetValue(id, out var bar) && bar != null)
                Object.Destroy(bar);

            timers.Remove(id);
            healthBars.Remove(id);
        }
    }

    #endregion

    #region Utility Methods

    private static Bounds GetCreatureBounds(GameObject creature)
    {
        if (creature.TryGetComponent<Collider>(out var collider))
        {
            var bounds = collider.bounds;
            bounds.center = creature.transform.InverseTransformPoint(bounds.center);
            return bounds;
        }

        Main.Logger.LogWarning(
            $"Creature '{creature.name}' (ID: {GetCreatureId(creature)}) has no collider! Using fallback bounds.");
        return new Bounds(Vector3.zero, Vector3.one);
    }

    private static string GetCreatureId(GameObject creature)
    {
        var id = creature.GetComponent<UniqueIdentifier>();
        return id != null && !string.IsNullOrEmpty(id.Id) ? id.Id : creature.GetInstanceID().ToString();
    }

    private static string GetCreatureName(Creature creature)
    {
        var techType = CraftData.GetTechType(creature.gameObject);
        return techType != TechType.None ? Language.main.Get(techType.AsString()) : "Unknown";
    }

    private static Vector3 CalculateHealthBarPosition(GameObject creature)
    {
        var bounds = GetCreatureBounds(creature);
        return new Vector3(0, bounds.size.y + bounds.size.y * Main.Config.HeightPadding, 0);
    }

    private static void GetBarDimensions(Creature creature, out float width, out float height)
    {
        var bounds = GetCreatureBounds(creature.gameObject);
        var scaledSize = Mathf.Max(Main.Config.MinimumSize,
            (bounds.size.y + bounds.size.x + bounds.size.z) * Main.Config.SizeMultiplier);

        width = BaseWidth * scaledSize;
        height = width / Main.Config.BarRatio;
    }

    private static void UpdateOrCreateTextElement(GameObject parent, string name, string text, float fontSize,
        Color color)
    {
        var existingText = parent.transform.Find(name)?.GetComponent<TextMeshProUGUI>();

        if (existingText != null)
        {
            existingText.text = text;
            return;
        }

        var textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform, false);
        textObj.transform.localPosition = Vector3.zero;

        var textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.enableWordWrapping = false;
        textComponent.fontSize = fontSize;
        textComponent.color = color;
        textComponent.text = text;
    }

    #endregion
}