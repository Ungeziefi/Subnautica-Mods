using System.Reflection;
using HarmonyLib;

namespace Ungeziefi.Container_Utilities
{
    [HarmonyPatch]
    internal class InventoryWarnings
    {
        private static bool wasInventoryFull;
        private static int lastFreeSlots = -1;
        private static readonly FieldInfo itemsMapField = typeof(ItemsContainer).GetField("itemsMap", BindingFlags.NonPublic | BindingFlags.Instance);

        [HarmonyPatch(typeof(Inventory), nameof(Inventory.OnAddItem)), HarmonyPostfix]
        private static void Inventory_OnAddItem(Inventory __instance)
        {
            if (!Main.Config.ShowFreeSlotWarnings && !Main.Config.ShowFullInventoryWarning) return;

            // Only track player inventory
            if (__instance != Inventory.main) return;

            UpdateWarnings(__instance);
        }

        [HarmonyPatch(typeof(Inventory), nameof(Inventory.OnRemoveItem)), HarmonyPostfix]
        private static void Inventory_OnRemoveItem(Inventory __instance)
        {
            // Reset tracking states when an item is removed from player inventory
            if (__instance == Inventory.main)
            {
                wasInventoryFull = false;
                lastFreeSlots = -1;
            }
        }

        private static void UpdateWarnings(Inventory inventory)
        {
            var container = inventory.container;
            if (container == null)
                return;

            // Calculate free inventory slots accurately using reflection
            int freeSlots = CalculateFreeSlots(container);

            // Check if inventory just became full
            bool isFullNow = freeSlots == 0;

            // Show full inventory warning when appropriate
            if (Main.Config.ShowFullInventoryWarning && isFullNow && !wasInventoryFull)
            {
                ErrorMessage.AddMessage("Inventory is now full");
            }

            // Show free slots warning if below threshold and the count has changed
            if (Main.Config.ShowFreeSlotWarnings &&
                freeSlots > 0 &&
                freeSlots <= Main.Config.FreeSlotWarningThreshold &&
                freeSlots != lastFreeSlots)
            {
                string slotText = freeSlots == 1 ? "slot" : "slots";
                ErrorMessage.AddMessage($"{freeSlots} inventory {slotText} remaining");
            }

            // Update tracking variables
            wasInventoryFull = isFullNow;
            lastFreeSlots = freeSlots;
        }

        private static int CalculateFreeSlots(ItemsContainer container)
        {
            // Force inventory sorting to ensure accurate mapping
            container.Sort();

            // Access the internal itemsMap via reflection
            if (itemsMapField.GetValue(container) is not InventoryItem[,] itemsMap)
            {
                // Fallback calculation if reflection fails
                return container.sizeX * container.sizeY - container.count;
            }

            int freeSlots = 0;

            // Count free cells by examining the item map directly
            for (int y = 0; y < container.sizeY; y++)
            {
                for (int x = 0; x < container.sizeX; x++)
                {
                    if (itemsMap[x, y] == null)
                    {
                        freeSlots++;
                    }
                }
            }

            return freeSlots;
        }
    }
}