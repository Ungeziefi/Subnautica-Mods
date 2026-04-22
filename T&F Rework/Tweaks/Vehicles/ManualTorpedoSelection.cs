using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UWE;

namespace Ungeziefi.Tweaks;

[HarmonyPatch]
public class ManualTorpedoSelection
{
    private static readonly Dictionary<string, TechType> selectedTorpedoTypes = new();

    private static float lastSelectionTime;
    private static readonly float selectionCooldown = 0.5f;

    // Get available torpedos
    private static List<TechType> GetAvailableTorpedoTypes(Vehicle vehicle)
    {
        List<TechType> result = new();
        List<ItemsContainer> containers = new();

        if (vehicle is SeaMoth seamoth)
        {
            for (var i = 0; i < seamoth.slotIDs.Length; i++)
                if (seamoth.modules.GetTechTypeInSlot(seamoth.slotIDs[i]) == TechType.SeamothTorpedoModule)
                {
                    var container = seamoth.GetStorageInSlot(i, TechType.SeamothTorpedoModule);
                    if (container != null)
                        containers.Add(container);
                }
        }
        else if (vehicle is Exosuit exosuit)
        {
            if (exosuit.leftArmType == TechType.ExosuitTorpedoArmModule)
            {
                var container = exosuit.GetStorageInSlot(0, TechType.ExosuitTorpedoArmModule);
                if (container != null)
                    containers.Add(container);
            }

            if (exosuit.rightArmType == TechType.ExosuitTorpedoArmModule)
            {
                var container = exosuit.GetStorageInSlot(1, TechType.ExosuitTorpedoArmModule);
                if (container != null)
                    containers.Add(container);
            }
        }

        // Check available torpedos in containers
        foreach (var torpedoType in vehicle.torpedoTypes)
        foreach (var container in containers)
            if (container.Contains(torpedoType.techType) && !result.Contains(torpedoType.techType))
            {
                result.Add(torpedoType.techType);
                break;
            }

        return result;
    }

    private static string GetVehicleId(Vehicle vehicle)
    {
        if (vehicle == null)
            return null;

        var prefabIdentifier = vehicle.GetComponent<PrefabIdentifier>();
        if (prefabIdentifier == null)
            return null;

        return prefabIdentifier.Id;
    }

    // Switch to next torpedo
    private static void CycleTorpedoSelection(Vehicle vehicle)
    {
        if (Time.time - lastSelectionTime < selectionCooldown)
            return;

        lastSelectionTime = Time.time;

        var vehicleId = GetVehicleId(vehicle);

        // Get available torpedoes
        var availableTorpedos = GetAvailableTorpedoTypes(vehicle);
        if (availableTorpedos.Count == 0)
        {
            selectedTorpedoTypes.Remove(vehicleId);
            ErrorMessage.AddMessage(Language.main.Get("VehicleTorpedoNoAmmo"));
            return;
        }

        // Get current selection or select first torpedo
        TechType nextTorpedo;
        if (selectedTorpedoTypes.TryGetValue(vehicleId, out var currentSelection))
        {
            var currentIndex = availableTorpedos.IndexOf(currentSelection);
            var nextIndex = (currentIndex + 1) % availableTorpedos.Count;
            nextTorpedo = availableTorpedos[nextIndex];
        }
        else
        {
            nextTorpedo = availableTorpedos[0];
        }

        selectedTorpedoTypes[vehicleId] = nextTorpedo;
        ErrorMessage.AddMessage($"{Language.main.Get(nextTorpedo)} selected");
    }

    // Override torpedo firing to use selected type
    [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.TorpedoShot))]
    [HarmonyPrefix]
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

        var vehicleId = GetVehicleId(__instance);

        // Use selected torpedo if available
        if (selectedTorpedoTypes.TryGetValue(vehicleId, out var selectedType) &&
            container.Contains(selectedType))
            foreach (var vehicleTorpedo in __instance.torpedoTypes)
                if (vehicleTorpedo != null && vehicleTorpedo.techType == selectedType)
                {
                    torpedoType = vehicleTorpedo;
                    break;
                }

        return true;
    }

    // Seamoth
    [HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.Update))]
    [HarmonyPostfix]
    public static void SeaMoth_Update(SeaMoth __instance)
    {
        if (!Main.Config.TCEnableFeature) return;

        var isPausedOrLoading = WaitScreen.IsWaiting
                                || Cursor.visible
                                || FreezeTime.HasFreezers()
                                || Player.main.GetPDA().isOpen;

        if (isPausedOrLoading ||
            !__instance.GetPilotingMode() ||
            __instance.modules.GetCount(TechType.SeamothTorpedoModule) == 0) return;

        if (GameInput.GetButtonDown(Main.SeamothCycleTorpedoButton))
            CycleTorpedoSelection(__instance);

        HandReticle.main.SetText(HandReticle.TextType.UseSubscript,
            $"Press {GameInput.FormatButton(Main.SeamothCycleTorpedoButton)} to change torpedo", false);
    }

    // Exosuit
    [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.Update))]
    [HarmonyPostfix]
    public static void Exosuit_Update(Exosuit __instance)
    {
        if (!Main.Config.TCEnableFeature) return;

        var isPausedOrLoading = WaitScreen.IsWaiting
                                || Cursor.visible
                                || FreezeTime.HasFreezers()
                                || Player.main.GetPDA().isOpen;

        if (isPausedOrLoading ||
            !__instance.GetPilotingMode() ||
            __instance.modules.GetCount(TechType.ExosuitTorpedoArmModule) == 0) return;

        if (GameInput.GetButtonDown(Main.PRAWNSuitCycleTorpedoButton))
            CycleTorpedoSelection(__instance);
    }

    [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.UpdateUIText))]
    [HarmonyPostfix]
    public static void Exosuit_UpdateUIText(Exosuit __instance)
    {
        if (!Main.Config.TCEnableFeature ||
            !__instance.GetPilotingMode() ||
            __instance.modules.GetCount(TechType.ExosuitTorpedoArmModule) == 0) return;

        HandReticle.main.SetText(HandReticle.TextType.UseSubscript,
            $"Press {GameInput.FormatButton(Main.PRAWNSuitCycleTorpedoButton)} to change torpedo", false);
    }
}