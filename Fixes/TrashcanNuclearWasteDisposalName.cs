using HarmonyLib;

namespace Ungeziefi.Fixes
{
    // From "Use Trashcan" to "Use Nuclear waste disposal"
    [HarmonyPatch(typeof(Trashcan))]
    public class FixTrashcanNuclearWasteDisposalName
    {
        [HarmonyPatch(nameof(Trashcan.OnEnable)), HarmonyPrefix]
        public static void OnEnable(Trashcan __instance)
        {
            if (Main.FixesConfig.NuclearWasteDisposalName && Language.main.GetCurrentLanguage() == "English")
            {
                var usePrefix = Language.main.Get("Use") + " ";
                __instance.storageContainer.hoverText = usePrefix + (__instance.biohazard ? Language.main.Get("LabTrashcan") : Language.main.Get("Trashcan"));
            }
        }
    }
}