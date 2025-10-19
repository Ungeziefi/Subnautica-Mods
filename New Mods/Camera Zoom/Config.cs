using Nautilus.Json;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace Ungeziefi.Camera_Zoom
{
    [Menu("Camera Zoom")]
    public class Config : ConfigFile
    {
        [Toggle("<color=#f1c353>Player</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool PlayerDivider;

        [Toggle(Label = "Enable feature")]
        public bool PCEnableFeature = true;

        [Toggle(Label = "Instant zoom")]
        public bool PCInstantZoom = false;

        [Toggle(Label = "Allow while building")]
        public bool PCAllowWhileBuilding = false;

        [Slider(Label = "Target FOV", DefaultValue = 20f, Min = 10f, Max = 40f, Step = 1)]
        public float PCTargetFOV = 20f;

        [Slider(Label = "Zoom speed", DefaultValue = 4f, Min = 1f, Max = 10f, Step = 1)]
        public float PCZoomSpeed = 4f;

        [Toggle("<color=#f1c353>Vehicles</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool VehiclesDivider;

        [Toggle(Label = "Enable feature")]
        public bool VCEnableFeature = true;

        [Toggle(Label = "Instant zoom")]
        public bool VCInstantZoom = false;

        [Slider(Label = "Target FOV", DefaultValue = 20f, Min = 10f, Max = 40f, Step = 1)]
        public float VCTargetFOV = 20f;

        [Slider(Label = "Zoom speed", DefaultValue = 4f, Min = 1f, Max = 10f, Step = 1)]
        public float VCZoomSpeed = 4f;

        [Toggle("<color=#f1c353>Cyclops Cameras</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool CyclopsCamerasDivider;

        [Toggle(Label = "Enable feature")]
        public bool CCEnableFeature = true;

        [Toggle(Label = "Use stepped zoom", Tooltip = "When enabled, zoom changes in discrete steps rather than continuously.")]
        public bool CCSteppedZoom = false;

        [Slider(Label = "Number of zoom steps", DefaultValue = 3f, Min = 1f, Max = 6f, Step = 1f)]
        public float CCZoomSteps = 3f;

        [Toggle(Label = "Use blink effect", Tooltip = "When enabled, the screen briefly fades to black during stepped zoom transitions.")]
        public bool CCUseBlinkEffect = true;

        [Slider(Label = "Minimum FOV", DefaultValue = 10f, Min = 10f, Max = 60f, Step = 1)]
        public float CCMinimumFOV = 10f;

        [Slider(Label = "Maximum FOV", DefaultValue = 90f, Min = 60f, Max = 90f, Step = 1)]
        public float CCMaximumFOV = 90f;

        [Slider(Label = "Zoom speed", DefaultValue = 30f, Min = 1f, Max = 100f, Step = 1)]
        public float CCZoomSpeed = 30f;

        [Slider(Label = "Blink speed", DefaultValue = 5f, Min = 1f, Max = 20f, Step = 1f)]
        public float CCBlinkSpeed = 5f;

        [Toggle("<color=#f1c353>Camera Drones</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool CameraDronesDivider;

        [Toggle(Label = "Enable feature")]
        public bool CDEnableFeature = true;

        [Toggle(Label = "Use stepped zoom", Tooltip = "When enabled, zoom changes in discrete steps rather than continuously.")]
        public bool CDSteppedZoom = false;

        [Slider(Label = "Number of zoom steps", DefaultValue = 3f, Min = 1f, Max = 6f, Step = 1f)]
        public float CDZoomSteps = 3f;

        [Toggle(Label = "Use blink effect", Tooltip = "When enabled, the screen briefly fades to black during stepped zoom transitions.")]
        public bool CDUseBlinkEffect = true;

        [Slider(Label = "Minimum FOV", DefaultValue = 10f, Min = 10f, Max = 60f, Step = 1)]
        public float CDMinimumFOV = 10f;

        [Slider(Label = "Maximum FOV", DefaultValue = 90f, Min = 60f, Max = 90f, Step = 1)]
        public float CDMaximumFOV = 90f;

        [Slider(Label = "Zoom speed", DefaultValue = 30f, Min = 1f, Max = 100f, Step = 1)]
        public float CDZoomSpeed = 30f;

        [Slider(Label = "Blink speed", DefaultValue = 5f, Min = 1f, Max = 20f, Step = 1f)]
        public float CDBlinkSpeed = 5f;
    }
}