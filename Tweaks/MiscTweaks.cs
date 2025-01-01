using System.Reflection;
using Nautilus.Handlers;

namespace Ungeziefi.Tweaks
{
    public class MiscTweaks
    {
        public static void ApplyAllTweaks()
        {
            MethodInfo[] methods = typeof(MiscTweaks).GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (MethodInfo method in methods)
            {
                if (method.Name.StartsWith("Apply") && method.Name != nameof(ApplyAllTweaks))
                {
                    method.Invoke(null, null);
                }
            }
        }

        public static void ApplyBladderfishTooltip()
        {
            if (Main.MinorTweaksConfig.BladderfishTooltip && Language.main.GetCurrentLanguage() == "English")
            {
                LanguageHandler.SetTechTypeTooltip(TechType.Bladderfish, Language.main.Get("Tooltip_Bladderfish") + " Provides some oxygen when consumed raw.");
            }
        }
        public static void ApplyTrashcanNameConsistency()
        {
            if (Main.MinorTweaksConfig.TrashcanNameConsistency && Language.main.GetCurrentLanguage() == "English")
            {
                // Override localization strings at runtime using Nautilus
                LanguageHandler.SetLanguageLine("Trashcan", "Trash can");
            }
        }
        public static void ApplyCapitalizeUseString()
        {
            if (Main.MinorTweaksConfig.CapitalizeUseString && Language.main.GetCurrentLanguage() == "English")
            {
                LanguageHandler.SetLanguageLine("Use", "Use");
            }
        }
    }
}