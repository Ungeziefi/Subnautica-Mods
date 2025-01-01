using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(SeaMoth))]
    public class FixSeaMothStorageMeshPosition
    {
        // Allow toggling the fix off without restarting the game
        private static readonly Vector3 originalLeftPosition = new Vector3(0f, 0f, 0f);
        private static readonly Vector3 originalRightPosition = new Vector3(0f, 0f, 0f);
        private static readonly Vector3 originalLeftEulerAngles = new Vector3(0f, 0f, 0f);
        private static readonly Vector3 originalRightEulerAngles = new Vector3(0f, 0f, 0f);

        private static void ResetStoragePosition(SeaMoth seaMoth, int slotID)
        {
            string storagePath = slotID == 0 ? "Model/Submersible_SeaMoth_extras/Submersible_seaMoth_geo/seaMoth_storage_01_L_geo" :
                                               "Model/Submersible_SeaMoth_extras/Submersible_seaMoth_geo/seaMoth_storage_01_R_geo";

            Transform storage = seaMoth.transform.Find(storagePath);
            if (storage)
            {
                Vector3 position = slotID == 0 ? originalLeftPosition : originalRightPosition;
                Vector3 eulerAngles = slotID == 0 ? originalLeftEulerAngles : originalRightEulerAngles;

                storage.localPosition = position;
                storage.localEulerAngles = eulerAngles;
            }
        }

        [HarmonyPatch(nameof(SeaMoth.OnUpgradeModuleChange)), HarmonyPostfix]
        static void OnUpgradeModuleChange(SeaMoth __instance, int slotID, TechType techType, bool added)
        {
            if (!Main.FixesConfig.SeamothStorageModulesGap)
            {
                ResetStoragePosition(__instance, slotID);
                return;
            }

            if (!added || techType != TechType.VehicleStorageModule)
            {
                return;
            }

            // 0 is the left module, 1 is the right module
            string storagePath = slotID == 0 ? "Model/Submersible_SeaMoth_extras/Submersible_seaMoth_geo/seaMoth_storage_01_L_geo" :
                                               "Model/Submersible_SeaMoth_extras/Submersible_seaMoth_geo/seaMoth_storage_01_R_geo";

            Transform storage = __instance.transform.Find(storagePath);
            if (storage)
            {
                Vector3 position = slotID == 0 ? new Vector3(0.01f, 0f, 0f) : new Vector3(-0.01f, 0f, 0f);
                Vector3 eulerAngles = slotID == 0 ? new Vector3(0f, 0f, -0.6f) : new Vector3(0f, 0f, 0.6f);

                storage.localPosition = position;
                storage.localEulerAngles = eulerAngles;
            }
        }
    }
}