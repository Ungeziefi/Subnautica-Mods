﻿using Nautilus.Json;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace Ungeziefi.Rotatable_Ladders
{
    [Menu("Rotatable Ladders")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Enable feature")]
        public bool EnableFeature = true;

        [Toggle(Label = "Empty hands only", Tooltip = "When enabled, the rotation prompt will only appear when you're not holding any tools or items.")]
        public bool EmptyHandsOnly = true;

        [Keybind(Label = "Rotate ladder key", Tooltip = "Configurable key in addition to Alt Tool when using a controller.")]
        public KeyCode RotateLadderKey = KeyCode.R;
    }
}