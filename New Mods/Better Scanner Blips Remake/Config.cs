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

        [Toggle("<color=#f1c353>Colors</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool ColorsDivider;

        [Toggle(Label = "Use custom blip color", Tooltip = "Enable to use a custom color for scanner blips.")]
        [OnChange(nameof(OnColorSettingChanged))]
        public bool UseCustomBlipColor = false;

        [ColorPicker(Label = "Blip color")]
        [OnChange(nameof(OnColorSettingChanged))]
        public Color BlipColor = new Color(1.00f, 0.64f, 0.00f, 1.00f);

        [Toggle(Label = "Use custom text color", Tooltip = "Enable to use a different color for text than for blips.")]
        [OnChange(nameof(OnColorSettingChanged))]
        public bool UseCustomTextColor = false;

        [ColorPicker(Label = "Text color")]
        [OnChange(nameof(OnColorSettingChanged))]
        public Color TextColor = new Color(1.00f, 0.68f, 0.00f, 1.00f);

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
        private void OnColorSettingChanged()
        {
            // Update color cache when any color setting changes
            ColorManagement.UpdateColorCache();
        }

        // Returns the final blip color (using ORIGINAL color if custom color is disabled)
        public Color GetBlipColor()
        {
            return UseCustomBlipColor ? BlipColor : ColorManagement.originalBlipColor;
        }

        // Returns the final text color (using ORIGINAL color if custom color is disabled)
        public Color GetTextColor()
        {
            return UseCustomTextColor ? TextColor : ColorManagement.originalTextColor;
        }
        #endregion
    }
}