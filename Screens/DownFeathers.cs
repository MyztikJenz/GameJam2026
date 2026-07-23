using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameJam2026 {
    class DownFeathers : GameScreen {
        // We need to move this into the subscreen. GameplayScreen should never get input.
        Vector2 playerPosition = new Vector2(100, 100);

        Viewport viewport;

        public DownFeathers(ScreenManager manager) : base(manager) { 
            // viewport = new Viewport(0, 0, 
            //                         screenManager.GraphicsDevice.PresentationParameters.BackBufferWidth - 100, 
            //                         screenManager.GraphicsDevice.PresentationParameters.BackBufferHeight - 100);

            PresentationParameters pp = screenManager.GraphicsDevice.PresentationParameters;
            viewport = new Viewport(0, 0, 
                                    pp.BackBufferWidth - screenManager.bombPanelSize + 50, 
                                    pp.BackBufferHeight - screenManager.defusalPanelSize);

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
            // screenManager.GraphicsDevice.Clear(Color.Chartreuse);

            screenManager.spriteBatch.Begin();
            screenManager.spriteBatch.Draw(screenManager.blankTexture, new Rectangle(Point.Zero, viewport.Bounds.Size), Color.Yellow);
            screenManager.spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}

