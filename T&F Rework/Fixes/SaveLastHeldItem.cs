using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class SaveLastHeldItem
    {
        private static bool isLoading = true;

        [HarmonyPatch(typeof(QuickSlots), nameof(QuickSlots.Select)), HarmonyPostfix]
        public static void QuickSlots_Select(QuickSlots __instance, int slotID)
        {
            if (!Main.Config.SaveLastHeldItem || isLoading) return;

            Main.SaveData.LastHeldItemSlot = slotID;
            Main.SaveData.Save();
        }

        public static void LoadedGameSetup()
        {
            if (!Main.Config.SaveLastHeldItem) return;

            if (Main.SaveData.LastHeldItemSlot >= 0 && Player.main.mode == Player.Mode.Normal)
            {
                Inventory.main.quickSlots.SelectImmediate(Main.SaveData.LastHeldItemSlot);
            }

            isLoading = false;
        }
    }
}