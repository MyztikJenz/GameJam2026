using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameJam2026 {
    class GameplayScreen : GameScreen {
        // We need to move this into the subscreen. GameplayScreen should never get input.
        Vector2 playerPosition = new Vector2(100, 100);

        public GameplayScreen(ScreenManager manager) : base(manager) { }

        public override void Load() {
            // Do something here to load stuff we need
        }

        public override void Unload() { }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
        }

        public override void HandleInput(GameTime gameTime, InputState input) {
            int playerIdx = (int)PlayerIndex.One;
            KeyboardState kState = input.currentKeyboardStates[playerIdx];

            Vector2 movement = Vector2.Zero;
            if (kState.IsKeyDown(Keys.Left) || kState.IsKeyDown(Keys.A)) {
                movement.X--;
            }
            if (kState.IsKeyDown(Keys.Right) || kState.IsKeyDown(Keys.D)) {
                movement.X++;
            }
            if (kState.IsKeyDown(Keys.Up) || kState.IsKeyDown(Keys.W)) {
                movement.Y--;
            }
            if (kState.IsKeyDown(Keys.Down) || kState.IsKeyDown(Keys.S)) {
                movement.Y++;
            }

            if (movement.Length() > 1) {
                movement.Normalize();
            }


        }
    }
}