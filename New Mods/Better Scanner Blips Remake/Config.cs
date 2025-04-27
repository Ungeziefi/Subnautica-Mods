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

        [Choice(Label = "Text content visibility",
            Options = new[] { "Default", "Hide resource name", "Hide distance", "Hide both" },
            Tooltip = "Controls what text is displayed on resource blips")]
        public string TextVisibility = "Default";

        [Toggle(Label = "Limit text visibility by distance", Tooltip = "When enabled, blip text will only appear when closer than the distance specified below.")]
        public bool LimitTextVisibilityByDistance = true;

        [Slider(Label = "Text visibility distance", Tooltip = "Blip text will only appear when closer than this distance (when enabled above).",
            DefaultValue = 50f, Min = 10f, Max = 150f, Step = 5f)]
        public float TextVisibilityDistance = 50f;

        [Toggle(Label = "Auto-hide nearby blips", Tooltip = "Automatically hide blips within a configurable distance (does not apply when range-based toggle is enabled).")]
        public bool AutoHideNearbyBlips = true;

        [Slider(Label = "Auto-hide distance", Tooltip = "Blips within this distance will be automatically hidden when auto-hide is enabled.",
            DefaultValue = 10f, Min = 1f, Max = 100f, Step = 1f)]
        public float AutoHideDistance = 10f;

        [Toggle(Label = "Hide blips inside habitats")]
        public bool HideBlipsInsideHabitats = true;

        [Keybind(Label = "Toggle blips hotkey")]
        public KeyCode ToggleBlipsKey = KeyCode.B;

        [Toggle(Label = "Range-based toggle", Tooltip = "When enabled, the hotkey will only toggle blips within the range specified below.")]
        public bool RangeBasedToggle = false;

        [Slider(Label = "Toggle range", Tooltip = "Blips within this range will be toggled when using the hotkey with range-based toggle enabled.",
            DefaultValue = 20f, Min = 1f, Max = 100f, Step = 1f)]
        public float ToggleRange = 20f;

        [Toggle("<color=#f1c353>Blip limits</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool BlipLimitsDivider;

        [Toggle(Label = "Limit visible blips", Tooltip = "When enabled, only the specified number of blips will be visible.")]
        public bool LimitVisibleBlips = false;

        [Slider(Label = "Maximum visible blips", Tooltip = "Only this many blips will be shown at once.",
            DefaultValue = 6, Min = 1, Max = 20, Step = 1)]
        public int MaximumVisibleBlips = 6;

        [Toggle("<color=#f1c353>Blip color</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool BlipColorDivider;

        [Toggle(Label = "Custom blip color", Tooltip = "Enable to use custom colors for scanner blips.")]
        [OnChange(nameof(UseCustomBlipColorChanged))]
        public bool UseCustomBlipColor = false;

        [Choice(Label = "Color presets", Tooltip = "Select a predefined color or use custom RGB values below.",
            Options = new[] { "Custom", "Red", "Green", "Blue", "Yellow", "Cyan", "Magenta", "White" })]
        [OnChange(nameof(ColorPresetChanged))]
        public string ColorPreset = "Custom";

        [Slider(Label = "Blip color (Red)", DefaultValue = 255f, Min = 0f, Max = 255f, Step = 1f)]
        [OnChange(nameof(OnColorComponentChanged))]
        public float BlipColorRed = 255f;

        [Slider(Label = "Blip color (Green)", DefaultValue = 255f, Min = 0f, Max = 255f, Step = 1f)]
        [OnChange(nameof(OnColorComponentChanged))]
        public float BlipColorGreen = 255f;

        [Slider(Label = "Blip color (Blue)", DefaultValue = 255f, Min = 0f, Max = 255f, Step = 1f)]
        [OnChange(nameof(OnColorComponentChanged))]
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

        [Slider(Label = "Text color (Red)", DefaultValue = 255f, Min = 0f, Max = 255f, Step = 1f)]
        [OnChange(nameof(OnColorComponentChanged))]
        public float TextColorRed = 255f;

        [Slider(Label = "Text color (Green)", DefaultValue = 255f, Min = 0f, Max = 255f, Step = 1f)]
        [OnChange(nameof(OnColorComponentChanged))]
        public float TextColorGreen = 255f;

        [Slider(Label = "Text color (Blue)", DefaultValue = 255f, Min = 0f, Max = 255f, Step = 1f)]
        [OnChange(nameof(OnColorComponentChanged))]
        public float TextColorBlue = 255f;

        [Toggle("<color=#f1c353>Grouping</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool GroupingDivider;

        [Toggle(Label = "Group nearby resources", Tooltip = "Group resources of the same type that are very close together.")]
        public bool GroupNearbyResources = true;

        [Slider(Label = "Grouping distance", DefaultValue = 20f, Min = 5f, Max = 50f, Step = 1f,
            Tooltip = "Resources within this distance of each other will be grouped.")]
        public float GroupingDistance = 20f;

        [Toggle(Label = "Break groups when nearby", Tooltip = "Show individual resources instead of groups when you get close to them.")]
        public bool BreakGroupsWhenNearby = true;

        [Slider(Label = "Group breaking distance", DefaultValue = 40f, Min = 10f, Max = 100f, Step = 5f,
            Tooltip = "Groups will break into individual resources when you're closer than this distance.")]
        public float GroupBreakingDistance = 40f;

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

            // Update color cache
            ColorManagement.UpdateColorCache();
        }

        // Called when "Custom text color" toggle changes
        private void UseCustomTextColorChanged()
        {
            // When enabling custom colors, apply selected preset (if not "Custom")
            if (UseCustomTextColor && TextColorPreset != "Custom")
                ApplyColorPreset(TextColorPreset, ref TextColorRed, ref TextColorGreen, ref TextColorBlue);

            // Update color cache
            ColorManagement.UpdateColorCache();
        }

        // Called when blip color preset choice changes
        private void ColorPresetChanged()
        {
            if (ColorPreset == "Custom") return;

            // Always apply the color preset when changed, regardless of whether custom color is enabled
            // This ensures values are ready when custom color is enabled
            ApplyColorPreset(ColorPreset, ref BlipColorRed, ref BlipColorGreen, ref BlipColorBlue);

            // Update color cache if using custom colors
            if (UseCustomBlipColor)
                ColorManagement.UpdateColorCache();
        }

        // Called when text color preset choice changes
        private void TextColorPresetChanged()
        {
            if (TextColorPreset == "Custom") return;

            // Always apply the color preset when changed, regardless of whether custom color is enabled
            // This ensures values are ready when custom color is enabled
            ApplyColorPreset(TextColorPreset, ref TextColorRed, ref TextColorGreen, ref TextColorBlue);

            // Update color cache if using custom colors
            if (UseCustomTextColor)
                ColorManagement.UpdateColorCache();
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

        // Called when any color component sliders change
        public void OnColorComponentChanged()
        {
            // Update the color cache when the user moves any color slider
            ColorManagement.UpdateColorCache();
        }

        // Returns the final blip color (using ORIGINAL color if custom color is disabled)
        public Color GetBlipColor()
        {
            if (!UseCustomBlipColor)
                return ColorManagement.originalBlipColor;

            // Convert 0-255 range to 0-1 range needed by Unity's Color
            return new Color(BlipColorRed / 255f, BlipColorGreen / 255f, BlipColorBlue / 255f);
        }

        // Returns the final text color (using ORIGINAL color if custom color is disabled)
        public Color GetTextColor()
        {
            if (!UseCustomTextColor)
                return ColorManagement.originalTextColor;

            // Convert 0-255 range to 0-1 range needed by Unity's Color
            return new Color(TextColorRed / 255f, TextColorGreen / 255f, TextColorBlue / 255f);
        }
        #endregion
    }
}