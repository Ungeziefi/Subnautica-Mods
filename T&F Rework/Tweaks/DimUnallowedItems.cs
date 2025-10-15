//using HarmonyLib;
//using System.Collections.Generic;

//namespace Ungeziefi.Tweaks
//{
//    [HarmonyPatch]
//    public class DimUnallowedItems
//    {
//        private static Dictionary<ItemsContainer, Planter> planters = new();
//        private static bool chargerOpen = false;
//        private static bool powerCellChargerOpen = false;

//        // Register planters
//        [HarmonyPatch(typeof(Planter), nameof(Planter.Start)), HarmonyPostfix]
//        public static void Planter_Start(Planter __instance)
//        {
//            if (!Main.Config.DimUnallowedItems) return;
//            planters[__instance.storageContainer.container] = __instance;
//        }

//        [HarmonyPatch(typeof(uGUI_InventoryTab), nameof(uGUI_InventoryTab.OnOpenPDA)), HarmonyPostfix]
//        public static void uGUI_InventoryTab_OnOpenPDA(uGUI_InventoryTab __instance)
//        {
//            if (!Main.Config.DimUnallowedItems) return;

//            // Get the current container player is interacting with
//            IItemsContainer itemsContainer = GetActiveContainer();
//            if (itemsContainer == null) return;

//            // Handle special case for Equipment
//            if (itemsContainer is Equipment equipment)
//            {
//                HandleEquipmentContainer(equipment, __instance);
//                return;
//            }

//            // Handle standard ItemsContainer
//            if (itemsContainer is ItemsContainer container)
//            {
//                // Special handling for planters
//                if (planters.ContainsKey(container))
//                {
//                    HandlePlanterContainer(planters[container], __instance);
//                    return;
//                }

//                // Standard container handling
//                HandleStandardContainer(container, __instance);
//            }
//        }

//        // Reset on close
//        [HarmonyPatch(typeof(uGUI_InventoryTab), nameof(uGUI_InventoryTab.OnClosePDA)), HarmonyPostfix]
//        public static void uGUI_InventoryTab_OnClosePDA(uGUI_InventoryTab __instance)
//        {
//            if (!Main.Config.DimUnallowedItems) return;

//            chargerOpen = false;
//            powerCellChargerOpen = false;
//            ResetItemAppearance(__instance);
//        }

//        #region Helpers
//        private static IItemsContainer GetActiveContainer()
//        {
//            for (int i = 0; i < Inventory.main.usedStorage.Count; i++)
//            {
//                IItemsContainer container = Inventory.main.GetUsedStorage(i);
//                if (container != null)
//                    return container;
//            }
//            return null;
//        }

//        private static void HandleEquipmentContainer(Equipment equipment, uGUI_InventoryTab inventoryTab)
//        {
//            // Check what kind of charger is open
//            chargerOpen = equipment.GetCompatibleSlot(EquipmentType.BatteryCharger, out string _);
//            powerCellChargerOpen = equipment.GetCompatibleSlot(EquipmentType.PowerCellCharger, out string _);

//            foreach (var pair in inventoryTab.inventory.items)
//            {
//                TechType techType = pair.Key.item.GetTechType();
//                EquipmentType itemType = CraftData.GetEquipmentType(techType);

//                // Dim incompatible items
//                if (!equipment.GetCompatibleSlot(itemType, out string _))
//                {
//                    if ((chargerOpen && !IsBattery(techType)) ||
//                        (powerCellChargerOpen && !IsPowerCell(techType)) ||
//                        (!chargerOpen && !powerCellChargerOpen))
//                    {
//                        pair.Value.SetChroma(0f);
//                    }
//                }
//            }
//        }

//        private static void HandlePlanterContainer(Planter planter, uGUI_InventoryTab inventoryTab)
//        {
//            foreach (var pair in inventoryTab.inventory.items)
//            {
//                if (!planter.IsAllowedToAdd(pair.Key.item, false))
//                    pair.Value.SetChroma(0f);
//            }
//        }

//        private static void HandleStandardContainer(ItemsContainer container, uGUI_InventoryTab inventoryTab)
//        {
//            foreach (var pair in inventoryTab.inventory.items)
//            {
//                TechType techType = pair.Key.item.GetTechType();
//                if (!container.IsTechTypeAllowed(techType))
//                    pair.Value.SetChroma(0f);
//            }
//        }

//        private static void ResetItemAppearance(uGUI_InventoryTab inventoryTab)
//        {
//            foreach (var pair in inventoryTab.inventory.items)
//                pair.Value.SetChroma(1f);
//        }

//        private static bool IsBattery(TechType techType)
//        {
//            switch (techType)
//            {
//                case TechType.Battery:
//                case TechType.LithiumIonBattery:
//                case TechType.PrecursorIonBattery:
//                    return true;
//                default:
//                    return false;
//            }
//        }

//        private static bool IsPowerCell(TechType techType)
//        {
//            switch (techType)
//            {
//                case TechType.PowerCell:
//                case TechType.PrecursorIonPowerCell:
//                    return true;
//                default:
//                    return false;
//            }
//        }
//        #endregion
//    }
//}