using System.Reflection;
using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class ForceEngineShutdown
    {
        private static bool IsPowerOff(CyclopsEngineChangeState instance)
        {
            var powerRelay = instance.GetComponentInParent<PowerRelay>();
            return powerRelay != null && powerRelay.GetPowerStatus() == PowerSystem.Status.Offline;
        }

        private static void SetInvalidButton(CyclopsEngineChangeState instance, bool value)
        {
            FieldInfo invalidButtonField = typeof(CyclopsEngineChangeState).GetField("invalidButton", BindingFlags.NonPublic | BindingFlags.Instance);
            if (invalidButtonField != null)
            {
                invalidButtonField.SetValue(instance, value);
            }
        }

        [HarmonyPatch(typeof(CyclopsEngineChangeState), nameof(CyclopsEngineChangeState.Update)), HarmonyPostfix]
        public static void CyclopsEngineChangeState_Update(CyclopsEngineChangeState __instance)
        {
            if (!Main.Config.ForceEngineShutdown)
            {
                return;
            }

            // Shut down engine if power is off
            if (IsPowerOff(__instance))
            {
                __instance.motorMode.engineOn = false;
                SetInvalidButton(__instance, true);
            }
            // Valid button if power is on
            else
            {
                SetInvalidButton(__instance, false);
            }
        }
    }
}