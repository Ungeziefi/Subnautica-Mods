using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace Ungeziefi.Lose_Everything;

[HarmonyPatch]
public class LoseEverything
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.LoseItems))]
    public static bool Inventory_LoseItems(Inventory __instance, ref bool __result)
    {
        if (!Main.Config.EnableFeature) return true;

        var list = new List<InventoryItem>();
        foreach (var inventoryItem in __instance.container)
            if (inventoryItem.item.destroyOnDeath)
            {
                if (Main.Config.KeepToolsOnDeath &&
                    inventoryItem.item.gameObject.GetComponent<PlayerTool>() != null) continue;

                list.Add(inventoryItem);
            }

        foreach (var inventoryItem2 in (IItemsContainer)__instance.equipment)
            if (Main.Config.LoseEquipmentOnDeath || inventoryItem2.item.destroyOnDeath)
                list.Add(inventoryItem2);

        __result = false;

        foreach (var t in list.Where(t => __instance.InternalDropItem(t.item, false))) __result = true;

        return false;
    }
}