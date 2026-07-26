using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameJam2026 {
    class DisarmTheBomb : GameScreen {
        float defuse_oscillator = 5f;
        float the_oscillator = 5f;
        float bomb_oscillator = 5f;
        public DisarmTheBomb(ScreenManager manager) : base(manager) {
            id = GameDetails.DefuseTheBombScreenID;
        }

        public override void Update(GameTime gameTime) {
            defuse_oscillator = Oscillate((float)gameTime.TotalGameTime.TotalSeconds * 3.5f, 1f, 5f);
            the_oscillator = Oscillate((20f + (float)gameTime.TotalGameTime.TotalSeconds) * 3.5f, 1f, 5f);
            bomb_oscillator = Oscillate((10f + (float)gameTime.TotalGameTime.TotalSeconds) * 3.5f, 1f, 5f);
        }

        public override void Draw(GameTime gameTime) {
            SpriteBatch sb = screenManager.spriteBatch;
            SpriteFont font = screenManager.font;

            sb.Begin(samplerState: SamplerState.PointClamp);
            sb.DrawString(font, "We're counting on you!", new Vector2(10, 10), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);

            var text = "Defuse";
            var textSize = font.MeasureString(text);
            sb.DrawString(font, text, new Vector2(viewport.Bounds.Width / 2 - textSize.X / 2, 100), Color.White, 0f, Vector2.Zero, defuse_oscillator, SpriteEffects.None, 1f);

            text = "The";
            textSize = font.MeasureString(text);
            sb.DrawString(font, text, new Vector2(viewport.Bounds.Width / 2 - textSize.X / 2, 300), Color.White, 0f, Vector2.Zero, the_oscillator, SpriteEffects.None, 1f);

            text = "Bomb!";
            textSize = font.MeasureString(text);
            sb.DrawString(font, text, new Vector2(viewport.Bounds.Width / 2 - textSize.X / 2, 500), Color.White, 0f, Vector2.Zero, bomb_oscillator, SpriteEffects.None, 1f);
            sb.End();

        }

        // Written by Gemini... at 2:57am :/ 
        float Oscillate(float t, float min, float max) {
            float range = max - min;
            if (range <= 0) return min;

            float cycle = range * 2f;
            float pos = t % cycle;

            // Handle negative input for t
            if (pos < 0) pos += cycle; 

            if (pos > range) {
                pos = cycle - pos;
            }

            return min + pos;
        }
    }
}
