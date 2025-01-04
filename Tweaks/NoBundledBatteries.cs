using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch(typeof(EnergyMixin))]
    public class NoBundledBatteries
    {
        [HarmonyPatch(nameof(EnergyMixin.OnCraftEnd)), HarmonyPrefix]
        private static bool OnCraftEnd(ref EnergyMixin __instance)
        {
            GameModeUtils.GetGameMode(out GameModeOption mode, out GameModeOption cheats);
            if (Main.Config.NoBundledBatteries == Config.NoBundledBatteriesOption.Disabled || mode == GameModeOption.Creative)
            {
                return true;
            }

            GameObject obj = __instance.gameObject;
            bool shouldRemoveBattery = Main.Config.NoBundledBatteries == Config.NoBundledBatteriesOption.AllRecipes ||
                                       (Main.Config.NoBundledBatteries == Config.NoBundledBatteriesOption.VanillaRecipes &&
                                       // The only vanilla recipes of powered tools and vehicles that don't require batteries are these
                                        (obj.GetComponentInParent<SubRoot>()?.isCyclops == true ||
                                         obj.GetComponent<Exosuit>() != null ||
                                         obj.GetComponent<Welder>() != null));

            return !shouldRemoveBattery;
        }
    }
}