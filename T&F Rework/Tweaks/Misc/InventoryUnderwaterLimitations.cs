using HarmonyLib;

namespace Ungeziefi.Tweaks;

[HarmonyPatch]
public class InventoryUnderwaterLimitations
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.GetItemAction))]
    [HarmonyPostfix]
    public static void Inventory_GetItemAction(Inventory __instance, ref ItemAction __result, InventoryItem item)
    {
        // No eating underwater
        if (Main.Config.NoEatingUnderwater &&
            !Player.main.CanBreathe() &&
            __result == ItemAction.Eat)
            __result = ItemAction.None;

        // No medkits underwater
        if (Main.Config.NoMedkitsUnderwater &&
            !Player.main.CanBreathe() &&
            __result == ItemAction.Use && item.item.GetTechType() == TechType.FirstAidKit)
            __result = ItemAction.None;
    }
}