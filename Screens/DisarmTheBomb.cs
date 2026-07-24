using Microsoft.Xna.Framework;

namespace GameJam2026 {
    class DisarmTheBomb : GameScreen {
        private float counter = 0f;

        public DisarmTheBomb(ScreenManager manager) : base(manager) {

        }

        public override void Update(GameTime gameTime) {
            counter += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (counter >= 3.0) {
                screenManager.GameHasFinished(this);
            }
        }

        public override void Draw(GameTime gameTime) {
            Utilities.DrawString(screenManager, "Disarm the bomb!", Vector2.One);
        }
    }
}
