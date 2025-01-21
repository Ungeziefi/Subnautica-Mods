using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class ForceEngineShutdown
    {
        [HarmonyPatch(typeof(CyclopsHelmHUDManager), nameof(CyclopsHelmHUDManager.Update)), HarmonyPrefix]
        public static void CyclopsHelmHUDManager_Update(CyclopsHelmHUDManager __instance)
        {
            if (!Main.Config.ForceEngineShutdown) return;

            bool isPowered = __instance.GetComponentInParent<PowerRelay>()?.IsPowered() ?? false;

            // Force engine off when unpowered
            // The UI doesn't update though, seems like a vanilla bug also when the sub awakes with the engine off
            var motorMode = __instance.GetComponentInParent<CyclopsMotorMode>();
            if (!isPowered && motorMode != null && motorMode.engineOn)
            {
                motorMode.engineOn = false;
            }
        }
    }
}