using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Ungeziefi.Moonpool_Rotation
{
    [Menu("Moonpool Rotation")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Enable feature")]
        public bool EnableFeature = true;
    }
}