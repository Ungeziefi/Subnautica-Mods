using System.Collections.Generic;
using Nautilus.Json;

namespace Ungeziefi.Fixes
{
    public class SaveData : SaveDataCache
    {
        public HashSet<string> OpenWreckDoors { get; } = new HashSet<string>();
        public HashSet<string> CyclopsClosedDoors { get; } = new HashSet<string>();
        public Dictionary<string, int> CyclopsSpeedMode { get; } = new Dictionary<string, int>();
        public HashSet<string> CyclopsesWithInternalLightOff { get; } = new HashSet<string>();
        public HashSet<string> CyclopsesWithFloodlightsOn { get; } = new HashSet<string>();
        public bool SeaglideLightOn { get; set; }
        public bool SeaglideMapOff { get; set; }
    }
}