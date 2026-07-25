using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace GameJam2026 {
    class BombScreen : GameScreen, IBombScreenService {

        Texture2D bomb;
        int gamesCompleted;
        bool gameIsActive;
        IDefusalInstructionsService defusalService;
        float startingBombClockTime = 60f; // seconds
        float bombClock; // running clock when the game is active
        SoundEffect tickingClock;
        SoundEffectInstance tickingClockInstance;


        Rectangle displayRect = new Rectangle(104, 18, 195, 63);

        public BombScreen(ScreenManager mgr) : base(mgr) { 
            PresentationParameters pp = screenManager.GraphicsDevice.PresentationParameters;
            viewport = new Viewport(pp.BackBufferWidth - screenManager.bombPanelSize, pp.BackBufferHeight - screenManager.bombPanelSize,
                                    screenManager.bombPanelSize, screenManager.bombPanelSize);

        }

        public override void Initialize() {
            screenManager.Game.Services.AddService(typeof(IBombScreenService), this);

            base.Initialize();
        }

        public override void Load() {
            bomb = screenManager.contentMgr.Load<Texture2D>("bomb/bomb");

            tickingClock = screenManager.contentMgr.Load<SoundEffect>("sounds/dragon-studio-clock-ticking-sfx-467486-edited");
            tickingClockInstance = tickingClock.CreateInstance();
            tickingClockInstance.IsLooped = true;
            tickingClockInstance.Volume = 0.1f;


            defusalService = screenManager.Game.Services.GetService(typeof(IDefusalInstructionsService)) as IDefusalInstructionsService;
        }

        public override void Update(GameTime gameTime) {
            if (gameIsActive) {
                bombClock -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                float newVolume = 0.1f; 
                if (bombClock <= 10f) { newVolume = 1.0f; }
                else if (bombClock <= 20f) { newVolume = 0.8f; }
                else if (bombClock <= 30f) { newVolume = 0.5f; }
                else if (bombClock <= 40f) { newVolume = 0.3f; }
                else if (bombClock <= 50f) { newVolume = 0.2f; }
                tickingClockInstance.Volume = newVolume;


                if (bombClock <= 0.00001f) {
                    gameIsActive = false;
                    tickingClockInstance.Stop();
                    screenManager.BombExploded();
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
            SpriteBatch sb = screenManager.spriteBatch;
            Rectangle playerRect = new Rectangle(100, 100, 50, 50);

            screenManager.GraphicsDevice.Viewport = viewport;


            sb.Begin(samplerState: SamplerState.PointClamp);
            sb.Draw(screenManager.blankTexture, new Rectangle(Point.Zero, viewport.Bounds.Size), Color.DarkGray);

            // draw the clock/display
            string displayString = bombClock.ToString("00.00");
            var stringSize = screenManager.cursedTimerFont.MeasureString(displayString);
            sb.Draw(screenManager.blankTexture, displayRect, Color.Black);
            int smallYAxisFontAdjustment = 5;
            Color textColor = Color.Green;
            if (bombClock < 10f) { textColor = Color.Red; }
            else if (bombClock < 30f) { textColor = Color.Yellow; }
            sb.DrawString(screenManager.cursedTimerFont, displayString, 
                            new Vector2(displayRect.Left + displayRect.Width / 2 - stringSize.X / 2, 
                                        displayRect.Top + displayRect.Height / 2 - stringSize.Y / 2 + smallYAxisFontAdjustment), 
                            textColor);


            sb.Draw(bomb, Point.Zero.ToVector2(), Color.White);
            sb.End();

            // Utilities.DebugString(screenManager, bombClock.ToString(), Vector2.One);
            base.Draw(gameTime);
        }

        //
        // IBombScreenService
        //
        public void GameCompleted() {
            gamesCompleted += 1;
            defusalService.RevealDoor(gamesCompleted);
        }

        public void GameStarted() {
            gameIsActive = true;
            bombClock = startingBombClockTime;
            tickingClockInstance.Play();
        }

        public float CurrentBombClock() {
            return bombClock;
        }

        public float StartingBombClockTime() {
            return startingBombClockTime;
        }
    }

    public interface IBombScreenService {
        void GameStarted();
        void GameCompleted();
        float CurrentBombClock();
        float StartingBombClockTime();
    }
}