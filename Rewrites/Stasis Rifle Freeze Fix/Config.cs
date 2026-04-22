using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Ungeziefi.Stasis_Rifle_Freeze_Fix;

[Menu("Stasis Rifle Freeze Fix")]
public class Config : ConfigFile
{
    [Toggle(Label = "Enable feature")] public bool EnableFeature = true;
}