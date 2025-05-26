using System.Collections.Generic;
using Nautilus.Json;

namespace Ungeziefi.Cuddlefish_Renamer
{
    public class SaveData : SaveDataCache
    {
        public Dictionary<string, string> CuddlefishNames { get; set; } = new Dictionary<string, string>();
    }
}