using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Ungeziefi.Dynamic_Scanner_Blips
{
    [Menu("Dynamic Scanner Blips")]
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

        [Toggle(Label = "Show distance")]
        public bool ShowDistance = true;
    }
}