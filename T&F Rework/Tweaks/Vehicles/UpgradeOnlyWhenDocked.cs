using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class UpgradeOnlyWhenDocked
    {
        [HarmonyPatch(typeof(VehicleUpgradeConsoleInput), nameof(VehicleUpgradeConsoleInput.OnHandHover)), HarmonyPrefix]
        public static bool VehicleUpgradeConsoleInput_OnHandHover(SeamothStorageInput __instance)
        {
            if (!Main.Config.UpgradeOnlyWhenDocked) return true;

            var vehicle = __instance.GetComponentInParent<Vehicle>();
            if (vehicle != null && !vehicle.docked)
            {
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(VehicleUpgradeConsoleInput), nameof(VehicleUpgradeConsoleInput.OnHandClick)), HarmonyPrefix]
        public static bool VehicleUpgradeConsoleInput_OnHandClick(SeamothStorageInput __instance)
        {
            if (!Main.Config.UpgradeOnlyWhenDocked) return true;

            var vehicle = __instance.GetComponentInParent<Vehicle>();
            if (vehicle != null && !vehicle.docked)
            {
                return false;
            }

            return true;
        }
    }
}