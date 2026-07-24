using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameJam2026 {
    class StartAndEndScreen : GameScreen {
        private bool _isEndScreen = false;
        private bool _updateBackground = true; // must run at least once.
        internal bool isEndScreen { get => _isEndScreen; set {
            _isEndScreen = value;
            _updateBackground = true;
        } }
        Texture2D background;
        string debugString = "";
        Button playButton;

        public StartAndEndScreen(ScreenManager manager) : base(manager) { 
            playButton = new Button("Play");
            playButton.Position = Vector2.One;
            playButton.Tapped += playButton_Tapped;
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            if (_updateBackground) {
                _updateBackground = false;

                Viewport viewport = screenManager.GraphicsDevice.Viewport;
                if (isEndScreen) {
                    background = Utilities.CreateLinearGradient(screenManager.GraphicsDevice, viewport.Bounds.Width, viewport.Bounds.Height, 
                                                                Color.DarkGray * 0.75f, Color.LightGray * 0.75f);
                }
                else {
                    background = Utilities.CreateLinearGradient(screenManager.GraphicsDevice, viewport.Bounds.Width, viewport.Bounds.Height, 
                                                                Color.Purple, Color.LightGray);
                }
            }
        }

        public override void HandleInput(GameTime gameTime, InputState input) {
            if (!isActive) { return; }

            if (input.isNewLeftMouseDown()) {
                playButton.HandleTap(input.mousePosition().ToVector2());
            }
       }

        public override void Draw(GameTime gameTime) {
            SpriteBatch sb = screenManager.spriteBatch;
            Viewport viewport = screenManager.GraphicsDevice.Viewport;

            sb.Begin();
            sb.Draw(background, Vector2.Zero, Color.White);
            sb.End();

            if (isEndScreen) {
                Vector2 strLoc = new Vector2(viewport.Width / 2, viewport.Height / 2 - 50);
                Utilities.DrawString(screenManager, "Ha Ha YoU sUcK!", strLoc);
            }
            else {
                // Start screen

                Vector2 strLoc = new Vector2(viewport.Width / 2, viewport.Height / 2 - 50);
                Utilities.DrawString(screenManager, "ShAlL wE pLaY a GaMe?", strLoc);

                strLoc.Y += 20;
                // Utilities.DrawString(screenManager, "Play", strLoc);
                sb.Begin();
                playButton.Position = strLoc;
                playButton.Draw(this);
                sb.End();
            }

            Utilities.DebugString(screenManager, debugString, new Vector2(10, 10));
        }

        void playButton_Tapped(object sender, EventArgs e) {
            debugString = "tapped";
            screenManager.GameHasFinished(this);
        }

    }
}
