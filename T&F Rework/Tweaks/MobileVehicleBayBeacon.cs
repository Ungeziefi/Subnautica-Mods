using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class MobileVehicleBayBeacon
    {
        [HarmonyPatch(typeof(Constructor), nameof(Constructor.OnEnable)), HarmonyPostfix]
        public static void Constructor_OnEnable(Constructor __instance)
        {
            if (!Main.Config.MobileVehicleBayBeacon) return;

            __instance.gameObject.EnsureComponent<PingInstance>();
        }

        [HarmonyPatch(typeof(PingInstance), nameof(PingInstance.Initialize)), HarmonyPostfix]
        public static void PingInstance_Initialize(PingInstance __instance)
        {
            var constructor = __instance.GetComponent<Constructor>();
            if (constructor == null) return;

            if (!Main.Config.MobileVehicleBayBeacon)
            {
                UnityEngine.Object.Destroy(__instance);
                return;
            }

            __instance.pingType = PingType.Signal;
            __instance.origin = constructor.transform;
            __instance.SetLabel(Language.main.Get("Constructor"));
        }
    }
}