﻿using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class InventoryUnderwaterLimitations
    {
        [HarmonyPatch(typeof(Inventory), nameof(Inventory.GetItemAction)), HarmonyPostfix]
        public static void Inventory_GetItemAction(Inventory __instance, ref ItemAction __result, InventoryItem item)
        {
            var pickupable = item.item;
            var tt = pickupable.GetTechType();

            // No eating underwater
            if (Main.Config.NoEatingUnderwater &&
                Player.main.IsUnderwater() &&
                __result == ItemAction.Eat &&
                pickupable.gameObject.GetComponent<Eatable>())
            {
                __result = ItemAction.None;
                return;
            }

            // No medkits underwater
            if (tt == TechType.FirstAidKit && __result == ItemAction.Use)
            {
                if ((Main.Config.NoMedkitsUnderwater &&
                    Player.main.IsUnderwater()) ||
                    Player.main.GetComponent<LiveMixin>().maxHealth - Player.main.GetComponent<LiveMixin>().health < 0.01f)
                {
                    __result = ItemAction.None;
                }
            }
        }
    }
}