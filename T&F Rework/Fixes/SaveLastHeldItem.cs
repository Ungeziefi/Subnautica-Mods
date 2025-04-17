using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class SaveLastHeldItem
    {
        private static bool isLoadingGame = true;

        [HarmonyPatch(typeof(Player), nameof(Player.Start)), HarmonyPostfix]
        public static void Player_Start()
        {
            if (!Main.Config.SaveLastHeldItem) return;

            isLoadingGame = true;
            UWE.CoroutineHost.StartCoroutine(LoadSavedSlotWhenReady());
        }

        private static System.Collections.IEnumerator LoadSavedSlotWhenReady()
        {
            yield return new WaitUntil(() =>
                Player.main != null &&
                Inventory.main.quickSlots != null);

            // Wait for player mode to update
            yield return new WaitForSeconds(0.1f);

            if (Main.SaveData.LastHeldItemSlot >= 0 && Player.main.mode == Player.Mode.Normal)
            {
                QuickSlots quickSlots = Inventory.main.quickSlots;
                quickSlots.SelectImmediate(Main.SaveData.LastHeldItemSlot);
            }

            isLoadingGame = false;
        }

        [HarmonyPatch(typeof(QuickSlots), nameof(QuickSlots.Select)), HarmonyPostfix]
        public static void QuickSlots_Select(QuickSlots __instance)
        {
            if (!Main.Config.SaveLastHeldItem ||
                isLoadingGame ||
                Player.main.mode != Player.Mode.Normal)
                return;

            Main.SaveData.LastHeldItemSlot = __instance.GetActiveSlotID();
            Main.SaveData.Save();
        }
    }
}