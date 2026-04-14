using HarmonyLib;

namespace Ungeziefi.Fixes.Misc
{
    [HarmonyPatch]
    public class DisableDeadTelemetry
    {
        [HarmonyPatch(typeof(Telemetry), nameof(Telemetry.IsAnalyzingSession)), HarmonyPrefix]
        public static bool Telemetry_IsAnalyzingSession()
        {
            if (Main.Config.DisableDeadTelemetry)
            {
                return false;
            }

            return true;
        }

    }
}