using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Ungeziefi.Tweaks
{
    [Menu("Tweaks")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Enable Push Seamoth", Tooltip = "Whether the push feature for the Seamoth is enabled or not.")]
        public bool EnablePushSeamoth = true;

        [Slider(Label = "Build Time Multiplier", Tooltip = "Multiplier for the build time of structures.", DefaultValue = 1, Min = 1, Max = 10, Step = 1)]
        public float BuildTimeMultiplier = 1f;

        [Slider(Label = "Craft Time Multiplier", Tooltip = "Multiplier for the craft time of items.", DefaultValue = 1, Min = 1, Max = 10, Step = 1)]
        public float CraftTimeMultiplier = 1f;

        [Slider(Label = "Min Creature Size", Tooltip = "Minimum size multiplier for creatures (1-10, divided by 10 for actual value).", DefaultValue = 5, Min = 1, Max = 10, Step = 1)]
        public int MinCreatureSize = 5;

        [Slider(Label = "Max Creature Size", Tooltip = "Maximum size multiplier for creatures (10-20, divided by 10 for actual value).", DefaultValue = 15, Min = 10, Max = 20, Step = 1)]
        public int MaxCreatureSize = 15;

        [Toggle(Label = "Disable Eating Underwater", Tooltip = "Whether eating underwater is disabled or not.")]
        public bool DisableEatingUnderwater = true;

        [Toggle(Label = "Disable Using Medkits Underwater", Tooltip = "Whether using medkits underwater is disabled or not.")]
        public bool DisableUsingMedkitsUnderwater = true;
    }
}