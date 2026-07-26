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
        // string debugString = "";
        Button playButton;
        Button playAgainButton;
        Button toggleBackgroundMusicButton;
        internal bool bombWasDefused;

        public StartAndEndScreen(ScreenManager manager) : base(manager) { 
            viewport = screenManager.GraphicsDevice.Viewport;
        }

        public override void Load() {
            playButton = new Button("Play");
            playButton.Tapped += playButton_Tapped;

            playAgainButton = new Button("Play Again?");
            playAgainButton.Tapped += playAgainButton_Tapped;

            toggleBackgroundMusicButton = new Button("Toggle Music");
            toggleBackgroundMusicButton.Size = new Vector2(200, 50);
            toggleBackgroundMusicButton.Tapped += toggleMusic_Tapped;
            toggleBackgroundMusicButton.Position = new Vector2(viewport.Bounds.Right - toggleBackgroundMusicButton.Size.X - 10, viewport.Bounds.Bottom - 60);
            toggleBackgroundMusicButton.BorderColor = Color.Purple;

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
                playAgainButton.HandleTap(input.mousePosition().ToVector2());
                toggleBackgroundMusicButton.HandleTap(input.mousePosition().ToVector2());
            }
       }

        public override void Draw(GameTime gameTime) {
            SpriteBatch sb = screenManager.spriteBatch;
            SpriteFont font = screenManager.font;

            sb.Begin();
            sb.Draw(background, Vector2.Zero, Color.White);
            sb.End();

            if (isEndScreen) {
                sb.Begin(samplerState: SamplerState.PointClamp);
                if (bombWasDefused) {
                    string text = "Very wow! Much good!";
                    Vector2 textSize = font.MeasureString(text);
                    sb.DrawString(font, text, new Vector2(viewport.Width / 2 - textSize.X / 2, 100), Color.Navy);

                    text = "Try to go faster next time!";
                    textSize = font.MeasureString(text);
                    sb.DrawString(font, text, new Vector2(viewport.Width / 2 - textSize.X / 2, 140), Color.Navy);
                }
                else {
                    string text = "Oh... well, um...";
                    Vector2 textSize = font.MeasureString(text);
                    sb.DrawString(font, text, new Vector2(viewport.Width / 2 - textSize.X / 2, 100), Color.Navy);

                    text = "We hope you had fun! Try again!";
                    textSize = font.MeasureString(text);
                    sb.DrawString(font, text, new Vector2(viewport.Width / 2 - textSize.X / 2, 140), Color.Navy);
                }
                playAgainButton.Position = new Vector2(viewport.Width / 2 - playButton.Size.X / 2, 200);;
                playAgainButton.Draw(this);

                sb.End();
            }
            else {
                // Start screen
                sb.Begin(samplerState: SamplerState.PointClamp);
                string text = "Not Another Bomb Game";
                Vector2 textSize = font.MeasureString(text);
                sb.DrawString(font, text, new Vector2(viewport.Width / 2 - textSize.X / 2, 10), Color.Navy);
                
                text = "A game by MsWonderMom and MyztikJenz";
                textSize = font.MeasureString(text);
                sb.DrawString(font, text, new Vector2(viewport.Width / 2 - textSize.X / 2, 40), Color.Navy);


                playButton.Position = new Vector2(viewport.Width / 2 - playButton.Size.X / 2, 100);
                playButton.Draw(this);
                sb.End();
            }

            sb.Begin();
            toggleBackgroundMusicButton.Draw(this);
            sb.End();

            // Utilities.DebugString(screenManager, debugString, new Vector2(10, 10));
        }

        void playButton_Tapped(object sender, EventArgs e) {
            screenManager.ToggleBackgroundMusic();
            screenManager.ToggleBackgroundMusic();
            screenManager.GameHasFinished(this);
        }

        void playAgainButton_Tapped(object sender, EventArgs e) {
            screenManager.GameHasFinished(this);
        }

        void toggleMusic_Tapped(object sender, EventArgs e) {
            screenManager.ToggleBackgroundMusic();
        }
    }
}
