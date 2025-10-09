using Nautilus.Handlers;
using System;
using System.Reflection;

namespace Ungeziefi.Tweaks
{
    // Custom attribute to mark methods that should be invoked by ApplyAllTweaks
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class MiscTweakAttribute : Attribute
    {
    }

    public class MiscTweaks
    {
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
        public static void BladderfishTooltip()
        {
            if (Main.Config.BladderfishTooltip)
            {
                LanguageHandler.SetTechTypeTooltip(TechType.Bladderfish, Language.main.Get("Tooltip_Bladderfish") + " Provides some oxygen when consumed raw.");
            }
        }

        [MiscTweak]
        public static void TrashcanNameConsistency()
        {
            if (Main.Config.TrashcanNameConsistency)
            {
                LanguageHandler.SetLanguageLine("Trashcan", "Trash can");
            }
        }

        [MiscTweak]
        public static void CapitalizeUseString()
        {
            if (Main.Config.CapitalizeUseString)
            {
                LanguageHandler.SetLanguageLine("Use", "Use");
            }
        }
    }
}