using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoUsedDataBoxLight
    {
        [HarmonyPatch(typeof(BlueprintHandTarget), nameof(BlueprintHandTarget.UnlockBlueprint)), HarmonyPostfix]
        public static void BlueprintHandTarget_UnlockBlueprint(BlueprintHandTarget __instance)
        {
            if (Main.Config.NoUsedDataBoxLight)
            {
                __instance.transform.Find("DataboxLightContainer")?.gameObject.SetActive(false);
            }
        }
    }
}