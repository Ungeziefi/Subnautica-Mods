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

            __instance.gameObject.EnsureComponent<PingInstance>();
        }
    }

    [HarmonyPatch(typeof(PingInstance))]
    internal class PingInstancePatcher
    {
        [HarmonyPatch(nameof(PingInstance.Initialize)), HarmonyPostfix]
        public static void Initialize(PingInstance __instance)
        {
            var constructor = __instance.GetComponent<Constructor>();
            if (constructor == null)
            {
                return;
            }

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