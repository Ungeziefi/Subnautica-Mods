using Nautilus.Json;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace Ungeziefi.Creature_Healthbars
{
    [Menu("Creature Healthbars")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Enable feature")]
        public bool EnableFeature = true;

        [Slider(Label = "Display duration", Tooltip = "How long the healthbar remains visible after taking damage (in seconds).",
            DefaultValue = 5.0f, Min = 1.0f, Max = 15.0f, Step = 1.0f, Format = "{0:0.0}s")]
        public float DisplayDuration = 5.0f;

        [Toggle(Label = "Show health numbers", Tooltip = "Display current/maximum health values on the health bar.")]
        public bool ShowHealthNumbers = false;

        [Slider(Label = "Size multiplier", Tooltip = "Controls the overall size of the health bar.",
            DefaultValue = 1.0f, Min = 0.5f, Max = 2.0f, Step = 0.1f, Format = "{0:0.0}")]
        public float SizeMultiplier = 1.0f;

        [Slider(Label = "Bar ratio", Tooltip = "Width-to-height ratio of the health bar. Higher values make wider, thinner bars.",
            DefaultValue = 8.0f, Min = 4.0f, Max = 16.0f, Step = 1.0f, Format = "{0:0.0}")]
        public float BarRatio = 8.0f;

        [Toggle("<color=#f1c353>Health color</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool HealthColorDivider;

        [ColorPicker(Label = "Preview", Tooltip = "The color of the health portion of the bar.", Advanced = true)]
        public Color HealthColor = new Color(1.0f, 0.1f, 0.1f, 0.85f); // Bright red

        [Toggle("<color=#f1c353>Background color</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool BackgroundColorDivider;

        [ColorPicker(Label = "Preview", Tooltip = "The color of the background portion of the bar.", Advanced = true)]
        public Color BackgroundColor = new Color(0.5f, 0.0f, 0.0f, 0.5f); // Dark red with transparency

        [Toggle("<color=#f1c353>Health numbers color</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool HealthNumbersColorDivider;

        [ColorPicker(Label = "Preview", Tooltip = "The color of the health numbers.", Advanced = true)]
        public Color HealthNumbersColor = Color.white;
    }
}