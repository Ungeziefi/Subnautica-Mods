using Nautilus.Json;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace Ungeziefi.Cuddlefish_Renamer
{
    [Menu("Cuddlefish Renamer")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Enable feature")]
        public bool EnableFeature = true;

        [Keybind(Label = "Rename key", Tooltip = "Configurable key in addition to Alt Tool when using a controller.")]
        public KeyCode RenameKey = KeyCode.R;

        [Toggle(Label = "Show name above")]
        public bool ShowNameAbove = true;

        [Toggle(Label = "Custom play prompt", Tooltip = "Show 'Play With [Name]' instead of 'Play With Fish'.")]
        public bool CustomPlayPrompt = true;

        [Slider(Label = "Name label height", Tooltip = "Height of the name label above the Cuddlefish.",
            DefaultValue = 0.5f, Min = 0.2f, Max = 1.0f, Step = 0.1f, Format = "{0:0.0}")]
        public float NameLabelHeight = 0.5f;

        [Toggle(Label = "Bold text")]
        public bool BoldText = false;

        [Slider(Label = "Font size", DefaultValue = 2f, Min = 1f, Max = 5f, Step = 0.1f, Format = "{0:0.0}")]
        public float NameFontSize = 2f;

        [Slider(Label = "Maximum name length", Tooltip = "Maximum number of characters allowed while naming a Cuddlefish.",
            DefaultValue = 25, Min = 10, Max = 50, Step = 5)]
        public int MaxNameLength = 25;

        [Toggle(Label = "Fade name with distance", Tooltip = "Gradually fade the name opacity based on distance.")]
        public bool FadeWithDistance = true;

        [Slider(Label = "Fade start distance", Tooltip = "Distance at which names begin fading out. Names are fully visible within this range and completely invisible at twice this distance.",
            DefaultValue = 10f, Min = 5f, Max = 45f, Step = 1f, Format = "{0:0}m")]
        public float FadeStartDistance = 10f;

        [Toggle("<color=#f1c353>Name color</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool NameColorDivider;

        [ColorPicker(Label = "Preview", Advanced = true)]
        public Color NameColor = Color.white;
    }
}