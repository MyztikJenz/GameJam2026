using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameJam2026 {
    public class TitleString {

        private string text;
        private double timer = 0f;
        private double opacity = 1f;
        private Color bgColor;
        private Color fgColor;

        public TitleString(string textToDraw, Color? bgColor = null, Color? fgColor = null) {
            text = textToDraw;
            this.bgColor = bgColor.HasValue ? bgColor.Value : Color.White;
            this.fgColor = fgColor.HasValue ? fgColor.Value : Color.Black;
        }

        public void Update(GameTime gameTime) {
            if (opacity <= 0) { return; }

            timer += gameTime.ElapsedGameTime.Milliseconds / 1000.0;

            if (timer > 2.0) {
                opacity -= (timer - 2.0) / 30f;
            }
        }

        // Do not call this from inside an active SpriteBatch block. We need to use the PointClamp sampler to get a toasty font.
        public void Draw(ScreenManager mgr, GameTime gameTime) {
            if (opacity <= 0) { return; }

            SpriteBatch sb = mgr.spriteBatch;
            sb.Begin(samplerState: SamplerState.PointClamp);
            sb.DrawString(mgr.font, text, new Vector2(5, 5), bgColor * (float)opacity, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0f);
            sb.DrawString(mgr.font, text, new Vector2(8, 8), fgColor * (float)opacity, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0f);
            sb.End();

        }

        public void Reset() {
            timer = 0f;
            opacity = 1f;
        }

    }

}