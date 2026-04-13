using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Ungeziefi.Drillable_Scan_Remake
{
    [Menu("Drillable Scan Remake")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Enable feature", Tooltip = "Default settings prevent clipping with pinned recipes.")]
        public bool EnableFeature = true;
    }
}