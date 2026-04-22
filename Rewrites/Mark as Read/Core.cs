using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace Ungeziefi.Mark_as_Read;

[HarmonyPatch]
public class MarkAsRead
{
    private static readonly Dictionary<int, (NotificationManager.Group tab, string label)> tabMap = new()
    {
        { 0, (NotificationManager.Group.Inventory, "Mark as viewed") },
        { 1, (NotificationManager.Group.Blueprints, "Mark as viewed") },
        { 3, (NotificationManager.Group.Gallery, "Mark as viewed") },
        { 4, (NotificationManager.Group.Log, "Mark as read") },
        { 5, (NotificationManager.Group.Encyclopedia, "Mark as read") }
    };

    [HarmonyPatch(typeof(uGUI_PDA), nameof(uGUI_PDA.OnToolbarClick))]
    [HarmonyPostfix]
    public static void uGUI_PDA_OnToolbarClick(int index, int button)
    {
        if (button != 1 || !tabMap.TryGetValue(index, out var tabData))
            return;

        var toRemove = NotificationManager.main.notifications
            .Keys
            .Where(n => n.group == tabData.tab)
            .ToList();

        foreach (var id in toRemove)
        {
            NotificationManager.main.notifications.Remove(id);
            NotificationManager.main.NotifyRemove(id);
        }
    }

    [HarmonyPatch(typeof(uGUI_PDA), nameof(uGUI_PDA.GetToolbarTooltip))]
    [HarmonyPostfix]
    private static void uGUI_PDA_GetToolbarTooltip(int index, TooltipData data)
    {
        if (!tabMap.TryGetValue(index, out var tabData))
            return;

        var count = NotificationManager.main.GetCount(tabData.tab);
        if (count > 0)
            data.prefix
                .Append("\n<size=20> </size>")
                .Append("<sprite name=\"MouseButtonRight\" color=#ADF8FFFF>")
                .Append($" - <color=#00FFFFFF>{tabData.label}</color>");
    }
}