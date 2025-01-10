using System.Collections.Generic;
using Nautilus.Json;

namespace Ungeziefi.Fixes
{
    public class SaveData : SaveDataCache
    {
        public HashSet<string> OpenWreckDoors = new HashSet<string>();
        public HashSet<string> CyclopsClosedDoors = new HashSet<string>();
        public Dictionary<string, int> CyclopsSpeedMode = new Dictionary<string, int>();
        public HashSet<string> CyclopsesWithInternalLightOff = new HashSet<string>();
        public HashSet<string> CyclopsesWithFloodlightsOn = new HashSet<string>();
        public bool SeaglideLightOn;
        public bool SeaglideMapOff;
        public HashSet<string> PrawnSuitsWithLightOff = new HashSet<string>();
        public Dictionary<string, TechType> LastHeldTools = new Dictionary<string, TechType>();
    }
}