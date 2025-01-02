using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(Constructor))]
    public class NoRedundantMobileVehicleBayClimbing
    {
        [HarmonyPatch(nameof(Constructor.Update)), HarmonyPostfix]
        public static void Update(Constructor __instance)
        {
            if (Main.Config.NoRedundantMobileVehicleBayClimbing && Player.main.transform.position.y > Ocean.GetOceanLevel())
            {
                __instance.climbTrigger.SetActive(false);
            }
        }
    }
}