using System;
using System.Reflection;
using Nautilus.Handlers;

namespace Ungeziefi.Fixes.Misc;

// Custom attribute to mark methods that should be invoked by ApplyAllFixes
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class MiscFixAttribute : Attribute
{
}

public class MiscFixes
{
    public static void ApplyAllFixes()
    {
        var methods = typeof(MiscFixes).GetMethods(BindingFlags.Public | BindingFlags.Static);
        foreach (var method in methods)
            if (method.GetCustomAttribute<MiscFixAttribute>() != null)
                method.Invoke(null, null);
    }

    [MiscFix]
    public static void CoffeeDrinkingSound()
    {
        if (Main.Config.CoffeeDrinkingSound) CraftDataHandler.SetEatingSound(TechType.Coffee, "event:/player/drink");
    }
}