using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch(typeof(Constructor))]
    internal class MobileVehicleBayBeacon
    {
        [HarmonyPatch(nameof(Constructor.OnEnable)), HarmonyPostfix]
        public static void OnEnable(Constructor __instance)
        {
            if (!Main.Config.MobileVehicleBayBeacon)
            {
                return;
            }

            var PingInstance = __instance.gameObject.EnsureComponent<PingInstance>();
            PingInstance.pingType = PingType.Signal;
            PingInstance.origin = __instance.transform;
            PingInstance.SetLabel(Language.main.Get("Constructor"));
        }
    }
}