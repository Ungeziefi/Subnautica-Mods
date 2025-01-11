using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class SulfurPlantRotation
    {
        [HarmonyPatch(typeof(LargeWorldEntity), nameof(LargeWorldEntity.Awake)), HarmonyPostfix]
        public static void LargeWorldEntity_Awake(LargeWorldEntity __instance)
        {
            if (!Main.Config.SulfurPlantRotation)
            {
                return;
            }

            TechType tt = CraftData.GetTechType(__instance.gameObject);
            if (tt == TechType.CrashHome || tt == TechType.CrashPowder)
            {
                Vector3 pos = __instance.transform.position;
                int x = (int)pos.x;
                int y = (int)pos.y;
                int z = (int)pos.z;

                if ((x == 280 && y == -40 && z == -195) ||
                    (x == 272 && y == -41 && z == -199))
                {
                    __instance.transform.Rotate(90f, 0f, 0f);
                }
            }
        }
    }
}