using HarmonyLib;
using Nautilus.Utility;
using UnityEngine;

namespace Ungeziefi.Container_Utilities;

[HarmonyPatch]
public class ContainerPrompts
{
    private static int GetUsedSlotsCount(ItemsContainer container)
    {
        var occupied = 0;
        foreach (var item in container) occupied += item.width * item.height;
        return occupied;
    }

    private static string GetFillColorHex(int used, int total)
    {
        var t = Mathf.Clamp01((float)used / total);
        var statusColor = Color.Lerp(Color.green, Color.red, t);
        return ColorUtility.ToHtmlStringRGB(statusColor);
    }

    [HarmonyPatch(typeof(StorageContainer), nameof(StorageContainer.OnHandHover))]
    [HarmonyPostfix]
    public static void StorageContainer_OnHandHover(StorageContainer __instance)
    {
        if (!Main.Config.ShowStorageInformation) return;

        var used = GetUsedSlotsCount(__instance.container);
        var total = ItemStorageHelper.GetTotalSlots(__instance.container);
        var colorHex = GetFillColorHex(used, total);

        var status = used >= total ? " - Full" : used == 0 ? " - Empty" : "";
        var countText = Main.Config.ColorCodedPrompts
            ? $"<color=#{colorHex}>{used}/{total}</color>"
            : $"{used}/{total}";

        HandReticle.main.SetText(HandReticle.TextType.HandSubscript, $"{countText}{status}", true);
    }
}