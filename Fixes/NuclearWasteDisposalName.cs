using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(Trashcan))]
    public class NuclearWasteDisposalName
    {
        [HarmonyPatch(nameof(Trashcan.OnEnable)), HarmonyPrefix]
        public static void OnEnable(Trashcan __instance)
        {
            if (Main.Config.NuclearWasteDisposalName)
            {
                var usePrefix = Language.main.Get("Use") + " ";
                __instance.storageContainer.hoverText = usePrefix + (__instance.biohazard ? Language.main.Get("LabTrashcan") : Language.main.Get("Trashcan"));
            }
        }
    }
}