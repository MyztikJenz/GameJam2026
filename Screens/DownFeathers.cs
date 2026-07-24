using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GameJam2026 {
    class DownFeathers : GameScreen {
        Vector2 playerPosition;
        Texture2D goose;
        Texture2D background;
        TitleString titleString;
        int score;

        private List<Feather> sourceFeathers;
        private List<Feather> displayedFeathers = new List<Feather>();

        Rectangle g;
       
        public DownFeathers(ScreenManager manager) : base(manager) { 
            PresentationParameters pp = screenManager.GraphicsDevice.PresentationParameters;
            viewport = new Viewport(0, 0, 
                                    pp.BackBufferWidth - screenManager.bombPanelSize + 50, 
                                    pp.BackBufferHeight - screenManager.defusalPanelSize);
                                    
            instructions = "Move: <- ->";
        }

        public override void Load() {
            goose = screenManager.contentMgr.Load<Texture2D>("goose");
            playerPosition = new Vector2(viewport.Bounds.Width / 2, viewport.Bounds.Height - 5 - goose.Height);

            sourceFeathers = Feather.CreateFeathers(screenManager.contentMgr, viewport.Bounds.Width);
            for (int x=0; x<7; x++) {
                Feather f = sourceFeathers[Random.Shared.Next(sourceFeathers.Count)];
                displayedFeathers.Add(new Feather(f));
            }

            background = Utilities.CreateLinearGradient(screenManager.GraphicsDevice, viewport.Bounds.Width, viewport.Bounds.Height, Color.Yellow, Color.Red);

            titleString = new TitleString("Down Feathers!");
        }

        public override void Unload() { }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            // Does the center-ish square of the goose touch the feather?
            g = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, goose.Bounds.Width, goose.Bounds.Height);
            g.Inflate(goose.Bounds.Width / -6, goose.Bounds.Height / -6);

            var feathersToRemove = new List<Feather>();
            foreach (Feather f in displayedFeathers) {
                f.Update(gameTime);

                if (f.Intersects(g)) {
                    score += 1;
                    feathersToRemove.Add(f);
                }

                // Is the feather off screen?
                if (f.featherPosition.Y > viewport.Bounds.Height) {
                    feathersToRemove.Add(f);
                }
            }

            foreach (Feather f in feathersToRemove) {
                displayedFeathers.Remove(f);

                // Out with the old, in with the new.
                Feather newFeather = sourceFeathers[Random.Shared.Next(sourceFeathers.Count)];
                displayedFeathers.Add(new Feather(newFeather));
            }

            titleString.Update(gameTime);

            if (score >= 10) {
                screenManager.GameHasFinished(this);
            }
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
            // if (kState.IsKeyDown(Keys.Up) || kState.IsKeyDown(Keys.W)) {
            //     movement.Y--;
            // }
            // if (kState.IsKeyDown(Keys.Down) || kState.IsKeyDown(Keys.S)) {
            //     movement.Y++;
            // }

            if (movement.Length() > 1) {
                movement.Normalize();
            }

            playerPosition += movement * 8f;

            if (playerPosition.X < 0) {
                playerPosition.X = 0;
            }
            if (playerPosition.X > viewport.Bounds.Width - goose.Width) {
                playerPosition.X = viewport.Bounds.Width - goose.Width;
            }
        }


        public override void Draw(GameTime gameTime)
        {
            screenManager.GraphicsDevice.Viewport = viewport;

            SpriteBatch sb = screenManager.spriteBatch;
            sb.Begin();
            sb.Draw(background, Vector2.Zero, Color.White);
            sb.Draw(goose, playerPosition, Color.White);

            foreach (Feather f in displayedFeathers) {
                f.Draw(screenManager, gameTime);
            }

            sb.End();

            titleString.Draw(screenManager, gameTime);

            base.Draw(gameTime);
        }
    }
}

