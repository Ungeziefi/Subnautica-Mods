using Nautilus.Handlers;

namespace Ungeziefi.Tweaks
{
    public class TweakBladderfishTooltip
    {
        public static void ApplyBladderfishTooltip()
        {
            if (Language.main.GetCurrentLanguage() == "English" && Main.Config.BladderfishTooltip)
            {
                LanguageHandler.SetTechTypeTooltip(TechType.Bladderfish, Language.main.Get("Tooltip_Bladderfish") + " Provides some oxygen when consumed raw.");
            }
        }
    }
}