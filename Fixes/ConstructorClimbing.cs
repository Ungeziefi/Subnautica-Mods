using HarmonyLib;

namespace Ungeziefi.Fixes
{
    // Fix being able to climb the constructor when standing on it
    [HarmonyPatch(typeof(Constructor))]
    public class FixConstructorClimbing
    {
        [HarmonyPatch(nameof(Constructor.Update)), HarmonyPostfix]
        public static void Update(Constructor __instance)
        {
            if (Main.FixesConfig.NoRedundantMobileVehicleBayClimbing && Player.main.transform.position.y > Ocean.GetOceanLevel())
            {
                __instance.climbTrigger.SetActive(false);
            }
        }
    }
}