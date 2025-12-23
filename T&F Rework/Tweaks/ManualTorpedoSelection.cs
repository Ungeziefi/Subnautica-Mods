using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class ManualTorpedoSelection
    {
        private static readonly Dictionary<string, TechType> selectedTorpedoTypes = new();

        // Selection cooldown
        private static float lastSelectionTime = 0f;
        private static readonly float selectionCooldown = 0.5f;

        // Get available torpedos
        private static List<TechType> GetAvailableTorpedoTypes(Vehicle vehicle)
        {
            List<TechType> result = new();
            List<ItemsContainer> containers = new();

            if (vehicle is SeaMoth seamoth)
            {
                for (int i = 0; i < seamoth.slotIDs.Length; i++)
                {
                    if (seamoth.modules.GetTechTypeInSlot(seamoth.slotIDs[i]) == TechType.SeamothTorpedoModule)
                    {
                        ItemsContainer container = seamoth.GetStorageInSlot(i, TechType.SeamothTorpedoModule);
                        if (container != null)
                            containers.Add(container);
                    }
                }
            }
            else if (vehicle is Exosuit exosuit)
            {
                if (exosuit.leftArmType == TechType.ExosuitTorpedoArmModule)
                {
                    ItemsContainer container = exosuit.GetStorageInSlot(0, TechType.ExosuitTorpedoArmModule);
                    if (container != null)
                        containers.Add(container);
                }

                if (exosuit.rightArmType == TechType.ExosuitTorpedoArmModule)
                {
                    ItemsContainer container = exosuit.GetStorageInSlot(1, TechType.ExosuitTorpedoArmModule);
                    if (container != null)
                        containers.Add(container);
                }
            }

            // Check available torpedos in containers
            foreach (var torpedoType in vehicle.torpedoTypes)
            {
                foreach (var container in containers)
                {
                    if (container.Contains(torpedoType.techType) && !result.Contains(torpedoType.techType))
                    {
                        result.Add(torpedoType.techType);
                        break;
                    }
                }
            }

            return result;
        }

        // Switch to next torpedo
        private static void CycleTorpedoSelection(Vehicle vehicle)
        {
            if (Time.time - lastSelectionTime < selectionCooldown)
                return;

            lastSelectionTime = Time.time;

            string vehicleId = vehicle.GetComponent<PrefabIdentifier>()?.Id ?? vehicle.name;

            // Get available torpedoes
            List<TechType> availableTorpedos = GetAvailableTorpedoTypes(vehicle);
            if (availableTorpedos.Count == 0)
            {
                selectedTorpedoTypes.Remove(vehicleId);
                ErrorMessage.AddMessage(Language.main.Get("VehicleTorpedoNoAmmo"));
                return;
            }

            // Get current selection or select first torpedo
            TechType nextTorpedo;
            if (selectedTorpedoTypes.TryGetValue(vehicleId, out TechType currentSelection))
            {
                int currentIndex = availableTorpedos.IndexOf(currentSelection);
                // If current selection is not found, or is invalid, default to first
                if (currentIndex == -1)
                    currentIndex = -1; // Will become 0 after increment

                int nextIndex = (currentIndex + 1) % availableTorpedos.Count;
                nextTorpedo = availableTorpedos[nextIndex];
            }
            else
            {
                nextTorpedo = availableTorpedos[0];
            }

            selectedTorpedoTypes[vehicleId] = nextTorpedo;
            ShowSelectedTorpedoMessage(nextTorpedo);
        }

        // Show torpedo selection message
        private static void ShowSelectedTorpedoMessage(TechType torpedoType)
        {
            ErrorMessage.AddMessage($"{Language.main.Get(torpedoType)} selected");
        }

        // Seamoth torpedo selection input
        [HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.Update)), HarmonyPostfix]
        public static void SeaMoth_Update(SeaMoth __instance)
        {
            if (!Main.Config.TCEnableFeature || !__instance.GetPilotingMode())
                return;

            if (GameInput.GetButtonDown(Main.SeamothCycleTorpedoButton))
                CycleTorpedoSelection(__instance);
        }

        // Exosuit torpedo selection input
        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.Update)), HarmonyPostfix]
        public static void Exosuit_Update(Exosuit __instance)
        {
            if (!Main.Config.TCEnableFeature || !__instance.GetPilotingMode())
                return;

            if (GameInput.GetButtonDown(Main.PRAWNSuitCycleTorpedoButton))
                CycleTorpedoSelection(__instance);
        }

        // Override torpedo firing to use selected type
        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.TorpedoShot)), HarmonyPrefix]
        public static bool Vehicle_TorpedoShot(Vehicle __instance, ItemsContainer container, ref TorpedoType torpedoType)
        {
            if (!Main.Config.TCEnableFeature || container == null)
                return true;

            // Get valid vehicle instance
            if (__instance == null)
#pragma warning disable Harmony003 // Harmony non-ref patch parameters modified
                __instance = Player.main.currentMountedVehicle;
#pragma warning restore Harmony003 // Harmony non-ref patch parameters modified
            if (__instance == null)
                return true;

            string vehicleId = __instance.GetComponent<PrefabIdentifier>()?.Id ?? __instance.name;

            // Use selected torpedo if available
            if (selectedTorpedoTypes.TryGetValue(vehicleId, out TechType selectedType) &&
                container.Contains(selectedType))
            {
                foreach (TorpedoType vehicleTorpedo in __instance.torpedoTypes)
                {
                    if (vehicleTorpedo != null && vehicleTorpedo.techType == selectedType)
                    {
                        torpedoType = vehicleTorpedo;
                        break;
                    }
                }
            }

            return true;
        }
    }
}