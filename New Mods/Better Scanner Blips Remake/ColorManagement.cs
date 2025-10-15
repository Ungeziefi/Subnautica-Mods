using UnityEngine;

namespace Ungeziefi.Better_Scanner_Blips_Remake
{
    public static class ColorManagement
    {
        // Original colors
        public static Color originalBlipColor = new(1.00f, 0.64f, 0.00f, 1.00f);
        public static Color originalTextColor = new(1.00f, 0.68f, 0.00f, 1.00f);

        // Cached colors
        private static Color cachedBlipColor;
        private static Color cachedTextColor;

        // Cached settings state to detect changes
        private static bool lastUseCustomBlipColor;
        private static bool lastUseCustomTextColor;
        private static Color lastBlipColor;
        private static Color lastTextColor;
        public static bool colorsInitialized = false;

        public static void UpdateColorCache()
        {
            Config config = Main.Config;

            // Update cached settings values
            lastUseCustomBlipColor = config.UseCustomBlipColor;
            lastUseCustomTextColor = config.UseCustomTextColor;
            lastBlipColor = config.BlipColor;
            lastTextColor = config.TextColor;

            // Update cached colors
            cachedBlipColor = config.GetBlipColor();
            cachedTextColor = config.GetTextColor();

            colorsInitialized = true;
        }

        public static bool ColorSettingsChanged()
        {
            if (!colorsInitialized) return true;

            Config config = Main.Config;

            return lastUseCustomBlipColor != config.UseCustomBlipColor ||
                   lastUseCustomTextColor != config.UseCustomTextColor ||
                   (config.UseCustomBlipColor && lastBlipColor != config.BlipColor) ||
                   (config.UseCustomTextColor && lastTextColor != config.TextColor);
        }

        public static Color GetCachedBlipColor()
        {
            return cachedBlipColor;
        }

        public static Color GetCachedTextColor()
        {
            return cachedTextColor;
        }
    }
}