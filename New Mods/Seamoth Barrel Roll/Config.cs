using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Ungeziefi.Seamoth_Barrel_Roll
{
    public enum StabilizationMode
    {
        Disabled,
        Normal,
        OnlyWhenUnpiloted,
        OnlyWhenNotRolling,
        OnlyWhenIdle
    }

    [Menu("Seamoth Barrel Roll")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Enable feature")]
        public bool EnableFeature = true;

        [Choice(Label = "Stabilization mode",
               Tooltip =
            "Disabled: No stabilization at all.\n" +
            "Normal: Default game behaviour.\n" +
            "Only when unpiloted: Only stabilizes when exiting the Seamoth.\n" +
            "Only when not rolling: Only stabilizes when not rolling" +
            "Only when idle: Only stabilizes when not rolling nor moving.",
                Options = new[] { "Disabled", "Normal", "Only when not piloting", "Only when not rolling", "Only when idle" })]
        public StabilizationMode StabilizationMode = StabilizationMode.OnlyWhenUnpiloted;

        [Slider(Label = "Roll force", Tooltip = "How strong the rolling motion is.", DefaultValue = 20f, Min = 10f, Max = 50f, Step = 1)]
        public float RollForce = 20f;

        [Slider(Label = "Roll acceleration", Tooltip = "How quickly the roll builds up.", DefaultValue = 20f, Min = 10f, Max = 100f, Step = 1)]
        public float RollAcceleration = 20f;

        [Toggle(Label = "Allow airborne rolling")]
        public bool AllowAirborneRolling = false;

        [Toggle(Label = "Rolling requires power")]
        public bool RollingRequiresPower = true;

        [Toggle(Label = "Stabilization requires power")]
        public bool StabilizationRequiresPower = true;

        [Toggle(Label = "Star Fox sound", Tooltip = "Plays the Star Fox \"Do a barrel roll\" when pressing down the roll keys.")]
        public bool StarFoxSound = false;
    }
}