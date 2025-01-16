using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Ungeziefi.Dynamic_Scanner_Blips
{
    [Menu("Dynamic Scanner Blips")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Enable feature")]
        public bool EnableFeature = true;

        [Slider(Label = "Maximum range", Min = 10f, Max = 300f, DefaultValue = 150f, Format = "{0:0.0}")]
        public float MaximumRange = 150f;

        [Slider(Label = "Minimum scale", Min = 0.1f, Max = 1f, DefaultValue = 0.3f, Format = "{0:0.0}")]
        public float MinimumScale = 0.3f;

        [Slider(Label = "Distant blip alpha", Min = 0f, Max = 1f, DefaultValue = 0.3f, Format = "{0:0.0}")]
        public float DistantAlpha = 0.3f;

        [Toggle(Label = "Show distance")]
        public bool ShowDistance = true;
    }
}