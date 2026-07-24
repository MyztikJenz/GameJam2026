using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameJam2026 {
    class ScoresAndStats : GameScreen {

        public ScoresAndStats(ScreenManager manager) : base(manager) { 
            PresentationParameters pp = screenManager.GraphicsDevice.PresentationParameters;
            viewport = new Viewport(pp.BackBufferWidth - screenManager.bombPanelSize + 50, 0, 
                                    screenManager.bombPanelSize + 50, pp.BackBufferHeight - screenManager.bombPanelSize);
        }

        public override void Load() {
        }

        public override void Unload() { }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            screenManager.GraphicsDevice.Viewport = viewport;

            // screenManager.GraphicsDevice.Clear(Color.Green);

            screenManager.spriteBatch.Begin();
            screenManager.spriteBatch.Draw(screenManager.blankTexture, new Rectangle(Point.Zero, viewport.Bounds.Size), Color.Green);
            screenManager.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

