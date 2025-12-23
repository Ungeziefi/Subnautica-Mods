using System.Collections.Generic;
using Nautilus.Json;

namespace Ungeziefi.Fixes
{
    public class SaveData : SaveDataCache
    {
        public HashSet<string> OpenWreckDoors { get; set; } = new();
        public HashSet<string> CyclopsClosedDoors { get; set; } = new();
        public Dictionary<string, int> CyclopsSpeedMode { get; set; } = new();
        public HashSet<string> CyclopsesWithInternalLightOff { get; set; } = new();
        public HashSet<string> CyclopsesWithFloodlightsOn { get; set; } = new();
        //public bool SeaglideLightOn { get; set; }
        //public bool SeaglideMapOff { get; set; }
        public int LastHeldItemSlot { get; set; } = -1;
    }
}