using Microsoft.Xna.Framework;

namespace GameJam2026 {
    public abstract class GameScreen {
        public ScreenManager screenManager { get; internal set; }
        public bool isActive { get; set; } = false;

        public GameScreen(ScreenManager mgr) {
            screenManager = mgr;
            // screenManager.AddScreen(this);
        }

        public virtual void Deactivate() { }
        public virtual void Load() { }
        public virtual void Unload() { }

        public virtual void Update(GameTime gameTime) { }

        public virtual void HandleInput(GameTime gameTime, InputState input) { }

        public virtual void Draw(GameTime gameTime) { }
    }
}