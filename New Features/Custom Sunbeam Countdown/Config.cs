using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Ungeziefi.Custom_Sunbeam_Countdown
{
    [Menu("Custom Sunbeam Countdown")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Enable feature", Tooltip = "Default settings prevent clipping with pinned recipes.")]
        public bool EnableFeature = true;

        [Slider(Label = "X position", DefaultValue = 1f, Min = 0f, Max = 1f, Step = 0.01f, Format = "{0:0.00}")]
        public float XPosition = 1f;

        [Slider(Label = "Y position", DefaultValue = 1f, Min = 0f, Max = 1f, Step = 0.01f, Format = "{0:0.00}")]
        public float YPosition = 1f;

        [Slider(Label = "Scale", DefaultValue = 0.70f, Min = 0.5f, Max = 2f, Step = 0.01f, Format = "{0:0.00}")]
        public float Scale = 0.70f;
    }
}