using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Ungeziefi.Tweaks
{
    [Menu("Tweaks")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Seamoth pushing", Tooltip = "Allows pushing a beached Seamoth.")]
        public bool SeamothPushing = true;

        [Slider(Label = "Build time multiplier", Tooltip = "Multiplier for the build time of structures.", DefaultValue = 1, Min = 1, Max = 10, Step = 1)]
        public float BuildTimeMultiplier = 1f;

        [Slider(Label = "Craft time multiplier", Tooltip = "Multiplier for the craft time of items.", DefaultValue = 1, Min = 1, Max = 10, Step = 1)]
        public float CraftTimeMultiplier = 1f;

        [Slider(Label = "Minimum creature size", Tooltip = "Minimum size multiplier for certain creatures (1-10, divided by 10 for the actual value).", DefaultValue = 5, Min = 1, Max = 10, Step = 1)]
        public int MinCreatureSize = 5;

        [Slider(Label = "Maximum creature size", Tooltip = "Maximum size multiplier for certain creatures (10-20, divided by 10 for the actual value).", DefaultValue = 15, Min = 10, Max = 20, Step = 1)]
        public int MaxCreatureSize = 15;

        [Toggle(Label = "Eating underwater", Tooltip = "Whether eating underwater is allowed or not.")]
        public bool EatingUnderwater = true;

        [Toggle(Label = "Medkits underwater", Tooltip = "Whether using medkits underwater is allowed or not.")]
        public bool MedkitsUnderwater = true;

        [Toggle(Label = "Chair swivelling", Tooltip = "Whether you can swivel the chair or not.")]
        public bool ChairSwivelling = true;

        [Toggle(Label = "Bladderfish tooltip", Tooltip = "Adds a tooltip about the Bladderfish providing oxygen if consumed raw.")]
        public bool BladderfishTooltip = true;
    }
}