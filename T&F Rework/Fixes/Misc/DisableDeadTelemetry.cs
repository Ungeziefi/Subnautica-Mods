using HarmonyLib;

namespace Ungeziefi.Fixes.Misc;

[HarmonyPatch]
public class DisableDeadTelemetry
{
    [HarmonyPatch(typeof(Telemetry), nameof(Telemetry.Start))]
    [HarmonyPrefix]
    public static bool Telemetry_Start()
    {
        if (Main.Config.DisableDeadTelemetry) return false;

        return true;
    }
}