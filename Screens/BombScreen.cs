using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameJam2026 {
    class BombScreen : GameScreen {

        Texture2D bomb;

        public BombScreen(ScreenManager mgr) : base(mgr) { 
            PresentationParameters pp = screenManager.GraphicsDevice.PresentationParameters;
            viewport = new Viewport(pp.BackBufferWidth - screenManager.bombPanelSize, pp.BackBufferHeight - screenManager.bombPanelSize,
                                    screenManager.bombPanelSize, screenManager.bombPanelSize);

        }

        public override void Load() {
            bomb = screenManager.contentMgr.Load<Texture2D>("bomb");
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager mgr = this.screenManager;
            Rectangle playerRect = new Rectangle(100, 100, 50, 50);

            mgr.GraphicsDevice.Viewport = viewport;

            mgr.spriteBatch.Begin();
            mgr.spriteBatch.Draw(mgr.blankTexture, new Rectangle(Point.Zero, viewport.Bounds.Size), Color.Red);
            mgr.spriteBatch.Draw(bomb, Point.Zero.ToVector2(), Color.White);
            mgr.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}