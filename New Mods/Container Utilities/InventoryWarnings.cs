using HarmonyLib;
using System.Reflection;

namespace Ungeziefi.Container_Utilities
{
    [HarmonyPatch]
    public class InventoryWarnings
    {
        private static bool wasInventoryFull;
        private static int lastFreeSlots = -1;
        private static readonly FieldInfo itemsMapField = typeof(ItemsContainer).GetField("itemsMap", BindingFlags.NonPublic | BindingFlags.Instance);

        [HarmonyPatch(typeof(Inventory), nameof(Inventory.OnAddItem)), HarmonyPostfix]
        private static void Inventory_OnAddItem(Inventory __instance)
        {
            if (!Main.Config.ShowFreeSlotWarnings && !Main.Config.ShowFullInventoryWarning) return;

            if (__instance != Inventory.main) return;

            UpdateWarnings(__instance);
        }

        [HarmonyPatch(typeof(Inventory), nameof(Inventory.OnRemoveItem)), HarmonyPostfix]
        private static void Inventory_OnRemoveItem(Inventory __instance)
        {
            // Reset when removed
            if (__instance == Inventory.main)
            {
                wasInventoryFull = false;
                lastFreeSlots = -1;
            }
        }

        private static void UpdateWarnings(Inventory inventory)
        {
            var container = inventory.container;
            if (container == null) return;

            int freeSlots = CalculateFreeSlots(container);
            bool isFullNow = freeSlots == 0;

            if (Main.Config.ShowFullInventoryWarning && isFullNow && !wasInventoryFull)
            {
                ErrorMessage.AddMessage("Inventory is now full");

                //if (Main.Config.FullInventoryAudioCue)
                //    FMODUWE.PlayOneShot(
                //    Nautilus.Utility.AudioUtils.GetFmodAsset("lorem"), // Still have to decide a better sound (https://github.com/SubnauticaModding/Nautilus/blob/master/Nautilus/Documentation/resources/SN1-FMODEvents.txt)
                //    Player.main.transform.position);
            }

            // Show warning
            if (Main.Config.ShowFreeSlotWarnings &&
                freeSlots > 0 &&
                freeSlots <= Main.Config.FreeSlotWarningThreshold &&
                freeSlots != lastFreeSlots &&
                !WaitScreen.IsWaiting)
            {
                string slotText = freeSlots == 1 ? "slot" : "slots";
                ErrorMessage.AddMessage($"{freeSlots} inventory {slotText} remaining");
            }

            wasInventoryFull = isFullNow;
            lastFreeSlots = freeSlots;
        }

        private static int CalculateFreeSlots(ItemsContainer container)
        {
            container.Sort();

            if (itemsMapField.GetValue(container) is not InventoryItem[,] itemsMap)
            {
                return container.sizeX * container.sizeY - container.count;
            }

            int freeSlots = 0;

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
