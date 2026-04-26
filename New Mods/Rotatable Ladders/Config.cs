using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Ungeziefi.Rotatable_Ladders;

[Menu("Rotatable Ladders")]
public class Config : ConfigFile
{
    [Toggle(Label = "Enable feature")] public bool EnableFeature = true;

    [Toggle(Label = "Empty hands only",
        Tooltip = "When enabled, the rotation prompt will only appear when you're not holding any tools or items.")]
    public bool EmptyHandsOnly = true;

    [Toggle(Label = "Affect connected ladder")]
    public bool AffectConnectedLadder = true;

    [Toggle(Label = "Smooth rotation")] public bool SmoothRotation = true;

    [Slider(Label = "Rotation duration", DefaultValue = 1f, Min = 0.1f, Max = 5.0f, Step = 0.1f,
        Format = "{0:0.0}s")]
    public float RotationDuration = 1f;
}