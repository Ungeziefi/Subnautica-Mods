using Nautilus.Json;
using System.Collections.Generic;

namespace Ungeziefi.Tweaks
{
    public class SaveData : SaveDataCache
    {
        public HashSet<string> PRAWNSuitsWithLightOff { get; set; } = new();
    }
}