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
            if (Language.main.GetCurrentLanguage() == "English" && Main.MinorTweaksConfig.BladderfishTooltip)
            {
                LanguageHandler.SetTechTypeTooltip(TechType.Bladderfish, Language.main.Get("Tooltip_Bladderfish") + " Provides some oxygen when consumed raw.");
            }
        }
        public static void ApplyTrashcanNameConsistency()
        {
            if (Language.main.GetCurrentLanguage() == "English" && Main.MinorTweaksConfig.TrashcanNameConsistency)
            {
                // Override localization strings at runtime using Nautilus
                LanguageHandler.SetLanguageLine("Trashcan", "Trash can");
            }
        }
        public static void ApplyCapitalizeUseString()
        {
            if (Language.main.GetCurrentLanguage() == "English" && Main.MinorTweaksConfig.CapitalizeUseString)
            {
                LanguageHandler.SetLanguageLine("Use", "Use");
            }
        }
    }
}