using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Ungeziefi.Abandon_Ship_During_Cyclops_Fire
{
    [Menu("Abandon Ship During Cyclops Fire")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Enable feature")]
        public bool EnableFeature = true;
    }
}