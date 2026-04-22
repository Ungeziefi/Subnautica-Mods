using System;
using System.Collections.Generic;

namespace Ungeziefi.ModSync;

[Serializable]
public class ModInfo
{
    public ModInfo()
    {
    }

    public ModInfo(string name, string version)
    {
        Name = name;
        Version = version;
    }

    public string Name { get; set; }
    public string Version { get; set; }
}

[Serializable]
public class ModListData
{
    public int NumberOfMods { get; set; }
    public List<ModInfo> Mods { get; set; } = new();
}

public class ModComparison
{
    public List<ModInfo> Added { get; } = new();
    public List<ModInfo> Removed { get; } = new();
    public List<(ModInfo Old, ModInfo New)> VersionChanged { get; } = new();
    public bool HasDifferences => Added.Count > 0 || Removed.Count > 0 || VersionChanged.Count > 0;
}