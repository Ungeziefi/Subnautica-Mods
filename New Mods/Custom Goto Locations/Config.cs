using Nautilus.Json;
using Nautilus.Options.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace Ungeziefi.Custom_Goto_Locations
{
    [Menu("Custom Goto Locations")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Enable feature")]
        public bool EnableFeature = true;

        public class TeleportLocation
        {
            public string Name;
            public Vector3 Position;

            public TeleportLocation(string name, Vector3 position)
            {
                Name = name;
                Position = position;
            }
        }

        public List<TeleportLocation> CustomTeleportLocations = new();
    }
}