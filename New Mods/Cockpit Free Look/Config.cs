using Nautilus.Json;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace Ungeziefi.Cockpit_Free_Look
{
    [Menu("Cockpit Free Look")]
    public class Config : ConfigFile
    {
        [Toggle("<color=#f1c353>General Settings</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool GeneralSettingsDivider;

        [Keybind(Label = "Free look key", Tooltip = "F key by default.")]
        public KeyCode FreeLookKey = KeyCode.F;

        [Keybind(Label = "Secondary free look key", Tooltip = "Controller A button by default.")]
        public KeyCode SecondaryFreeLookKey = KeyCode.JoystickButton0;

        [Toggle(Label = "Hold key mode", Tooltip = "When enabled, you must hold the key to free look instead of toggling it.")]
        public bool HoldKeyMode = false;

        [Slider(Label = "Return duration", Tooltip = "How long it takes to return to the locked camera position when disabling free look.",
            DefaultValue = 0.3f, Min = 0f, Max = 2.0f, Step = 0.1f, Format = "{0:0.0}s")]
        public float FreeLookReturnDuration = 0.3f;

        [Slider(Label = "Look sensitivity", DefaultValue = 1.0f, Min = 0.1f, Max = 5.0f, Step = 0.1f, Format = "{0:0.0}")]
        public float FreeLookSensitivity = 1.0f;

        [Toggle("<color=#f1c353>Seamoth</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool SeamothDivider;

        [Toggle(Label = "Enable feature")]
        public bool SeamothEnableFeature = true;

        [Slider(Label = "Horizontal angle limit", DefaultValue = 80f, Min = 10f, Max = 80f, Step = 1f, Format = "{0:0}°")]
        public float SeamothHorizontalLimit = 80f;

        [Slider(Label = "Vertical angle limit", DefaultValue = 45f, Min = 10f, Max = 45f, Step = 1f, Format = "{0:0}°")]
        public float SeamothVerticalLimit = 45f;

        [Slider(Label = "Camera tilt angle", Tooltip = "Maximum tilt angle when looking to the sides.",
            DefaultValue = 10f, Min = 0f, Max = 15f, Step = 1f, Format = "{0:0}°")]
        public float CameraTiltAngle = 10f;

        [Toggle("<color=#f1c353>PRAWN Suit</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool PRAWNSuitDivider;

        [Toggle(Label = "Enable feature")]
        public bool PRAWNEnableFeature = true;

        [Toggle(Label = "Lock PRAWN arms", Tooltip = "Prevents the PRAWN Suit's arms from moving during free look.")]
        public bool LockPRAWNArms = true;

        [Slider(Label = "Horizontal angle limit", DefaultValue = 60f, Min = 10f, Max = 80f, Step = 1f, Format = "{0:0}°")]
        public float PRAWNAngleLimit = 60f;
    }
}