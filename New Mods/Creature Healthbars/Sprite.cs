using UnityEngine;

namespace Ungeziefi.Creature_Healthbars
{
    public partial class CreatureHealthbars
    {
        private static void CreateSprite()
        {
            if (roundedSprite != null) return;

            int width = Main.Config.SpriteWidth;
            int height = Main.Config.SpriteHeight;
            Texture2D texture = new(width, height, TextureFormat.RGBA32, true);
            Color[] pixels = new Color[width * height];

            // Fill with transparent pixels initially
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = Color.clear;

            float radiusPercentage = Main.Config.CornerRoundness;

            // Radius based on smaller dimension, then clamp to half of it
            int radius = Mathf.RoundToInt(Mathf.Min(height, width) * radiusPercentage);
            radius = Mathf.Clamp(radius, 0, Mathf.Min(height, width) / 2);

            // Draw rounded rectangle
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // If radius is 0, just fill the entire rectangle
                    if (radius == 0)
                    {
                        pixels[y * width + x] = Color.white;
                        continue;
                    }

                    // Corner regions
                    bool isInTopLeftCorner = x < radius && y < radius;
                    bool isInTopRightCorner = x >= width - radius && y < radius;
                    bool isInBottomLeftCorner = x < radius && y >= height - radius;
                    bool isInBottomRightCorner = x >= width - radius && y >= height - radius;

                    // Apply rounded corners based on distance from corner center
                    if (isInTopLeftCorner)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), new Vector2(radius, radius));
                        if (distance <= radius)
                            pixels[y * width + x] = Color.white;
                    }
                    else if (isInTopRightCorner)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), new Vector2(width - radius - 1, radius));
                        if (distance <= radius)
                            pixels[y * width + x] = Color.white;
                    }
                    else if (isInBottomLeftCorner)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), new Vector2(radius, height - radius - 1));
                        if (distance <= radius)
                            pixels[y * width + x] = Color.white;
                    }
                    else if (isInBottomRightCorner)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), new Vector2(width - radius - 1, height - radius - 1));
                        if (distance <= radius)
                            pixels[y * width + x] = Color.white;
                    }
                    else
                    {
                        // Fill non-corner areas
                        pixels[y * width + x] = Color.white;
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            texture.filterMode = FilterMode.Trilinear;
            roundedSprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100);
        }
    }
}