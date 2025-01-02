using System.Collections.Generic;
using Nautilus.Json;

namespace Ungeziefi.Fixes
{
    public class SaveData : SaveDataCache
    {
        public Dictionary<string, bool> OpenWreckDoors = new Dictionary<string, bool>();
    }
}