using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameJam2026 {
    class LongWayDown : GameScreen {
        Vector2 playerPosition;
        Texture2D dude;
        Texture2D background;
        TitleString titleString;
        int score;

        public LongWayDown(ScreenManager manager) : base(manager) {
            instructions = "Move: <- ->, Type: numbers";
        }

        public override void Load() {
            dude = screenManager.contentMgr.Load<Texture2D>("fall_down_guy");
            background = Utilities.CreateLinearGradient(screenManager.GraphicsDevice, viewport.Bounds.Width, viewport.Bounds.Height, Color.DarkSlateBlue, Color.LightSteelBlue);
            titleString = new TitleString("Long Way Down!", Color.Yellow, Color.Gray);
            playerPosition = new Vector2(viewport.Bounds.Width / 2, 25);


            base.Load();
        }

        public override void Update(GameTime gameTime) {
            titleString.Update(gameTime);

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

            if (movement.Length() > 1) {
                movement.Normalize();
            }

            playerPosition += movement * 8f;

            if (playerPosition.X < 0) {
                playerPosition.X = 0;
            }
            if (playerPosition.X > viewport.Bounds.Width - dude.Width) {
                playerPosition.X = viewport.Bounds.Width - dude.Width;
            }

        }


        public override void Draw(GameTime gameTime) {
            screenManager.GraphicsDevice.Viewport = viewport;

            SpriteBatch sb = screenManager.spriteBatch;
            sb.Begin();
            sb.Draw(background, Vector2.Zero, Color.White);
            sb.Draw(dude, playerPosition, Color.White);
            sb.End();

            titleString.Draw(screenManager, gameTime);

            base.Draw(gameTime);
        }
    }
}