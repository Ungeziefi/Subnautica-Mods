using HarmonyLib;
using Nautilus.Handlers;

namespace Ungeziefi.Fixes
{
    // From "Use Trashcan" to "Use Trash can"
    // From "Use Trashcan" to "Use Nuclear waste disposal"
    // Capitalized "Use"
    [HarmonyPatch(typeof(Trashcan))]
    public class FixTrashcanNames
    {
        [HarmonyPatch(nameof(Trashcan.OnEnable))]
        public static void Prefix(Trashcan __instance)
        {
            if (Language.main.GetCurrentLanguage() == "English")
            {
                // Override localization strings at runtime using Nautilus
                LanguageHandler.SetLanguageLine("Trashcan", "Trash can");
                LanguageHandler.SetLanguageLine("Use", "Use");
                var usePrefix = Language.main.Get("Use") + " ";
                __instance.storageContainer.hoverText = usePrefix + (__instance.biohazard ? Language.main.Get("LabTrashcan") : Language.main.Get("Trashcan"));
            }
        }
    }
}