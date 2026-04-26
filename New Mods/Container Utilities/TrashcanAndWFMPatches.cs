using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Container_Utilities;

[HarmonyPatch]
public class TrashcanAndWFMPatches
{
    #region Trashcan

    private static bool IsTrashcan(StorageContainer instance)
    {
        return instance.gameObject.GetComponent<Trashcan>() != null;
    }

    [HarmonyPatch(typeof(StorageContainer), nameof(StorageContainer.Awake))]
    [HarmonyPrefix]
    private static void StorageContainer_Awake(StorageContainer __instance)
    {
        if (Main.Config.EnableCustomContainerSizes && IsTrashcan(__instance))
        {
            var width = Main.Config.TrashcanStorageWidth;
            var height = Main.Config.TrashcanStorageHeight;

            if (width > 0 && height > 0) __instance.Resize(width, height);
        }
    }

    [HarmonyPatch(typeof(Trashcan), nameof(Trashcan.OnEnable))]
    [HarmonyPostfix]
    public static void Trashcan_OnEnable(Trashcan __instance)
    {
        __instance.startDestroyTimeOut = Main.Config.TrashcanDestroyDelay;
        __instance.destroyInterval = Main.Config.TrashcanDestroyInterval;
    }

    #endregion

    #region Water Filtration Machine

    [HarmonyPatch(typeof(FiltrationMachine), nameof(FiltrationMachine.Start))]
    [HarmonyPostfix]
    private static void FiltrationMachine_Start(FiltrationMachine __instance)
    {
        if (!Main.Config.EnableCustomContainerSizes) return;

        var totalSlots = Main.Config.WFMStorageWidth * Main.Config.WFMStorageHeight;

        // Adjust max water/salt
        var requestedTotal = Main.Config.WFMMaxWaterBottles + Main.Config.WFMMaxSalt;
        if (requestedTotal > totalSlots)
        {
            var ratio = (float)totalSlots / requestedTotal;
            __instance.maxWater = Mathf.FloorToInt(Main.Config.WFMMaxWaterBottles * ratio);
            __instance.maxSalt = Mathf.FloorToInt(Main.Config.WFMMaxSalt * ratio);
        }
        else
        {
            __instance.maxWater = Main.Config.WFMMaxWaterBottles;
            __instance.maxSalt = Main.Config.WFMMaxSalt;
        }

        __instance.storageContainer.Resize(Main.Config.WFMStorageWidth, Main.Config.WFMStorageHeight);
    }

    #endregion
}