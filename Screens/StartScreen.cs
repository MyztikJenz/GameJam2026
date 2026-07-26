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
        Texture2D trashPanda;
        Texture2D kangaroo;
        Texture2D boxer;
        Texture2D body;
        Texture2D[] heads = new Texture2D[5];

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

            trashPanda = screenManager.contentMgr.Load<Texture2D>("didnt_make_it_in/trash_panda");
            kangaroo = screenManager.contentMgr.Load<Texture2D>("didnt_make_it_in/kangaroo");
            boxer = screenManager.contentMgr.Load<Texture2D>("didnt_make_it_in/boxer");
            body = screenManager.contentMgr.Load<Texture2D>("didnt_make_it_in/Body");

            for(int x=0; x<heads.Length; x++) {
               heads[x] = screenManager.contentMgr.Load<Texture2D>("didnt_make_it_in/Head_" + (x+1));
            }

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

            float scaleUp = 1.5f;
            sb.Begin(samplerState: SamplerState.PointClamp);
            sb.Draw(kangaroo, new Vector2(0, viewport.Bounds.Bottom - kangaroo.Height * scaleUp), null, Color.White, 0f, Vector2.Zero, scaleUp, SpriteEffects.None, 1f);
            if (!isEndScreen) {
                string text  = "Characters that";
                string text2 = "didn't make it in";
                Vector2 textSize = font.MeasureString(text2);
                Vector2 bottomString = new Vector2(20, viewport.Bounds.Bottom - 20 - textSize.Y);
                sb.DrawString(font, text2, bottomString, Color.MediumPurple);
                textSize = font.MeasureString(text);
                sb.DrawString(font, text, new Vector2(20, bottomString.Y - textSize.Y), Color.MediumPurple);
            }
            sb.Draw(body, new Vector2(viewport.Bounds.Center.X - body.Width / 2, viewport.Bounds.Bottom - body.Height - 75), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            int xOffset = viewport.Bounds.Center.X - heads[0].Width * 2 - (heads[0].Width / 2) - 10;
            int slide = heads[0].Width + 5;
            for (int x=0; x<heads.Length; x++) {
                sb.Draw(heads[x], new Vector2(xOffset, viewport.Bounds.Bottom - body.Height - 75 - heads[x].Height), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                xOffset += slide;
            }
            sb.Draw(trashPanda, new Vector2(viewport.Bounds.Right - trashPanda.Width * scaleUp, viewport.Bounds.Bottom - trashPanda.Height * scaleUp), null, Color.White, 0, Vector2.Zero, scaleUp, SpriteEffects.FlipHorizontally, 1f);
            sb.Draw(boxer, new Vector2(viewport.Bounds.Left + 20, viewport.Bounds.Top + 30), null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 1f);
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
                Vector2 textSize = screenManager.chalkFont.MeasureString(text);
                sb.DrawString(screenManager.chalkFont, text, new Vector2(viewport.Width / 2 - textSize.X / 2, 10), Color.Navy);
                var yOffset = textSize.Y;

                text = "A game by MsWonderMom and MyztikJenz";
                textSize = font.MeasureString(text);
                sb.DrawString(font, text, new Vector2(viewport.Width / 2 - textSize.X / 2, yOffset + 20), Color.Navy);

                playButton.Position = new Vector2(viewport.Width / 2 - playButton.Size.X / 2, yOffset + 80);
                playButton.Draw(this);

                text = "Thank you for playing!";
                textSize = font.MeasureString(text);
                sb.DrawString(font, text, new Vector2(viewport.Width / 2 - textSize.X / 2, viewport.Bounds.Bottom - textSize.Y - 10), Color.MediumPurple);

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
