using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Ungeziefi.Lose_Everything;

[Menu("Lose Everything")]
public class Config : ConfigFile
{
    [Toggle(Label = "Enable feature")] public bool EnableFeature = true;

    [Toggle(Label = "Lose items on death")]
    public bool LoseEquipmentOnDeath = true;

    [Toggle(Label = "Keep tools on death")]
    public bool KeepToolsOnDeath = true;
}