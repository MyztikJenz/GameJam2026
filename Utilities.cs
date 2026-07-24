using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameJam2026 {
    public static class Utilities {

        public static Texture2D CreateLinearGradient(GraphicsDevice graphics, int width, int height, Color startColor, Color endColor) {
            Texture2D texture = new Texture2D(graphics, width, height);
            Color[] colorData = new Color[width * height];

            for (int y = 0; y < height; y++) {
                // Calculate interpolation factor along the height (0.0f to 1.0f)
                float amount = (float)y / (height - 1);
                Color blendedColor = Color.Lerp(startColor, endColor, amount);

                for (int x = 0; x < width; x++) {
                    colorData[y * width + x] = blendedColor;
                }
            }

            texture.SetData(colorData);
            return texture;
        }

        public static void DebugString(ScreenManager mgr, string text, Vector2 location) {
            SpriteBatch sb = mgr.spriteBatch;
            sb.Begin(samplerState: SamplerState.PointClamp);
            sb.DrawString(mgr.font, text, location, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            sb.End();

        }

    }
}