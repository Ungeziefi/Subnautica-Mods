using HarmonyLib;
using System.Text;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class BatteriesHaveTooltips
    {
        [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.ItemCommons)), HarmonyPostfix]
        private static void TooltipFactory_ItemCommons(StringBuilder sb, TechType techType)
        {
            if (!Main.Config.BatteriesHaveTooltips) return;

            switch (techType)
            {
                case TechType.Battery:
                    TooltipFactory.WriteDescription(sb, Language.main.Get("Tooltip_Battery"));
                    break;
                case TechType.PowerCell:
                    TooltipFactory.WriteDescription(sb, Language.main.Get("Tooltip_PowerCell"));
                    break;
                case TechType.PrecursorIonBattery:
                    TooltipFactory.WriteDescription(sb, Language.main.Get("Tooltip_PrecursorIonBattery"));
                    break;
                case TechType.PrecursorIonPowerCell:
                    TooltipFactory.WriteDescription(sb, Language.main.Get("Tooltip_PrecursorIonPowerCell"));
                    break;
            }
        }
    }
}