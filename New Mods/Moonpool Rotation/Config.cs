using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Ungeziefi.Moonpool_Rotation
{
    [Menu("Moonpool Rotation")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Enable feature")]
        public bool EnableFeature = true;

        [Slider(Label = "Wait before rotation",
        Tooltip = "Time in seconds to wait before the moonpool rotates back to its original position. Default is recommended.",
        DefaultValue = 1.8f, Min = 0.5f, Max = 5.0f, Step = 0.1f, Format = "{0:0.0}s")]
        public float WaitBeforeRotation = 1.8f;

        [Slider(Label = "Return rotation duration",
                Tooltip = "Time in seconds for the moonpool to rotate back to its original position. Default is recommended.",
                DefaultValue = 1.5f, Min = 0.5f, Max = 5.0f, Step = 0.1f, Format = "{0:0.0}s")]
        public float ReturnRotationDuration = 1.5f;
    }
}