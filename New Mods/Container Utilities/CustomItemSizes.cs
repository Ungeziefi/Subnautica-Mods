using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace Ungeziefi.Container_Utilities;

[HarmonyPatch]
public static class CustomItemSizes
{
    private static Dictionary<TechType, Vector2int> _cache;

    public static void RefreshCache()
    {
        _cache = Main.Config.SizeOverrides
            .GroupBy(x => x.TechType)
            .ToDictionary(g => g.Key, g => g.First().Size);
    }

    [HarmonyPatch(typeof(TechData), nameof(TechData.GetItemSize))]
    [HarmonyPostfix]
    public static void TechData_GetItemSize(TechType techType, ref Vector2int __result)
    {
        if (Main.Config.CustomItemSizes)
        {
            if (_cache == null) RefreshCache();

            if (_cache.TryGetValue(techType, out var customSize))
            {
                __result = customSize;
                return;
            }
        }

        if (Main.Config.AllItems1x1) __result = new Vector2int(1, 1);
    }
}