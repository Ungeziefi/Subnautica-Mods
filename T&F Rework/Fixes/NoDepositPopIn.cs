using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoDepositPopIn
    {
        [HarmonyPatch(typeof(LargeWorldEntity), nameof(LargeWorldEntity.Awake)), HarmonyPostfix]
        private static void LargeWorldEntity_Awake(LargeWorldEntity __instance)
        {
            if (Main.Config.NoDepositPopIn == "Disabled") return;

            var drillable = __instance.GetComponent<Drillable>();
            var resource = __instance.GetComponent<ResourceTracker>(); // Check ResourceTracker to avoid breaking floating stones (they'd sink otherwise)
            if (drillable && resource)
            {
                __instance.cellLevel = Main.Config.NoDepositPopIn switch
                {
                    "Medium" => LargeWorldEntity.CellLevel.Medium,
                    "Far" => LargeWorldEntity.CellLevel.Far,
                    "VeryFar" => LargeWorldEntity.CellLevel.VeryFar,
                    _ => LargeWorldEntity.CellLevel.Near // Default
                };
            }
        }
    }
}