using System.Collections.Generic;
using HarmonyLib;

namespace Ungeziefi.Container_Utilities;

[HarmonyPatch]
public class QuickTransfer
{
    #region Transfer All Items

    public static bool MoveAllItems(InventoryItem item)
    {
        return TransferItems(item, false);
    }

    #endregion

    #region Transfer Similar Items

    public static bool MoveSameItems(InventoryItem item)
    {
        return TransferItems(item, true);
    }

    #endregion

    #region Shared Code

    private static InventoryItem selectedItem;

    private static bool TransferItems(InventoryItem item, bool transferSimilarOnly)
    {
        if (item == null || item.container is not ItemsContainer container)
            return false;

        // Get target container
        var targetContainer = Inventory.main.GetOppositeContainer(item);
        if (targetContainer == null) return false;

        // Create transfer list
        List<InventoryItem> itemsToTransfer = new();

        // Populate transfer list based on mode
        if (transferSimilarOnly)
            container.GetItems(item.techType, itemsToTransfer); // Only same type items
        else
            foreach (var type in container.GetItemTypes())
                container.GetItems(type, itemsToTransfer); // All items

        // Attempt to transfer all items in list
        var anyTransferred = false;
        foreach (var inventoryItem in itemsToTransfer)
            if (Inventory.AddOrSwap(inventoryItem, targetContainer))
                anyTransferred = true;

        return anyTransferred;
    }

    // Track selected item
    [HarmonyPatch(typeof(uGUI_ItemsContainer), nameof(uGUI_ItemsContainer.SelectItem))]
    [HarmonyPostfix]
    public static void uGUI_ItemsContainer_SelectItem(uGUI_ItemsContainer __instance, object item)
    {
        var key = item as uGUI_ItemIcon;
        if (key == null || !__instance.icons.TryGetValue(key, out selectedItem))
            return;
    }

    #endregion

    #region Patches

    [HarmonyPatch(typeof(Inventory), "ExecuteItemAction", typeof(ItemAction), typeof(InventoryItem))]
    [HarmonyPrefix]
    public static bool Inventory_ExecuteItemAction(Inventory __instance, InventoryItem item, ItemAction action)
    {
        var oppositeContainer = __instance.GetOppositeContainer(item);
        if (action != ItemAction.Switch || oppositeContainer == null ||
            item.container is Equipment || oppositeContainer is Equipment)
            return true;

        if (Main.Config.EnableTransferAllItems && GameInput.GetButtonHeld(Main.TransferAllItemsButton))
            return !MoveAllItems(item);

        if (Main.Config.EnableTransferSimilarItems && GameInput.GetButtonHeld(Main.TransferAllSimilarItemsButton))
            return !MoveSameItems(item);

        return true;
    }

    [HarmonyPatch(typeof(GamepadInputModule), nameof(GamepadInputModule.OnUpdate))]
    [HarmonyPostfix]
    public static void GamepadInputModule_OnUpdate()
    {
        if (Main.Config.EnableTransferAllItems && GameInput.GetButtonHeld(Main.TransferAllSimilarItemsButton))
            MoveAllItems(selectedItem);
        else if (Main.Config.EnableTransferSimilarItems && GameInput.GetButtonHeld(Main.TransferAllSimilarItemsButton))
            MoveSameItems(selectedItem);
    }

    #endregion
}