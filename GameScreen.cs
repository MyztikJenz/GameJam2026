using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameJam2026 {
    public abstract class GameScreen {
        public ScreenManager screenManager { get; internal set; }
        public bool isActive { get; set; } = false;
        public string instructions { get; set; } = "";
        public Viewport viewport { get; set; }

        public GameScreen(ScreenManager mgr) {
            screenManager = mgr;

            // The default viewport is for the game scene. Unique scenes will set their own.
            PresentationParameters pp = screenManager.GraphicsDevice.PresentationParameters;
            viewport = new Viewport(0, 0, 
                                    pp.BackBufferWidth - screenManager.bombPanelSize + 50, 
                                    pp.BackBufferHeight - screenManager.defusalPanelSize);

        }

        public virtual void Deactivate() { }
        public virtual void Load() { }
        public virtual void Unload() { }

        public virtual void Update(GameTime gameTime) { }

        public virtual void HandleInput(GameTime gameTime, InputState input) { }

        public virtual void Draw(GameTime gameTime) { 
            if (instructions.Length > 0) {
                Vector2 textSize = screenManager.font.MeasureString(instructions);
                Utilities.DrawString(screenManager, instructions, new Vector2(10, viewport.Bounds.Height - textSize.Y - 5));

            }
        }
    }
}