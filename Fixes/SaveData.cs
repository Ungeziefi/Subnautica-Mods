using System.Collections.Generic;
using Nautilus.Json;

namespace Ungeziefi.Fixes
{
    public class SaveData : SaveDataCache
    {
        public HashSet<string> OpenWreckDoors = new HashSet<string>();

        public HashSet<string> CyclopsClosedDoors = new HashSet<string>();

        public Dictionary<string, int> CyclopsSpeedMode = new Dictionary<string, int>();
    }
}