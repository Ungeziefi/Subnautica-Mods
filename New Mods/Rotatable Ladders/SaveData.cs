using System.Collections.Generic;
using Nautilus.Json;

namespace Ungeziefi.Rotatable_Ladders
{
    public class SaveData : SaveDataCache
    {
        public Dictionary<string, float> RotatedLaddersBottom { get; set; } = new Dictionary<string, float>();
        public Dictionary<string, float> RotatedLaddersTop { get; set; } = new Dictionary<string, float>();
    }
}