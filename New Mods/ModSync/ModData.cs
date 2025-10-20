using System;
using System.Collections.Generic;

namespace Ungeziefi.ModSync
{
    [Serializable]
    public class ModInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }

        public ModInfo() { }

        public ModInfo(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public override bool Equals(object obj)
        {
            return obj is ModInfo other && Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name?.GetHashCode() ?? 0;
        }
    }

    [Serializable]
    public class ModListData
    {
        public int NumberOfMods { get; set; }
        public List<ModInfo> Mods { get; set; } = new List<ModInfo>();
    }

    public class ModComparison
    {
        public List<ModInfo> Added { get; } = new List<ModInfo>();
        public List<ModInfo> Removed { get; } = new List<ModInfo>();
        public List<(ModInfo Old, ModInfo New)> VersionChanged { get; } = new List<(ModInfo, ModInfo)>();
        public bool HasDifferences => Added.Count > 0 || Removed.Count > 0 || VersionChanged.Count > 0;
    }
}