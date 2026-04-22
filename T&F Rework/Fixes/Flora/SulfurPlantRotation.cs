using HarmonyLib;

namespace Ungeziefi.Fixes.Flora;

[HarmonyPatch]
public class SulfurPlantRotation
{
    [HarmonyPatch(typeof(LargeWorldEntity), nameof(LargeWorldEntity.Awake))]
    [HarmonyPostfix]
    public static void LargeWorldEntity_Awake(LargeWorldEntity __instance)
    {
        if (!Main.Config.SulfurPlantRotation) return;

        var tt = CraftData.GetTechType(__instance.gameObject);
        if (tt == TechType.CrashHome || tt == TechType.CrashPowder)
        {
            var pos = __instance.transform.position;
            var x = (int)pos.x;
            var y = (int)pos.y;
            var z = (int)pos.z;

            if ((x == 280 && y == -40 && z == -195) ||
                (x == 272 && y == -41 && z == -199))
                __instance.transform.Rotate(90f, 0f, 0f);
        }
    }
}