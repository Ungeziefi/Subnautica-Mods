using HarmonyLib;

namespace Ungeziefi.Container_Utilities
{
    [HarmonyPatch]
    public class AllItems1x1
    {
        [HarmonyPatch(typeof(TechData), nameof(TechData.GetItemSize)), HarmonyPostfix]
        public static void TechData_GetItemSize(TechType techType, ref Vector2int __result)
        {
            // Only apply 1x1 if no custom size is set
            if (Main.Config.AllItems1x1 &&
                !CustomSlotSizes.HasCustomSizeFor(techType))
            {
                __result = new Vector2int(1, 1);
            }
        }
    }
}