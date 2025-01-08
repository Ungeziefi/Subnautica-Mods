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
            var config = Main.Config.NoBundledBatteriesOption;

            if (config == NoBundledBatteriesOption.Disabled || mode == GameModeOption.Creative)
            {
                return true;
            }

            GameObject obj = __instance.gameObject;
            bool isVanillaRecipe = obj.GetComponentInParent<SubRoot>()?.isCyclops == true ||
                                   obj.GetComponent<Exosuit>() != null ||
                                   obj.GetComponent<Welder>() != null;

            bool shouldRemoveBattery = config == NoBundledBatteriesOption.AllRecipes ||
                                       (config == NoBundledBatteriesOption.VanillaRecipes && isVanillaRecipe);

            return !shouldRemoveBattery;
        }
    }
}