using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Seamoth_Barrel_Roll
{
    [HarmonyPatch]
    public partial class SeamothBarrelRoll
    {
        [HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.OnPilotModeEnd)), HarmonyPostfix]
        public static void SeaMoth_OnPilotModeEnd(SeaMoth __instance)
        {
            if (!Main.Config.EnableFeature || !activeRolls.ContainsKey(__instance))
                return;

            // Reset engine sound
            __instance.engineSound?.AccelerateInput(1f);

            // Apply stabilization when empty if configured
            if (Main.Config.StabilizationMode == StabilizationMode.OnlyWhenEmpty &&
                (!Main.Config.StabilizationRequiresPower || HasPower(__instance)))
            {
                // Convert rotation to -180/180 range
                float zAngle = __instance.transform.eulerAngles.z;
                if (zAngle > 180f) zAngle -= 360f;

                if (Mathf.Abs(zAngle) > 2f)
                {
                    __instance.useRigidbody.AddTorque(-__instance.transform.forward * Mathf.Sign(zAngle),
                        ForceMode.VelocityChange);
                }
            }

            // Remove tracking state
            activeRolls.Remove(__instance);
        }
    }
}