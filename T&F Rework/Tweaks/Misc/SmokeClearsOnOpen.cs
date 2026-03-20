using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class SmokeClearsOnOpen
    {
        [HarmonyPatch(typeof(EscapePodFirstUseCinematicsController), nameof(EscapePodFirstUseCinematicsController.OnTopHatchCinematicEnd)), HarmonyPostfix]
        private static void EscapePodFirstUseCinematicsController_OnTopHatchCinematicEnd(EscapePod __instance)
        {
            if (!Main.Config.SmokeClearsOnOpen) return;

            // From EscapePod-MainPrefab/EscapePod/models/Life_Pod_damaged_03
            // to
            // EscapePod-MainPrefab/EscapePod
            var escapePod = __instance.transform.parent.parent;

            // EscapePod/LifePodDamagedFX(Clone)
            var lifePodDamagedFX = escapePod.Find("LifePodDamagedFX(Clone)");
            if (lifePodDamagedFX != null)
            {
                for (int i = 0; i < lifePodDamagedFX.childCount; i++)
                {
                    var child = lifePodDamagedFX.GetChild(i);
                    if (child.name.ToLower().Contains("smoke"))
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }

            // EscapePod/ModulesRoot/RadioRoot/Radio(Clone)/xSparksElecLoop(Clone)/xSmoke
            var radio = escapePod.Find("ModulesRoot/RadioRoot/Radio(Clone)/xSparksElecLoop(Clone)/xSmoke");
            if (radio != null)
            {
                radio.gameObject.SetActive(false);
            }
        }
    }
}