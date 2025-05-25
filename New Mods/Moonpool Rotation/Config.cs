using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Ungeziefi.Moonpool_Rotation
{
    [Menu("Moonpool Rotation")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Enable feature")]
        public bool EnableFeature = true;

        [Toggle(Label = "Use advanced rotation", Tooltip = "When enabled, the moonpool will rotate to match the vehicle's direction instead of just flipping 180 degrees.")]
        public bool UseAdvancedRotation = true;

        [Slider(Label = "Wait before rotation", DefaultValue = 1.8f, Min = 0.5f, Max = 5.0f, Step = 0.1f, Format = "{0:0.0}s")]
        public float WaitBeforeRotation = 1.8f;

        [Slider(Label = "Maximum return rotation duration", DefaultValue = 1.5f, Min = 0.5f, Max = 5.0f, Step = 0.1f, Format = "{0:0.0}s")]
        public float MaxReturnRotationDuration = 1.5f;

        [Slider(Label = "Minimum return rotation duration", DefaultValue = 1.2f, Min = 0.2f, Max = 2.0f, Step = 0.1f, Format = "{0:0.0}s",
            Tooltip = "Prevents small rotations from being too fast.")]
        public float MinReturnRotationDuration = 1.2f;
    }
}