using System.Collections.Generic;
using Nautilus.Json;

namespace Ungeziefi.Tweaks
{
    public class SaveData : SaveDataCache
    {
        public HashSet<string> PRAWNSuitsWithLightOff { get; } = new();
    }
}