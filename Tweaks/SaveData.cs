using System.Collections.Generic;
using Nautilus.Json;

namespace Ungeziefi.Tweaks
{
    public class SaveData : SaveDataCache
    {
        public Dictionary<string, float> RotatedLadders { get; set; } = new Dictionary<string, float>();
    }
}