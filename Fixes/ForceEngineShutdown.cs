using System.Reflection;
using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(CyclopsEngineChangeState))]
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

        [HarmonyPatch(nameof(CyclopsEngineChangeState.Update)), HarmonyPostfix]
        public static void Update(CyclopsEngineChangeState __instance)
        {
            // Guard statement to check if the feature is enabled
            if (!Main.Config.ForceEngineShutdown)
            {
                return;
            }

            // Check if the power is off
            if (IsPowerOff(__instance))
            {
                // Shut down the engine if the power is off
                __instance.motorMode.engineOn = false;
                SetInvalidButton(__instance, true);
            }
            else
            {
                // Reset the invalidButton field if the power is on
                SetInvalidButton(__instance, false);
            }
        }
    }
}