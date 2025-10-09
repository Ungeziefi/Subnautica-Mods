using Nautilus.Json;
using System.Collections.Generic;

namespace Ungeziefi.Cuddlefish_Renamer
{
    public class SaveData : SaveDataCache
    {
        public Dictionary<string, string> CuddlefishNames { get; set; } = new Dictionary<string, string>();
    }
}