using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace Ungeziefi.Container_Utilities
{
    [HarmonyPatch]
    public class DimUnallowedItems
    {
        private static readonly Dictionary<ItemsContainer, Planter> planterContainers = new();
        private static readonly Dictionary<ItemsContainer, Aquarium> aquariumContainers = new();
        private static readonly ConditionalWeakTable<ItemsContainer, HashSet<TechType>> unallowedTechTable = new();

        // Track planters
        [HarmonyPatch(typeof(Planter), nameof(Planter.Start)), HarmonyPostfix]
        public static void Planter_Start(Planter __instance)
        {
            if (!Main.Config.DimUnallowedItems) return;

            planterContainers[__instance.storageContainer.container] = __instance;
        }

        // Track aquariums
        [HarmonyPatch(typeof(Aquarium), nameof(Aquarium.Start)), HarmonyPostfix]
        public static void Aquarium_Start(Aquarium __instance)
        {
            if (!Main.Config.DimUnallowedItems) return;

            aquariumContainers[__instance.storageContainer.container] = __instance;
        }

        [HarmonyPatch(typeof(uGUI_InventoryTab), nameof(uGUI_InventoryTab.OnOpenPDA)), HarmonyPostfix]
        public static void OnOpenPDA(uGUI_InventoryTab __instance)
        {
            if (!Main.Config.DimUnallowedItems) return;

            // Find open container
            IItemsContainer openContainer = null;
            for (int i = 0; i < Inventory.main.usedStorage.Count; i++)
            {
                openContainer = Inventory.main.GetUsedStorage(i);
                if (openContainer != null) break;
            }

            if (openContainer == null) return;

            // Equipment containers
            if (openContainer is Equipment equipment)
            {
                HandleEquipmentContainer(equipment, __instance);
                return;
            }

            if (openContainer is ItemsContainer container)
            {
                // Check if planter
                if (planterContainers.TryGetValue(container, out Planter planter))
                {
                    HandlePlanterContainer(planter, __instance);
                    return;
                }

                // Check if aquarium
                if (aquariumContainers.TryGetValue(container, out Aquarium aquarium))
                {
                    HandleAquariumContainer(aquarium, __instance);
                    return;
                }

                // Standard containers
                HandleStandardContainer(container, __instance);
            }
        }

        // Reset
        [HarmonyPatch(typeof(uGUI_InventoryTab), nameof(uGUI_InventoryTab.OnClosePDA)), HarmonyPostfix]
        public static void OnClosePDA(uGUI_InventoryTab __instance)
        {
            if (!Main.Config.DimUnallowedItems) return;

            __instance.inventory.items.Values.ForEach(icon => icon.SetChroma(1f));
        }

        private static void HandleEquipmentContainer(Equipment equipment, uGUI_InventoryTab inventoryTab)
        {
            foreach (var pair in inventoryTab.inventory.items)
            {
                InventoryItem invItem = pair.Key;
                uGUI_ItemIcon icon = pair.Value;
                TechType techType = invItem.item.GetTechType();
                EquipmentType itemType = TechData.GetEquipmentType(techType);

                if (!equipment.GetCompatibleSlot(itemType, out _))
                {
                    icon.SetChroma(0f);
                }
            }
        }

        private static void HandlePlanterContainer(Planter planter, uGUI_InventoryTab inventoryTab)
        {
            foreach (var pair in inventoryTab.inventory.items)
            {
                InventoryItem invItem = pair.Key;
                uGUI_ItemIcon icon = pair.Value;

                if (!planter.IsAllowedToAdd(invItem.item, false))
                {
                    icon.SetChroma(0f);
                }
            }
        }

        private static void HandleAquariumContainer(Aquarium aquarium, uGUI_InventoryTab inventoryTab)
        {
            foreach (var pair in inventoryTab.inventory.items)
            {
                InventoryItem invItem = pair.Key;
                uGUI_ItemIcon icon = pair.Value;

                if (!aquarium.IsAllowedToAdd(invItem.item, false))
                {
                    icon.SetChroma(0f);
                }
            }
        }

        private static void HandleStandardContainer(ItemsContainer container, uGUI_InventoryTab inventoryTab)
        {
            foreach (var pair in inventoryTab.inventory.items)
            {
                InventoryItem invItem = pair.Key;
                uGUI_ItemIcon icon = pair.Value;
                TechType techType = invItem.item.GetTechType();

                if (!container.IsTechTypeAllowed(techType))
                {
                    icon.SetChroma(0f);
                }
            }
        }

        #region Fix Allowed Items
        [HarmonyPatch(typeof(ItemsContainer), nameof(ItemsContainer.IsTechTypeAllowed)), HarmonyPostfix]
        public static void IsTechTypeAllowed_Postfix(ItemsContainer __instance, TechType techType, ref bool __result)
        {
            if (!Main.Config.DimUnallowedItems) return;

            // If whitelist already denied it, respect that
            if (!__result) return;

            // Add blacklist system
            if (unallowedTechTable.TryGetValue(__instance, out HashSet<TechType> unallowedTech) && unallowedTech.Count > 0)
            {
                __result = !unallowedTech.Contains(techType);
            }
        }

        [HarmonyPatch(typeof(Trashcan), nameof(Trashcan.OnEnable)), HarmonyPostfix]
        public static void Trashcan_OnEnable(Trashcan __instance)
        {
            if (!Main.Config.DimUnallowedItems) return;

            // Reactor rods whitelist
            if (__instance.biohazard)
            {
                if (__instance.storageContainer.container.allowedTech == null)
                {
                    __instance.storageContainer.container.allowedTech = new HashSet<TechType> { TechType.ReactorRod, TechType.DepletedReactorRod };
                }
            }

            // Reactor rods blacklist
            else
            {
                SetUnallowedTech(__instance.storageContainer.container, new HashSet<TechType> { TechType.ReactorRod, TechType.DepletedReactorRod });
            }
        }

        public static void SetUnallowedTech(ItemsContainer container, HashSet<TechType> unallowedTech)
        {
            unallowedTechTable.Remove(container);
            unallowedTechTable.Add(container, unallowedTech);
        }
        #endregion
    }
}