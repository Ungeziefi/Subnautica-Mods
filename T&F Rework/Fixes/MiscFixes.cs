using Nautilus.Handlers;
using System;
using System.Reflection;

namespace Ungeziefi.Fixes
{
    // Custom attribute to mark methods that should be invoked by ApplyAllFixes
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class MiscFixAttribute : Attribute
    {
    }

    public class MiscFixes
    {
        public static void ApplyAllFixes()
        {
            MethodInfo[] methods = typeof(MiscFixes).GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (MethodInfo method in methods)
            {
                if (method.GetCustomAttribute<MiscFixAttribute>() != null)
                {
                    method.Invoke(null, null);
                }
            }
        }

        [MiscFix]
        public static void CoffeeDrinkingSound()
        {
            if (Main.Config.CoffeeDrinkingSound)
            {
                CraftDataHandler.SetEatingSound(TechType.Coffee, "event:/player/drink");
            }
        }
    }
}