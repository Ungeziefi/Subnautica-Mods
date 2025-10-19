using UnityEngine;

namespace Ungeziefi.Better_Scanner_Blips_Remake
{
    public static class ColorManagement
    {
        // Original colors
        public static Color originalBlipColor = new(1.00f, 0.64f, 0.00f, 1.00f);
        public static Color originalTextColor = new(1.00f, 0.68f, 0.00f, 1.00f);

        private static Color cachedBlipColor;
        private static Color cachedTextColor;
        private static (bool useCustomBlip, bool useCustomText, Color blipColor, Color textColor) lastSettings;
        public static bool colorsInitialized = false;

        public static void UpdateColorCache()
        {
            var config = Main.Config;
            lastSettings = (config.UseCustomBlipColor, config.UseCustomTextColor, config.BlipColor, config.TextColor);
            cachedBlipColor = config.GetBlipColor();
            cachedTextColor = config.GetTextColor();
            colorsInitialized = true;
        }

        public static bool ColorSettingsChanged()
        {
            if (!colorsInitialized) return true;

            var config = Main.Config;
            var currentSettings = (config.UseCustomBlipColor, config.UseCustomTextColor, config.BlipColor, config.TextColor);
            return !lastSettings.Equals(currentSettings);
        }

        public static Color GetCachedBlipColor() => cachedBlipColor;
        public static Color GetCachedTextColor() => cachedTextColor;
    }
}