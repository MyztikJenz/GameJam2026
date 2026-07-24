using Microsoft.Xna.Framework;

namespace GameJam2026 {
    class DisarmTheBomb : GameScreen {

        public DisarmTheBomb(ScreenManager manager) : base(manager) {

        }

        public override void Draw(GameTime gameTime) {
            Utilities.DrawString(screenManager, "Disarm the bomb!", Vector2.One);
        }
    }
}
