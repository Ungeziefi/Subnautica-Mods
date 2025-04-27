using UnityEngine;

namespace Ungeziefi.Better_Scanner_Blips_Remake
{
    public static class ColorManagement
    {
        // Original colors
        public static Color originalBlipColor = new Color(1.00f, 0.64f, 0.00f, 1.00f);
        public static Color originalTextColor = new Color(1.00f, 0.68f, 0.00f, 1.00f);

        // Cached colors
        private static Color cachedBlipColor;
        private static Color cachedTextColor;

        // Cached settings state to detect changes
        private static bool lastUseCustomBlipColor;
        private static bool lastUseCustomTextColor;
        private static float lastBlipRed, lastBlipGreen, lastBlipBlue;
        private static float lastTextRed, lastTextGreen, lastTextBlue;
        public static bool colorsInitialized = false;

        public static void UpdateColorCache()
        {
            Config config = Main.Config;

            // Update cached settings values
            lastUseCustomBlipColor = config.UseCustomBlipColor;
            lastUseCustomTextColor = config.UseCustomTextColor;
            lastBlipRed = config.BlipColorRed;
            lastBlipGreen = config.BlipColorGreen;
            lastBlipBlue = config.BlipColorBlue;
            lastTextRed = config.TextColorRed;
            lastTextGreen = config.TextColorGreen;
            lastTextBlue = config.TextColorBlue;

            // Update cached colors
            cachedBlipColor = config.UseCustomBlipColor ? config.GetBlipColor() : originalBlipColor;
            cachedTextColor = config.UseCustomTextColor ? config.GetTextColor() : originalTextColor;

            colorsInitialized = true;
        }

        public static bool ColorSettingsChanged()
        {
            if (!colorsInitialized) return true;

            Config config = Main.Config;

            return lastUseCustomBlipColor != config.UseCustomBlipColor ||
                   lastUseCustomTextColor != config.UseCustomTextColor ||
                   (config.UseCustomBlipColor && (
                       lastBlipRed != config.BlipColorRed ||
                       lastBlipGreen != config.BlipColorGreen ||
                       lastBlipBlue != config.BlipColorBlue)) ||
                   (config.UseCustomTextColor && (
                       lastTextRed != config.TextColorRed ||
                       lastTextGreen != config.TextColorGreen ||
                       lastTextBlue != config.TextColorBlue));
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