using HarmonyLib;

namespace Ungeziefi.Fixes
{
    // Fix being able to climb the constructor when standing on it
    [HarmonyPatch(typeof(Constructor))]
    public class FixConstructorClimbing
    {
        [HarmonyPatch(nameof(Constructor.Update))]
        public static void Postfix(Constructor __instance)
        {
            if (Player.main.transform.position.y > Ocean.GetOceanLevel())
            {
                __instance.climbTrigger.SetActive(false);
            }
        }
    }
}