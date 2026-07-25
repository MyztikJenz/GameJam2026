using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameJam2026
{
    public class GameJam2026Game : Game {
        public const int FixedWidth = 1465;
        public const int FixedHeight = 768;

        GraphicsDeviceManager graphics;
        ScreenManager screenManager;
        bool resizing;

        public GameJam2026Game() {
            graphics = new GraphicsDeviceManager(this);
            ConfigureFixedBackBuffer();
            Content.RootDirectory = "Content";

            screenManager = new ScreenManager(this);
            Components.Add(screenManager);
        }

        protected override void Initialize() {
            ConfigureFixedBackBuffer();
            graphics.ApplyChanges();
            Window.AllowUserResizing = false;
            Window.ClientSizeChanged += Window_ClientSizeChanged;
            base.Initialize();
        }

        protected override void LoadContent() { 
            base.LoadContent();
        }

        protected override void UnloadContent() { 
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime) {
            if (GraphicsDevice.PresentationParameters.BackBufferWidth != FixedWidth ||
                GraphicsDevice.PresentationParameters.BackBufferHeight != FixedHeight ||
                Window.ClientBounds.Width != FixedWidth ||
                Window.ClientBounds.Height != FixedHeight) {
                ApplyFixedSize();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component, which gets called
            // because it's a component. Neat.
            base.Draw(gameTime);
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e) {
            ApplyFixedSize();
        }

        private void ConfigureFixedBackBuffer() {
            graphics.PreferredBackBufferWidth = FixedWidth;
            graphics.PreferredBackBufferHeight = FixedHeight;
            graphics.IsFullScreen = false;
        }

        private void ApplyFixedSize() {
            if (resizing) {
                return;
            }

            resizing = true;
            try {
                ConfigureFixedBackBuffer();
                graphics.ApplyChanges();

                MethodInfo changeClientSize = Window.GetType().GetMethod("ChangeClientSize", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                changeClientSize?.Invoke(Window, new object[] { FixedWidth, FixedHeight });
            }
            finally {
                resizing = false;
            }
        }
    }
}
