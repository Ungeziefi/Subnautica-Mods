using UnityEngine;

namespace Ungeziefi.Creature_Healthbars;

public partial class CreatureHealthbars
{
    private static void CreateSprite()
    {
        if (roundedSprite != null) return;

        var width = Main.Config.SpriteWidth;
        var height = Main.Config.SpriteHeight;
        Texture2D texture = new(width, height, TextureFormat.RGBA32, true);
        var pixels = new Color[width * height];

        var radiusPercentage = Main.Config.CornerRoundness;
        var radius = Mathf.Min(height, width) * radiusPercentage;
        radius = Mathf.Clamp(radius, 0, Mathf.Min(height, width) / 2f);

        for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
        {
            // Calculate distance to the nearest inner corner point
            var dx = Mathf.Max(radius - x, 0, x - (width - 1 - radius));
            var dy = Mathf.Max(radius - y, 0, y - (height - 1 - radius));
            var dist = Mathf.Sqrt(dx * dx + dy * dy);

            if (dist <= radius)
            {
                // Fade alpha between the edge of the radius and 1 pixel inward for some AA
                var alpha = Mathf.Clamp01(radius - dist + 0.5f);
                pixels[y * width + x] = new Color(1, 1, 1, alpha);
            }
            else
            {
                pixels[y * width + x] = Color.clear;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        // Prevent edge bleeding
        texture.wrapMode = TextureWrapMode.Clamp;
        roundedSprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100);
    }
}