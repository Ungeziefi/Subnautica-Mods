using System;
using System.Reflection;
using Nautilus.Handlers;

namespace Ungeziefi.Tweaks
{
    // Custom attribute to mark methods that should be invoked by ApplyAllTweaks
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class MiscTweakAttribute : Attribute
    {
    }

    public class MiscTweaks
    {
        // Apply all tweaks by invoking methods marked with the MiscTweak attribute
        public static void ApplyAllTweaks()
        {
            MethodInfo[] methods = typeof(MiscTweaks).GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (MethodInfo method in methods)
            {
                if (method.GetCustomAttribute<MiscTweakAttribute>() != null)
                {
                    method.Invoke(null, null);
                }
            }
        }

        [MiscTweak]
        public static void ApplyBladderfishTooltip()
        {
            if (Main.Config.BladderfishTooltip && Language.main.GetCurrentLanguage() == "English")
            {
                LanguageHandler.SetTechTypeTooltip(TechType.Bladderfish, Language.main.Get("Tooltip_Bladderfish") + " Provides some oxygen when consumed raw.");
            }
        }

        [MiscTweak]
        public static void ApplyTrashcanNameConsistency()
        {
            if (Main.Config.TrashcanNameConsistency && Language.main.GetCurrentLanguage() == "English")
            {
                LanguageHandler.SetLanguageLine("Trashcan", "Trash can");
            }
        }

        [MiscTweak]
        public static void ApplyCapitalizeUseString()
        {
            if (Main.Config.CapitalizeUseString && Language.main.GetCurrentLanguage() == "English")
            {
                LanguageHandler.SetLanguageLine("Use", "Use");
            }
        }
    }
}