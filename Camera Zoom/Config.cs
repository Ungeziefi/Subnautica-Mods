using Nautilus.Json;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace Ungeziefi.Camera_Zoom
{
    [Menu("Camera Zoom")]
    public class Config : ConfigFile
    {
        [Toggle("<color=#f1c353>Cyclops cameras</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool CyclopsCamerasDivider;

        [Toggle(Label = "Enable feature")]
        public bool CCEnableFeature = true;

        [Keybind(Label = "Zoom in key", Tooltip = "Zoom in key in addition to moving forward.")]
        public KeyCode CCZoomInKey = KeyCode.LeftShift;

        [Keybind(Label = "Zoom out key", Tooltip = "Zoom in key in addition to moving backward.")]
        public KeyCode CCZoomOutKey = KeyCode.LeftControl;

        [Slider(Label = "Minimum FOV", DefaultValue = 10f, Min = 10f, Max = 60f, Step = 1)]
        public float CCMinimumFOV = 10f;

        [Slider(Label = "Maximum FOV", DefaultValue = 90f, Min = 60f, Max = 90f, Step = 1)]
        public float CCMaximumFOV = 90f;

        [Slider(Label = "Zoom speed", DefaultValue = 30f, Min = 1f, Max = 100f, Step = 1)]
        public float CCZoomSpeed = 30f;

        [Toggle("<color=#f1c353>Player camera</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool PlayerCameraDivider;

        [Toggle(Label = "Enable feature")]
        public bool PCEnableFeature = true;

        [Toggle(Label = "Instant zoom")]
        public bool PCInstantZoom = false;

        [Toggle(Label = "Allow vehicle zoom", Tooltip = "Allows zooming while piloting the Seamoth or PRAWN Suit.")]
        public bool PCAllowVehicleZoom = false;

        [Keybind(Label = "Zoom key", Tooltip = "Scroll wheel click by default.")]
        public KeyCode PCZoomKey = KeyCode.Mouse2;

        [Keybind(Label = "Secondary zoom key", Tooltip = "Controller left thumb stick click by default.")]
        public KeyCode PCSecondaryZoomKey = KeyCode.JoystickButton8;

        [Slider(Label = "Target FOV", DefaultValue = 20f, Min = 10f, Max = 40f, Step = 1)]
        public float PCTargetFOV = 20f;

        [Slider(Label = "Zoom speed", DefaultValue = 4f, Min = 1f, Max = 10f, Step = 1)]
        public float PCZoomSpeed = 4f;
    }
}