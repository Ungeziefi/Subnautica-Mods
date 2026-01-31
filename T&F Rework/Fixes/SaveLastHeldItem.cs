using HarmonyLib;
using Nautilus.Handlers;
using System.Collections;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class SaveLastHeldItem
    {
        [HarmonyPatch(typeof(QuickSlots), nameof(QuickSlots.Select)), HarmonyPostfix]
        public static void QuickSlots_Select(QuickSlots __instance)
        {
            if (!Main.Config.SaveLastHeldItem ||
                WaitScreen.IsWaiting ||
                Player.main.mode != Player.Mode.Normal)
                return;

            Main.SaveData.LastHeldItemSlot = __instance.GetActiveSlotID();
            Main.SaveData.Save();
        }

        public static void RegisterLoadingTask()
        {
            if (!Main.Config.SaveLastHeldItem) return;

            WaitScreenHandler.RegisterLateAsyncLoadTask(
                Main.PLUGIN_NAME,
                RestoreLastHeldItem,
                "Restoring last held item"
            );
        }

        private static IEnumerator RestoreLastHeldItem(WaitScreenHandler.WaitScreenTask task)
        {
            if (Main.SaveData.LastHeldItemSlot >= 0 && Player.main.mode == Player.Mode.Normal)
            {
                Inventory.main.quickSlots.SelectImmediate(Main.SaveData.LastHeldItemSlot);
            }

            yield break;
        }
    }
}