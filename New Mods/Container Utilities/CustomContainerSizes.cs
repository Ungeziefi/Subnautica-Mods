using HarmonyLib;
using System.Reflection;
using UnityEngine;
using Ungeziefi.Container_Utilities;

namespace CustomizedStorage.Patches
{
    [HarmonyPatch(typeof(StorageContainer), nameof(StorageContainer.Awake))]
    class StorageContainer_Awake_Patch
    {
        private static bool Prefix(StorageContainer __instance)
        {
            if (!Main.Config.EnableCustomContainerSizes)
                return true;

            var (width, height) = __instance switch
            {
                _ when IsSmallLocker(__instance) => (Main.Config.WallLockerWidth, Main.Config.WallLockerHeight),
                _ when IsLargeLocker(__instance) => (Main.Config.StandardLockerWidth, Main.Config.StandardLockerHeight),
                _ when IsEscapePodLocker(__instance) => (Main.Config.EscapePodLockerWidth, Main.Config.EscapePodLockerHeight),
                _ when IsCyclopsLocker(__instance) => (Main.Config.CyclopsLockerWidth, Main.Config.CyclopsLockerHeight),
                _ when IsWaterproofLocker(__instance) => (Main.Config.WallLockerWidth, Main.Config.WallLockerHeight),
                _ when IsTrashcan(__instance) => (Main.Config.TrashcanWidth, Main.Config.TrashcanHeight),
                _ => (0, 0)
            };

            if (width > 0 && height > 0)
                __instance.Resize(width, height);

            return true;
        }

        private static bool IsSmallLocker(StorageContainer instance) =>
            instance.gameObject.name.StartsWith("SmallLocker");

        private static bool IsLargeLocker(StorageContainer instance) =>
            instance.gameObject.name.StartsWith("Locker");

        private static bool IsEscapePodLocker(StorageContainer instance) =>
            instance.gameObject.GetComponent<SpawnEscapePodSupplies>() != null;

        private static bool IsCyclopsLocker(StorageContainer instance) =>
            instance.gameObject.name.StartsWith("submarine_locker_01_door");

        private static bool IsWaterproofLocker(StorageContainer instance) =>
            instance.gameObject.GetComponent<SmallStorage>() != null;

        private static bool IsTrashcan(StorageContainer instance) =>
            instance.gameObject.GetComponent<Trashcan>() != null;
    }

    [HarmonyPatch(typeof(SeamothStorageContainer), nameof(SeamothStorageContainer.Init))]
    class SeamothStorageContainer_Init_Patch
    {
        private static bool Prefix(SeamothStorageContainer __instance)
        {
            if (!Main.Config.EnableCustomContainerSizes)
                return true;

            __instance.width = Main.Config.SeamothStorageWidth;
            __instance.height = Main.Config.SeamothStorageHeight;
            return true;
        }
    }

    [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.UpdateStorageSize))]
    class Exosuit_UpdateStorageSize_Patch
    {
        private static void Postfix(Exosuit __instance)
        {
            if (!Main.Config.EnableCustomContainerSizes)
                return;

            __instance.storageContainer.Resize(
                Main.Config.ExosuitStorageWidth,
                Main.Config.ExosuitStorageHeight);
        }
    }

    [HarmonyPatch(typeof(BaseBioReactor), nameof(BaseBioReactor.container), MethodType.Getter)]
    class BaseBioReactor_get_container_Patch
    {
        private static readonly FieldInfo BaseBioReactor_container = typeof(BaseBioReactor)
            .GetField("_container", BindingFlags.NonPublic | BindingFlags.Instance);

        private static void Postfix(BaseBioReactor __instance)
        {
            if (!Main.Config.EnableCustomContainerSizes)
                return;

            if (BaseBioReactor_container.GetValue(__instance) is ItemsContainer container)
                container.Resize(Main.Config.BioreactorWidth, Main.Config.BioreactorHeight);
        }
    }

    [HarmonyPatch(typeof(FiltrationMachine), nameof(FiltrationMachine.Start))]
    class FiltrationMachine_Start_Patch
    {
        private static void Postfix(FiltrationMachine __instance)
        {
            if (!Main.Config.EnableCustomContainerSizes)
                return;

            // Calculate maximum allowed items based on container size
            int totalSlots = Main.Config.FiltrationWidth * Main.Config.FiltrationHeight;

            // Adjust max water/salt if they exceed container capacity
            int requestedTotal = Main.Config.FiltrationMaxWater + Main.Config.FiltrationMaxSalt;
            if (requestedTotal > totalSlots)
            {
                float ratio = (float)totalSlots / requestedTotal;
                __instance.maxWater = Mathf.FloorToInt(Main.Config.FiltrationMaxWater * ratio);
                __instance.maxSalt = Mathf.FloorToInt(Main.Config.FiltrationMaxSalt * ratio);
            }
            else
            {
                __instance.maxWater = Main.Config.FiltrationMaxWater;
                __instance.maxSalt = Main.Config.FiltrationMaxSalt;
            }

            __instance.storageContainer.Resize(Main.Config.FiltrationWidth, Main.Config.FiltrationHeight);
        }
    }

    [HarmonyPatch(typeof(Trashcan), nameof(Trashcan.OnEnable))]
    class Trashcan_OnEnable_Patch
    {
        private static void Postfix(Trashcan __instance)
        {
            if (!Main.Config.EnableCustomContainerSizes)
                return;

            __instance.startDestroyTimeOut = Main.Config.TrashcanDestroyDelay;
            __instance.destroyInterval = Main.Config.TrashcanDestroyInterval;
        }
    }

    [HarmonyPatch(typeof(uGUI_ItemsContainer), nameof(uGUI_ItemsContainer.OnResize))]
    class uGUI_ItemsContainer_OnResize_Patch
    {
        private static void Postfix(uGUI_ItemsContainer __instance, int width, int height)
        {
            float x = __instance.rectTransform.anchoredPosition.x;
            float y = height switch
            {
                9 => -39f,
                10 => -75f,
                _ => -4f
            };

            float sign = Mathf.Sign(x);
            float newX = sign * (width == 8 ? 292f : 284f);

            __instance.rectTransform.anchoredPosition = new Vector2(newX, y);
        }
    }
}