using HarmonyLib;

namespace Ungeziefi.Container_Utilities
{
    [HarmonyPatch]
    public class DimUnallowedItems
    {
        [HarmonyPatch(typeof(uGUI_InventoryTab), nameof(uGUI_InventoryTab.OnOpenPDA)), HarmonyPostfix]
        public static void OnOpenPDA(uGUI_InventoryTab __instance)
        {
            if (!Main.Config.DimUnallowedItems) return;
            if (__instance?.inventory?.items == null) return;

            IItemsContainer container = null;
            for (int i = 0; i < Inventory.main.usedStorage.Count; i++)
            {
                container = Inventory.main.GetUsedStorage(i);
                if (container != null) break;
            }

            if (container == null) return;

            foreach (var pair in __instance.inventory.items)
            {
                InventoryItem invItem = pair.Key;
                uGUI_ItemIcon icon = pair.Value;

                if (invItem?.item == null || icon == null) continue;

                Pickupable pickupable = invItem.item;
                TechType techType = invItem.item.GetTechType();
                EquipmentType itemType = TechData.GetEquipmentType(techType);

                if (!container.AllowedToAdd(pickupable, false) || (container is Equipment equipment && !equipment.GetCompatibleSlot(itemType, out _)))
                {
                    icon.SetChroma(0f);
                }
            }
        }

        // Reset
        [HarmonyPatch(typeof(uGUI_InventoryTab), nameof(uGUI_InventoryTab.OnClosePDA)), HarmonyPostfix]
        public static void OnClosePDA(uGUI_InventoryTab __instance)
        {
            if (!Main.Config.DimUnallowedItems) return;
            if (__instance?.inventory?.items == null) return;

            __instance.inventory.items.Values.ForEach(icon => icon.SetChroma(1f));
        }
    }
}