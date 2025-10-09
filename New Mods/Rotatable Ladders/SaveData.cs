using Nautilus.Json;
using System.Collections.Generic;

namespace Ungeziefi.Rotatable_Ladders
{
    public class SaveData : SaveDataCache
    {
        public Dictionary<string, float> RotatedLaddersBottom { get; set; } = new Dictionary<string, float>();
        public Dictionary<string, float> RotatedLaddersTop { get; set; } = new Dictionary<string, float>();
    }
}