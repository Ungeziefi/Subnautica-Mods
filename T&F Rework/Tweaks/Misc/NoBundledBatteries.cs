using HarmonyLib;

namespace Ungeziefi.Tweaks;

[HarmonyPatch]
public class NoBundledBatteries
{
    [HarmonyPatch(typeof(EnergyMixin), nameof(EnergyMixin.OnCraftEnd))]
    [HarmonyPrefix]
    private static bool EnergyMixin_OnCraftEnd(ref EnergyMixin __instance)
    {
        GameModeUtils.GetGameMode(out var mode, out _);
        var config = Main.Config.NoBundledBatteriesOption;

        if (config == NoBundledBatteriesOption.Disabled || mode == GameModeOption.Creative) return true;

        var obj = __instance.gameObject;
        var isVanillaRecipe =
            (obj.GetComponentInParent<SubRoot>() != null && obj.GetComponentInParent<SubRoot>().isCyclops) ||
            obj.GetComponent<Exosuit>() != null ||
            obj.GetComponent<Welder>() != null;

        var shouldRemoveBattery = config == NoBundledBatteriesOption.AllRecipes ||
                                  (config == NoBundledBatteriesOption.VanillaRecipes && isVanillaRecipe);

        return !shouldRemoveBattery;
    }
}