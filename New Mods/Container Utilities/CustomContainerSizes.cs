using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Container_Utilities
{
    [HarmonyPatch]
    class CustomContainerSizes
    {
        [HarmonyPatch(typeof(StorageContainer), nameof(StorageContainer.Awake)), HarmonyPrefix]
        private static void StorageContainer_Awake(StorageContainer __instance)
        {
            if (!Main.Config.EnableCustomContainerSizes) return;

            var (width, height) = __instance switch
            {
                _ when IsSmallLocker(__instance) => (Main.Config.WallLockerWidth, Main.Config.WallLockerHeight),
                _ when IsLargeLocker(__instance) => (Main.Config.StandardLockerWidth, Main.Config.StandardLockerHeight),
                _ when IsEscapePodLocker(__instance) => (Main.Config.EscapePodLockerWidth, Main.Config.EscapePodLockerHeight),
                _ when IsCyclopsLocker(__instance) => (Main.Config.CyclopsLockerWidth, Main.Config.CyclopsLockerHeight),
                _ when IsWaterproofLocker(__instance) => (Main.Config.WaterproofLockerWidth, Main.Config.WaterproofLockerHeight),
                _ when IsTrashcan(__instance) => (Main.Config.TrashcanWidth, Main.Config.TrashcanHeight),
                _ => (0, 0)
            };

            if (width > 0 && height > 0)
                __instance.Resize(width, height);
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


        [HarmonyPatch(typeof(SeamothStorageContainer), nameof(SeamothStorageContainer.Init)), HarmonyPostfix]
        public static void SeamothStorageContainer_Init(SeamothStorageContainer __instance)
        {
            if (!Main.Config.EnableCustomContainerSizes) return;

            __instance.width = Main.Config.SeamothStorageWidth;
            __instance.height = Main.Config.SeamothStorageHeight;
        }

        [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.UpdateStorageSize)), HarmonyPostfix]
        private static void Exosuit_UpdateStorageSize(Exosuit __instance)
        {
            if (!Main.Config.EnableCustomContainerSizes) return;

            __instance.storageContainer.Resize(
                Main.Config.ExosuitStorageWidth,
                Main.Config.ExosuitStorageHeight);
        }

        [HarmonyPatch(typeof(BaseBioReactor), nameof(BaseBioReactor.Start)), HarmonyPostfix]
        private static void BaseBioReactor_Start(BaseBioReactor __instance)
        {
            if (!Main.Config.EnableCustomContainerSizes) return;

            __instance.container.Resize(Main.Config.FiltrationWidth, Main.Config.FiltrationHeight);
        }

        [HarmonyPatch(typeof(FiltrationMachine), nameof(FiltrationMachine.Start)), HarmonyPostfix]
        private static void FiltrationMachine_Start(FiltrationMachine __instance)
        {
            if (!Main.Config.EnableCustomContainerSizes) return;

            int totalSlots = Main.Config.FiltrationWidth * Main.Config.FiltrationHeight;

            // Adjust max water/salt
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

        [HarmonyPatch(typeof(Trashcan), nameof(Trashcan.OnEnable)), HarmonyPostfix]
        private static void Trashcan_OnEnable(Trashcan __instance)
        {
            if (!Main.Config.EnableCustomContainerSizes) return;

            __instance.startDestroyTimeOut = Main.Config.TrashcanDestroyDelay;
            __instance.destroyInterval = Main.Config.TrashcanDestroyInterval;
        }
    }
}