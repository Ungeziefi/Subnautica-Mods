using HarmonyLib;

namespace Ungeziefi.Container_Utilities;

[HarmonyPatch]
internal class CustomContainerSizes
{
    private static bool IsSmallLocker(StorageContainer instance)
    {
        return instance.gameObject.name.StartsWith("SmallLocker");
    }

    private static bool IsLargeLocker(StorageContainer instance)
    {
        return instance.gameObject.name.StartsWith("Locker");
    }

    private static bool IsEscapePodLocker(StorageContainer instance)
    {
        return instance.gameObject.GetComponent<SpawnEscapePodSupplies>() != null;
    }

    private static bool IsCyclopsLocker(StorageContainer instance)
    {
        return instance.gameObject.name.StartsWith("submarine_locker_01_door");
    }

    private static bool IsWaterproofLocker(StorageContainer instance)
    {
        return instance.gameObject.GetComponent<SmallStorage>() != null;
    }
    
    [HarmonyPatch(typeof(StorageContainer), nameof(StorageContainer.Awake))]
    [HarmonyPrefix]
    private static void StorageContainer_Awake(StorageContainer __instance)
    {
        if (!Main.Config.EnableCustomContainerSizes) return;

        var (width, height) = __instance switch
        {
            _ when IsSmallLocker(__instance) => (Main.Config.WallLockerWidth, Main.Config.WallLockerHeight),
            _ when IsLargeLocker(__instance) => (Main.Config.StandardLockerWidth, Main.Config.StandardLockerHeight),
            _ when IsEscapePodLocker(__instance) => (Main.Config.EscapePodLockerWidth,
                Main.Config.EscapePodLockerHeight),
            _ when IsCyclopsLocker(__instance) => (Main.Config.CyclopsLockerWidth, Main.Config.CyclopsLockerHeight),
            _ when IsWaterproofLocker(__instance) => (Main.Config.WaterproofLockerWidth,
                Main.Config.WaterproofLockerHeight),
            _ => (0, 0)
        };

        if (width > 0 && height > 0)
            __instance.Resize(width, height);
    }

    [HarmonyPatch(typeof(SeamothStorageContainer), nameof(SeamothStorageContainer.Init))]
    [HarmonyPostfix]
    public static void SeamothStorageContainer_Init(SeamothStorageContainer __instance)
    {
        if (!Main.Config.EnableCustomContainerSizes) return;

        __instance.container.Resize(Main.Config.SeamothStorageWidth, Main.Config.SeamothStorageHeight);
    }
    
    [HarmonyPatch(typeof(Exosuit), nameof(Exosuit.UpdateStorageSize))]
    [HarmonyPrefix]
    private static bool Exosuit_UpdateStorageSize(Exosuit __instance)
    {
        if (!Main.Config.EnableCustomContainerSizes) return true;

        int num = Main.Config.ExosuitStorageHeight + __instance.modules.GetCount(TechType.VehicleStorageModule);
        __instance.storageContainer.Resize(Main.Config.ExosuitStorageWidth, num);

        return false;
    }

    [HarmonyPatch(typeof(BaseBioReactor), nameof(BaseBioReactor.Start))]
    [HarmonyPostfix]
    private static void BaseBioReactor_Start(BaseBioReactor __instance)
    {
        if (!Main.Config.EnableCustomContainerSizes) return;

        __instance.container.Resize(Main.Config.BioreactorStorageWidth, Main.Config.BioreactorStorageHeight);
    }
}