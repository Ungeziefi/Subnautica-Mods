using System.Collections.Generic;
using Nautilus.Json;

namespace Ungeziefi.Rotatable_Ladders
{
    public class SaveData : SaveDataCache
    {
        public Dictionary<string, float> RotatedLadders { get; set; } = new Dictionary<string, float>();
    }
}