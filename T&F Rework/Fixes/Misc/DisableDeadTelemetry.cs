using HarmonyLib;

namespace Ungeziefi.Fixes.Misc
{
    [HarmonyPatch]
    public class DisableDeadTelemetry
    {
        [HarmonyPatch(typeof(Telemetry), nameof(Telemetry.SessionStart)), HarmonyPrefix]
        public static bool Telemetry_SessionStart()
        {
            if (Main.Config.DisableDeadTelemetry)
            {
                return false;
            }

            return true;
        }

    }
}