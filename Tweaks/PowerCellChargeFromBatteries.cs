using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    public class TweakPowerCellChargeFromBatteries
    {
        // Sets the charge level of newly crafted power cells based on the charge level of the batteries used for crafting
        [HarmonyPatch(typeof(CrafterLogic))]
        public class TweakPowerCellChargeFromBatteries_CrafterLogic
        {
            private static List<Battery> batteriesUsedForCrafting = new List<Battery>();

            [HarmonyPatch(nameof(CrafterLogic.NotifyCraftEnd))]
            public static void Postfix(CrafterLogic __instance, GameObject target, TechType techType)
            {
                if (!Main.Config.PowerCellChargeFromBatteries)
                {
                    return;
                }

                Battery battery = target.GetComponent<Battery>();
                if (battery && batteriesUsedForCrafting.Count > 1)
                {
                    float totalCharge = 0f;
                    foreach (var b in batteriesUsedForCrafting)
                    {
                        totalCharge += b.charge;
                    }
                    float averageCharge = totalCharge / batteriesUsedForCrafting.Count;
                    float newCharge = Mathf.Clamp(averageCharge, 0, battery.capacity);
                    battery.charge = newCharge;
                }
                batteriesUsedForCrafting.Clear();
            }

            [HarmonyPatch(typeof(Inventory))]
            public class TweakPowerCellChargeFromBatteries_Inventory
            {
                [HarmonyPatch(nameof(Inventory.ConsumeResourcesForRecipe))]
                public static void Prefix(Inventory __instance, TechType techType)
                {
                    batteriesUsedForCrafting.Clear();
                }

                [HarmonyPatch(nameof(Inventory.OnRemoveItem))]
                public static void Postfix(Inventory __instance, InventoryItem item)
                {
                    Battery battery = item.item.GetComponent<Battery>();
                    if (battery)
                    {
                        batteriesUsedForCrafting.Add(battery);
                    }
                }
            }
        }
    }
}
