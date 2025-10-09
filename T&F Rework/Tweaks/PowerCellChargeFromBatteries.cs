using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class PowerCellChargeFromBatteries
    {
        private static bool crafting = false;
        private static List<Battery> batteriesUsedForCrafting = new List<Battery>();

        [HarmonyPatch(typeof(CrafterLogic), nameof(CrafterLogic.NotifyCraftEnd)), HarmonyPostfix]
        public static void CrafterLogic_NotifyCraftEnd(CrafterLogic __instance, GameObject target, TechType techType)
        {
            if (!Main.Config.PowerCellChargeFromBatteries) return;

            Battery battery = target.GetComponent<Battery>();
            if (battery && batteriesUsedForCrafting.Count > 0)
            {
                // Total charge from all used batteries
                float totalCharge = 0f;
                foreach (var usedBattery in batteriesUsedForCrafting)
                {
                    totalCharge += usedBattery.charge;
                }

                // Set charge to total and clamp to capacity
                battery.charge = Mathf.Min(totalCharge, battery.capacity);
            }

            // Cleanup
            batteriesUsedForCrafting.Clear();
            crafting = false;
        }

        [HarmonyPatch(typeof(Inventory), nameof(Inventory.ConsumeResourcesForRecipe)), HarmonyPrefix]
        public static void Inventory_ConsumeResourcesForRecipe(Inventory __instance, TechType techType)
        {
            crafting = true;
            batteriesUsedForCrafting.Clear();  // Reset battery list
        }

        [HarmonyPatch(typeof(Inventory), nameof(Inventory.OnRemoveItem)), HarmonyPostfix]
        public static void Inventory_OnRemoveItem(Inventory __instance, InventoryItem item)
        {
            if (!crafting) return;

            Battery battery = item.item.GetComponent<Battery>();
            if (battery)
            {
                batteriesUsedForCrafting.Add(battery);  // Store removed batteries
            }
        }
    }
}