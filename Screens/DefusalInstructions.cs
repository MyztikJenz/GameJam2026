using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameJam2026 {
    class DefusalInstructions : GameScreen {

        public DefusalInstructions(ScreenManager manager) : base(manager) { 
            PresentationParameters pp = screenManager.GraphicsDevice.PresentationParameters;
            viewport = new Viewport(0, pp.BackBufferHeight - screenManager.defusalPanelSize,
                                    pp.BackBufferWidth - screenManager.bombPanelSize, screenManager.defusalPanelSize);

        }

        public override void Load() {
            // Do something here to load stuff we need
        }

        public override void Unload() { }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            screenManager.GraphicsDevice.Viewport = viewport;
            // screenManager.GraphicsDevice.Clear(Color.Purple);

            screenManager.spriteBatch.Begin();
            screenManager.spriteBatch.Draw(screenManager.blankTexture, new Rectangle(Point.Zero, viewport.Bounds.Size), Color.Purple);
            screenManager.spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}

