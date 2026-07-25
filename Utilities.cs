using System;
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

        private static void _drawString(ScreenManager mgr, string text, Vector2 location, Color color) {
            SpriteBatch sb = mgr.spriteBatch;
            try {
                sb.Begin(samplerState: SamplerState.PointClamp);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Begin cannot be called again", StringComparison.OrdinalIgnoreCase)) {
                throw new InvalidOperationException(
                    "Utilities.DrawString cannot be called while a SpriteBatch is already between Begin() and End(). " +
                    "Draw the string with the active SpriteBatch instead, or call Utilities.DrawString after ending the current batch.",
                    ex);
            }

            try {
                sb.DrawString(mgr.font, text, location, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            finally {
                sb.End();
            }
        }

        public static void DrawString(ScreenManager mgr, string text, Vector2 location, Color? color = null) {
            Color textColor = color.HasValue ? color.Value : Color.White;
            _drawString(mgr, text, location, textColor);
        }

        public static void DrawString(ScreenManager mgr, string text, Point location, Color? color = null) {
            Color textColor = color.HasValue ? color.Value : Color.White;
            _drawString(mgr, text, location.ToVector2(), textColor);
        }

        public static void DebugString(ScreenManager mgr, string text, Vector2 location) {
            _drawString(mgr, text, location, Color.White);
        }

    }
}
