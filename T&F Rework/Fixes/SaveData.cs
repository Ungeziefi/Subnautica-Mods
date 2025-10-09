using Nautilus.Json;
using System.Collections.Generic;

namespace Ungeziefi.Fixes
{
    public class SaveData : SaveDataCache
    {
        public HashSet<string> OpenWreckDoors { get; } = new();
        public HashSet<string> CyclopsClosedDoors { get; } = new();
        public Dictionary<string, int> CyclopsSpeedMode { get; } = new();
        public HashSet<string> CyclopsesWithInternalLightOff { get; } = new();
        public HashSet<string> CyclopsesWithFloodlightsOn { get; } = new();
        //public bool SeaglideLightOn { get; set; }
        //public bool SeaglideMapOff { get; set; }
        public int LastHeldItemSlot { get; set; } = -1;
    }
}