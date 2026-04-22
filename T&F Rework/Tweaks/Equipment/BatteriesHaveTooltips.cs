using System.Text;
using HarmonyLib;

namespace Ungeziefi.Tweaks;

[HarmonyPatch]
public class BatteriesHaveTooltips
{
    [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.ItemCommons))]
    [HarmonyPostfix]
    private static void TooltipFactory_ItemCommons(StringBuilder sb, TechType techType)
    {
        if (!Main.Config.BatteriesHaveTooltips) return;

        var key = techType switch
        {
            TechType.Battery => "Tooltip_Battery",
            TechType.PowerCell => "Tooltip_PowerCell",
            TechType.PrecursorIonBattery => "Tooltip_PrecursorIonBattery",
            TechType.PrecursorIonPowerCell => "Tooltip_PrecursorIonPowerCell",
            _ => null
        };

        if (key != null) TooltipFactory.WriteDescription(sb, Language.main.Get(key));
    }
}