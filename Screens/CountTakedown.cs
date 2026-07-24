using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace GameJam2026 {
    class CountTakedown : GameScreen {
        Viewport viewport;
        Texture2D vonCount;
        Texture2D ship;
        Vector2 playerPosition;
        Texture2D background;
        TitleString titleString;
        Texture2D bullet;
        int score;

        Random random = new Random();
        List<SimpleTracker> bullets = new List<SimpleTracker>();


        private struct SpritePositioning {
            internal float X { get; private set; }
            internal float Y { get; private set; }
            internal int? moveTo { get; set; }
            internal int speed { get; set; }

            internal SpritePositioning(int X, int Y) {
                this.X = X;
                this.Y = Y;
            }

            internal void UpdatePosition(GameTime gameTime) {
                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (moveTo.HasValue && moveTo.Value < X) {
                    X -= (float)speed * deltaTime;

                }
                else {
                    X += (float)speed * deltaTime;
                }

                // Removing the moveTo value requires an equality check on a float. Gross.
                // This should find when we're within "FLOAT_EPISILON" (wrt speed) of moveTo.
                if (moveTo.HasValue && Math.Abs(X - moveTo.Value) <= (float)speed * deltaTime) {
                    moveTo = null;
                }
            }

            internal Vector2 vector2() {
                return new Vector2(X, Y);
            }
        }

        SpritePositioning vonCountPosition;
        
        public CountTakedown(ScreenManager manager) : base(manager) { 
            PresentationParameters pp = screenManager.GraphicsDevice.PresentationParameters;
            viewport = new Viewport(0, 0, 
                                    pp.BackBufferWidth - screenManager.bombPanelSize + 50, 
                                    pp.BackBufferHeight - screenManager.defusalPanelSize);

        }

        public override void Load() {
            vonCount = screenManager.contentMgr.Load<Texture2D>("vonCount");
            ship = screenManager.contentMgr.Load<Texture2D>("ship");
            bullet = screenManager.contentMgr.Load<Texture2D>("bullet");

            playerPosition = new Vector2(viewport.Bounds.Width / 2, viewport.Bounds.Height - 5 - ship.Height);
            vonCountPosition = new SpritePositioning(viewport.Bounds.Width / 2, 10);

            background = Utilities.CreateLinearGradient(screenManager.GraphicsDevice, viewport.Bounds.Width, viewport.Bounds.Height, Color.Black, Color.DarkBlue);
            titleString = new TitleString("Take Down von Count!", Color.Red, Color.White);

        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            titleString.Update(gameTime);

            if (!vonCountPosition.moveTo.HasValue) {
                // Find a new target position and a random speed to get there
                vonCountPosition.moveTo = random.Next(0, viewport.Bounds.Width - vonCount.Width);
                vonCountPosition.speed = random.Next(100, 300);
            }

            vonCountPosition.UpdatePosition(gameTime);

            List<SimpleTracker> bulletsToRemove = new List<SimpleTracker>();
            Rectangle vonCountRect = new Rectangle(vonCountPosition.vector2().ToPoint(), vonCount.Bounds.Size);
            foreach (SimpleTracker bullet in bullets) {
                if (bullet.Intersects(vonCountRect)) {
                    score += 1;
                    bulletsToRemove.Add(bullet);
                }
                else if (bullet.HasArrived()) {
                    bulletsToRemove.Add(bullet);
                }
                else {
                    bullet.Update(gameTime);
                }
            }

            foreach (SimpleTracker bullet in bulletsToRemove) {
                bullets.Remove(bullet);
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

            // if (kState.IsKeyDown(Keys.Space)) {
            if (input.isNewKeyPress(Keys.Space)) {
                FireBullet(new Vector2(playerPosition.X + ship.Width / 2, viewport.Bounds.Height - 5 - ship.Height - 10));
            }


            if (movement.Length() > 1) {
                movement.Normalize();
            }

            playerPosition += movement * 8f;

            if (playerPosition.X < 0) {
                playerPosition.X = 0;
            }
            if (playerPosition.X > viewport.Bounds.Width - ship.Width) {
                playerPosition.X = viewport.Bounds.Width - ship.Width;
            }

        }

        public override void Draw(GameTime gameTime)
        {
            screenManager.GraphicsDevice.Viewport = viewport;

            SpriteBatch sb = screenManager.spriteBatch;
            sb.Begin();
            sb.Draw(background, Vector2.Zero, Color.White);
            sb.Draw(vonCount, vonCountPosition.vector2(), Color.White);

            foreach (SimpleTracker bullet in bullets) {
                sb.Draw(bullet.texture, bullet.position, Color.White);
            }

            sb.Draw(ship, playerPosition, Color.White);

            sb.End();

            titleString.Draw(screenManager, gameTime);
            // Utilities.DebugString(screenManager, "count: " + vonCountPosition.moveTo + " " + vonCountPosition.X, new Vector2(10, 5));
            if (score > 10) {
                Utilities.DebugString(screenManager, "score: " + score, new Vector2(10, 5));
            }

            Utilities.DebugString(screenManager, "Move: <- ->, Shoot: Space", new Vector2(10, viewport.Bounds.Height - 15));
            base.Draw(gameTime);
        }

        private void FireBullet(Vector2 initialPosition) {
            if (bullets.Count >= 5) { return; }

            Vector2 moveTo = new Vector2(initialPosition.X, 0 - bullet.Height);
            SimpleTracker b = new SimpleTracker(bullet, initialPosition, moveTo);
            b.ySpeed = 300;

            bullets.Add(b);
        }
    }
}
