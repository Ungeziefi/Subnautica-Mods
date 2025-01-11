using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NuclearWasteDisposalName
    {
        [HarmonyPatch(typeof(Trashcan), nameof(Trashcan.OnEnable)), HarmonyPrefix]
        public static void Trashcan_OnEnable(Trashcan __instance)
        {
            if (Main.Config.NuclearWasteDisposalName)
            {
                var usePrefix = Language.main.Get("Use") + " ";
                __instance.storageContainer.hoverText = usePrefix + (__instance.biohazard ? Language.main.Get("LabTrashcan") : Language.main.Get("Trashcan"));
            }
        }
    }
}