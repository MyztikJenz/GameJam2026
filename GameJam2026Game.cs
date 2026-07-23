using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace GameJam2026
{
    public class GameJam2026Game : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D pixelTexture;
        Rectangle playerRect;
        const int PlayerSize = 50;
        const float PlayerSpeed = 300f;

        public GameJam2026Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });

            playerRect = new Rectangle(100, 100, PlayerSize, PlayerSize);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            if (keyboardState.IsKeyDown(Keys.Escape) ||
                keyboardState.IsKeyDown(Keys.Back) ||
                gamePadState.Buttons.Back == ButtonState.Pressed)
            {
                try { Exit(); }
                catch (PlatformNotSupportedException) { /* ignore */ }
            }

            Vector2 movement = Vector2.Zero;
            if (keyboardState.IsKeyDown(Keys.Left)) movement.X -= 1f;
            if (keyboardState.IsKeyDown(Keys.Right)) movement.X += 1f;
            if (keyboardState.IsKeyDown(Keys.Up)) movement.Y -= 1f;
            if (keyboardState.IsKeyDown(Keys.Down)) movement.Y += 1f;

            if (movement != Vector2.Zero)
            {
                movement.Normalize();
                float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                playerRect.X = (int)MathHelper.Clamp(playerRect.X + movement.X * PlayerSpeed * deltaSeconds, 0, GraphicsDevice.PresentationParameters.BackBufferWidth - playerRect.Width);
                playerRect.Y = (int)MathHelper.Clamp(playerRect.Y + movement.Y * PlayerSpeed * deltaSeconds, 0, GraphicsDevice.PresentationParameters.BackBufferHeight - playerRect.Height);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(pixelTexture, playerRect, Color.Red);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
