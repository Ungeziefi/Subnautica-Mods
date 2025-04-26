using Nautilus.Json;
using Nautilus.Options.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace Ungeziefi.Better_Scanner_Blips_Remake
{
    [Menu("Better Scanner Blips Remake")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Enable feature")]
        public bool EnableFeature = true;

        [Slider(Label = "Maximum range", DefaultValue = 150f, Min = 10f, Max = 300f, Step = 1f)]
        public float MaximumRange = 150f;

        [Slider(Label = "Minimum scale", DefaultValue = 0.3f, Min = 0.1f, Max = 1f, Step = 0.1f, Format = "{0:0.0}")]
        public float MinimumScale = 0.3f;

        [Slider(Label = "Distant blip alpha", DefaultValue = 0.3f, Min = 0f, Max = 1f, Step = 0.1f, Format = "{0:0.0}")]
        public float DistantAlpha = 0.3f;

        [Toggle("<color=#f1c353>Visibility</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool VisibilityDivider;

        [Choice(Label = "Text visibility",
            Options = new[] { "Default", "Hide resource name", "Hide distance", "Hide both" },
            Tooltip = "Controls what text is displayed on resource blips")]
        public string TextVisibility = "Default";

        [Keybind(Label = "Toggle blips hotkey", Tooltip = "Press this key to toggle blips on and off.")]
        public KeyCode ToggleBlipsKey = KeyCode.B;

        [Toggle(Label = "Range-based toggle", Tooltip = "When enabled, the hotkey will only toggle blips within the range specified below.")]
        public bool RangeBasedToggle = false;

        [Slider(Label = "Toggle range", Tooltip = "Blips within this range will be toggled when using the hotkey with range-based toggle enabled.",
            DefaultValue = 15f, Min = 1f, Max = 100f, Step = 1f)]
        public float ToggleRange = 15f;

        [Toggle(Label = "Auto-hide nearby blips", Tooltip = "Automatically hide blips within a configurable distance (does not apply when range-based toggle is enabled).")]
        public bool AutoHideNearbyBlips = true;

        [Slider(Label = "Auto-hide distance", Tooltip = "Blips within this distance will be automatically hidden when auto-hide is enabled.",
            DefaultValue = 15f, Min = 1f, Max = 100f, Step = 1f)]
        public float AutoHideDistance = 15f;

        [Toggle("<color=#f1c353>Blip color</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool BlipColorDivider;

        [Toggle(Label = "Custom blip color", Tooltip = "Enable to use custom colors for scanner blips.")]
        [OnChange(nameof(UseCustomBlipColorChanged))]
        public bool UseCustomBlipColor = false;

        [Choice(Label = "Color presets", Tooltip = "Select a predefined color or use custom RGB values below.",
            Options = new[] { "Custom", "Red", "Green", "Blue", "Yellow", "Cyan", "Magenta", "White" })]
        [OnChange(nameof(ColorPresetChanged))]
        public string ColorPreset = "Custom";

        [Slider(Label = "Blip color (Red)", DefaultValue = 255f, Min = 0f, Max = 255f, Step = 1f, Format = "{0:0}")]
        public float BlipColorRed = 255f;

        [Slider(Label = "Blip color (Green)", DefaultValue = 255f, Min = 0f, Max = 255f, Step = 1f, Format = "{0:0}")]
        public float BlipColorGreen = 255f;

        [Slider(Label = "Blip color (Blue)", DefaultValue = 255f, Min = 0f, Max = 255f, Step = 1f, Format = "{0:0}")]
        public float BlipColorBlue = 255f;

        [Toggle("<color=#f1c353>Text color</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool TextColorDivider;

        [Toggle(Label = "Custom text color", Tooltip = "Enable to use different colors for text than for blips.")]
        [OnChange(nameof(UseCustomTextColorChanged))]
        public bool UseCustomTextColor = false;

        [Choice(Label = "Text color presets", Tooltip = "Select a predefined color or use custom RGB values below.",
            Options = new[] { "Custom", "Red", "Green", "Blue", "Yellow", "Cyan", "Magenta", "White" })]
        [OnChange(nameof(TextColorPresetChanged))]
        public string TextColorPreset = "Custom";

        [Slider(Label = "Text color (Red)", DefaultValue = 255f, Min = 0f, Max = 255f, Step = 1f, Format = "{0:0}")]
        public float TextColorRed = 255f;

        [Slider(Label = "Text color (Green)", DefaultValue = 255f, Min = 0f, Max = 255f, Step = 1f, Format = "{0:0}")]
        public float TextColorGreen = 255f;

        [Slider(Label = "Text color (Blue)", DefaultValue = 255f, Min = 0f, Max = 255f, Step = 1f, Format = "{0:0}")]
        public float TextColorBlue = 255f;

        #region Color Management
        private static readonly Dictionary<string, Vector3> ColorPresets = new()
        {
            { "Red",     new Vector3(255, 0, 0) },
            { "Green",   new Vector3(0, 255, 0) },
            { "Blue",    new Vector3(0, 0, 255) },
            { "Yellow",  new Vector3(255, 255, 0) },
            { "Cyan",    new Vector3(0, 255, 255) },
            { "Magenta", new Vector3(255, 0, 255) },
            { "White",   new Vector3(255, 255, 255) }
        };

        // Called when "Custom blip color" toggle changes
        private void UseCustomBlipColorChanged()
        {
            // When enabling custom colors, apply selected preset (if not "Custom")
            if (UseCustomBlipColor && ColorPreset != "Custom")
                ApplyColorPreset(ColorPreset, ref BlipColorRed, ref BlipColorGreen, ref BlipColorBlue);
        }

        // Called when "Custom text color" toggle changes
        private void UseCustomTextColorChanged()
        {
            // When enabling custom colors, apply selected preset (if not "Custom")
            if (UseCustomTextColor && TextColorPreset != "Custom")
                ApplyColorPreset(TextColorPreset, ref TextColorRed, ref TextColorGreen, ref TextColorBlue);
        }

        // Called when blip color preset choice changes
        private void ColorPresetChanged()
        {
            if (ColorPreset == "Custom") return;

            // Always apply the color preset when changed, regardless of whether custom color is enabled
            // This ensures values are ready when custom color is enabled
            ApplyColorPreset(ColorPreset, ref BlipColorRed, ref BlipColorGreen, ref BlipColorBlue);
        }

        // Called when text color preset choice changes
        private void TextColorPresetChanged()
        {
            if (TextColorPreset == "Custom") return;

            // Always apply the color preset when changed, regardless of whether custom color is enabled
            // This ensures values are ready when custom color is enabled
            ApplyColorPreset(TextColorPreset, ref TextColorRed, ref TextColorGreen, ref TextColorBlue);
        }

        // Helper method: Applies RGB values from a preset to color sliders
        private void ApplyColorPreset(string preset, ref float red, ref float green, ref float blue)
        {
            if (ColorPresets.TryGetValue(preset, out Vector3 color))
            {
                red = color.x;
                green = color.y;
                blue = color.z;
            }
        }

        // Returns the final blip color (white if custom color is disabled)
        public Color GetBlipColor()
        {
            if (!UseCustomBlipColor)
                return Color.white;

            // Convert 0-255 range to 0-1 range needed by Unity's Color
            return new Color(BlipColorRed / 255f, BlipColorGreen / 255f, BlipColorBlue / 255f);
        }

        // Returns the final text color (white if custom color is disabled)
        public Color GetTextColor()
        {
            if (!UseCustomTextColor)
                return Color.white;

            // Convert 0-255 range to 0-1 range needed by Unity's Color
            return new Color(TextColorRed / 255f, TextColorGreen / 255f, TextColorBlue / 255f);
        }
        #endregion
    }
}