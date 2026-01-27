using System;
using Nautilus.Json;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace Ungeziefi.Creature_Healthbars
{
    public enum CreatureFilterOption
    {
        AllCreatures,
        OnlyPredators,
        OnlyNonPredators
    }

    [Menu("Creature Healthbars")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Enable feature")]
        public bool EnableFeature = true;

        [Toggle(Label = "Only show for player damage")]
        public bool OnlyShowForPlayerDamage = true;

        [Choice(Label = "Creature filter",
        Tooltip = "Controls which types of creatures will display health bars.",
        Options = new[] { "All creatures", "Only predators", "Only non-predators" })]
        public CreatureFilterOption CreatureFilter = CreatureFilterOption.AllCreatures;

        [Slider(Label = "Display duration", Tooltip = "How long the health bar remains visible after taking damage (in seconds).",
            DefaultValue = 5.0f, Min = 1.0f, Max = 15.0f, Step = 1.0f, Format = "{0:0.0}s")]
        public float DisplayDuration = 5.0f;

        [Toggle(Label = "Show health numbers")]
        public bool ShowHealthNumbers = true;

        [Toggle(Label = "Show name")]
        public bool ShowName = false;

        [Slider(Label = "Size multiplier", Tooltip = "Controls the overall size of the health bar.",
            DefaultValue = 0.5f, Min = 0.5f, Max = 2.0f, Step = 0.1f, Format = "{0:0.0}")]
        public float SizeMultiplier = 0.5f;

        [Slider(Label = "Minimum size", Tooltip = "Prevents the health bar from being too small on tiny creatures.",
            DefaultValue = 2.0f, Min = 0.5f, Max = 3.0f, Step = 0.1f, Format = "{0:0.0}")]
        public float MinimumSize = 2.0f;

        [Slider(Label = "Bar ratio", Tooltip = "Width-to-height ratio of the health bar. Higher values make wider, thinner bars.",
            DefaultValue = 8.0f, Min = 4.0f, Max = 16.0f, Step = 1.0f, Format = "{0:0.0}")]
        public float BarRatio = 8.0f;

        [Slider(Label = "Height padding", Tooltip = "Space between the creature and its health bar (as % of creature height).",
            DefaultValue = 0f, Min = -1.0f, Max = 1.0f, Step = 0.1f, Format = "{0:0.0}")]
        public float HeightPadding = 0f;

        [Toggle("<color=#FFAC09FF>Sprite</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool SpriteDivider;

        [Slider(Label = "Sprite width", Tooltip = "Width in pixels, a higher resolution will look smoother.",
            DefaultValue = 512, Min = 64, Max = 512, Step = 64, Format = "{0}px")]
        public int SpriteWidth = 512;

        [Slider(Label = "Sprite height", Tooltip = "Height in pixels, a higher resolution will look smoother.",
            DefaultValue = 128, Min = 16, Max = 128, Step = 16, Format = "{0}px")]
        public int SpriteHeight = 128;

        [Slider(Label = "Corner roundness", DefaultValue = 0.250f, Min = 0.0f, Max = 0.5f, Step = 0.025f, Format = "{0:0.000}")]
        public float CornerRoundness = 0.250f;

        [Toggle("<color=#FFAC09FF>Health color</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool HealthColorDivider;

        [ColorPicker(Label = "Preview", Tooltip = "The color of the health portion of the bar.", Advanced = true)]
        public Color HealthColor = new(1.0f, 0.1f, 0.1f, 0.85f); // Bright red

        [Toggle("<color=#FFAC09FF>Background color</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool BackgroundColorDivider;

        [ColorPicker(Label = "Preview", Tooltip = "The color of the background portion of the bar.", Advanced = true)]
        public Color BackgroundColor = new(0.5f, 0.0f, 0.0f, 0.5f); // Dark red with transparency

        [Toggle("<color=#FFAC09FF>Text color</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool TextColorDivider;

        [ColorPicker(Label = "Preview", Advanced = true)]
        public Color TextColor = Color.white;

        [Toggle("<color=#FFAC09FF>Debug</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool DebugDivider;

        [Toggle(Label = "Enable freeze creatures toggle")]
        public bool EnableFreezeCreaturesToggle = false;

        [Keybind(Label = "Freeze creatures key", Tooltip = "Right bracket key by default.")]
        public KeyCode FreezeCreaturesKey = KeyCode.RightBracket;
    }
}