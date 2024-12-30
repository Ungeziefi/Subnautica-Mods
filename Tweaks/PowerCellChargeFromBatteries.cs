using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    // Sets the charge level of newly crafted power cells based on the charge level of the batteries used for crafting
    public class TweakPowerCellChargeFromBatteries
    {
        // List of batteries used for crafting
        private static List<Battery> batteriesUsedForCrafting = new List<Battery>();

        [HarmonyPatch(typeof(CrafterLogic))]
        public class TweakPowerCellChargeFromBatteries_CrafterLogic
        {
            [HarmonyPatch(nameof(CrafterLogic.NotifyCraftEnd))]
            public static void Postfix(CrafterLogic __instance, GameObject target, TechType techType)
            {
                if (!Main.Config.PowerCellChargeFromBatteries)
                {
                    return;
                }

                // Get the Battery component from the crafted item
                Battery battery = target.GetComponent<Battery>();
                if (battery && batteriesUsedForCrafting.Count > 1)
                {
                    // Calculate the total charge of the used batteries
                    float totalCharge = 0f;
                    foreach (var b in batteriesUsedForCrafting)
                    {
                        totalCharge += b.charge;
                    }
                    // Calculate the average charge and set it to the new battery
                    float averageCharge = totalCharge / batteriesUsedForCrafting.Count;
                    float newCharge = Mathf.Clamp(averageCharge, 0, battery.capacity);
                    battery.charge = newCharge;
                }
                // Clear the list of used batteries after crafting
                batteriesUsedForCrafting.Clear();
            }
        }

        [HarmonyPatch(typeof(Inventory))]
        public class TweakPowerCellChargeFromBatteries_Inventory
        {
            // Clear the list of used batteries before crafting
            [HarmonyPatch(nameof(Inventory.ConsumeResourcesForRecipe))]
            public static void Prefix(Inventory __instance, TechType techType)
            {
                batteriesUsedForCrafting.Clear();
            }

            // Add removed batteries to the list
            [HarmonyPatch(nameof(Inventory.OnRemoveItem))]
            public static void Postfix(Inventory __instance, InventoryItem item)
            {
                // Get the Battery component from the removed item
                Battery battery = item.item.GetComponent<Battery>();
                if (battery)
                {
                    // Add the battery to the list of used batteries
                    batteriesUsedForCrafting.Add(battery);
                }
            }
        }
    }
}
