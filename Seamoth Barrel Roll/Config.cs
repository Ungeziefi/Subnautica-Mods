using Nautilus.Json;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace Ungeziefi.Seamoth_Barrel_Roll
{
    public enum StabilizationMode
    {
        Disabled,
        Normal,
        OnlyWhenEmpty
    }

    [Menu("Seamoth Barrel Roll")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Enable feature")]
        public bool EnableFeature = true;

        [Choice(Label = "Stabilization mode",
               Tooltip = "Disabled: No stabilization at all.\nNormal: Default game behaviour.\nOnly when empty: Only stabilizes when exiting the Seamoth.",
                Options = new[] { "Disabled", "Normal", "Only when empty" })]
        public StabilizationMode StabilizationMode = StabilizationMode.OnlyWhenEmpty;

        [Keybind(Label = "Roll left key")]
        public KeyCode RollLeftKey = KeyCode.LeftControl;

        [Keybind(Label = "Roll right key")]
        public KeyCode RollRightKey = KeyCode.LeftAlt;

        [Slider(Label = "Roll force", Tooltip = "How strong the rolling motion is.", DefaultValue = 20f, Min = 10f, Max = 50f, Step = 1)]
        public float RollForce = 20f;

        [Slider(Label = "Roll acceleration", Tooltip = "How quickly the roll builds up.", DefaultValue = 20f, Min = 10f, Max = 100f, Step = 1)]
        public float RollAcceleration = 20f;

        [Toggle(Label = "Allow airborne rolling")]
        public bool AllowAirborneRolling = false;
    }
}