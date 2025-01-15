using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoMVBClimbOnTop
    {
        [HarmonyPatch(typeof(Constructor), nameof(Constructor.Update)), HarmonyPostfix]
        public static void Constructor_Update(Constructor __instance)
        {
            if (Main.Config.NoMVBClimbOnTop && Player.main.transform.position.y > Ocean.GetOceanLevel())
            {
                __instance.climbTrigger.SetActive(false);
            }
        }
    }
}