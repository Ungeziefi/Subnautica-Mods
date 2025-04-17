using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class DestructibleDroopingStingers
    {
        [HarmonyPatch(typeof(HangingStinger), nameof(HangingStinger.Start)), HarmonyPostfix]
        public static void HangingStinger_Start(HangingStinger __instance)
        {
            if (!Main.Config.DestructibleDroopingStingers) return;

            LiveMixin liveMixin = __instance.GetComponent<LiveMixin>();
            if (liveMixin && liveMixin.data)
            {
                liveMixin.data.destroyOnDeath = true;
            }
        }
    }
}