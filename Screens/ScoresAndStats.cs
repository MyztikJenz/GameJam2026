using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameJam2026 {
    class ScoresAndStats : GameScreen, IScoresAndStatsInterface {
        GameDetail currentGame;
        IBombScreenService bombService;
        float newGameSplitTime;
        Texture2D bombIcon;
        float bombCurrentTime;
        float bombBestTime;

        public ScoresAndStats(ScreenManager manager) : base(manager) { 
            PresentationParameters pp = screenManager.GraphicsDevice.PresentationParameters;
            viewport = new Viewport(pp.BackBufferWidth - screenManager.bombPanelSize + 50, 0, 
                                    screenManager.bombPanelSize - 50, pp.BackBufferHeight - screenManager.bombPanelSize);
        }

        public override void Initialize() {
            GameJam2026Game.GameObj.Services.AddService(typeof(IScoresAndStatsInterface), this);

            base.Initialize();
        }

        public override void Load() { 
            bombService = GameJam2026Game.GameObj.Services.GetService(typeof(IBombScreenService)) as IBombScreenService;
            bombIcon = screenManager.contentMgr.Load<Texture2D>("icons/bomb_icon");
        }

        public override void Unload() { }

        public override void Update(GameTime gameTime) {
            if (currentGame != null) {
                currentGame.currentTime = newGameSplitTime - bombService.CurrentBombClock();

                bombCurrentTime = bombService.StartingBombClockTime() - bombService.CurrentBombClock();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
            SpriteBatch sb = screenManager.spriteBatch;
            screenManager.GraphicsDevice.Viewport = viewport;
            SpriteFont font = screenManager.cursedTimerSmallFont;

            sb.Begin(samplerState: SamplerState.PointClamp);
            sb.Draw(screenManager.blankTexture, new Rectangle(Point.Zero, viewport.Bounds.Size), Color.RoyalBlue);

            int yOffset = 10;
            int slide = 40; // 35px images + 5px spacing
            sb.DrawString(font, "Best", new Vector2(220, yOffset), Color.White);

            yOffset += slide;
            _drawScoreLayout(sb, bombIcon, font, bombCurrentTime == 0 ? "--.--" : bombCurrentTime.ToString("00.00"), 
                                                 bombBestTime == 0 ? "--.--" : bombBestTime.ToString("00.00"), yOffset);


            yOffset += slide;
            foreach(GameDetail game in GameDetails.games) {
                if (game.id == GameDetails.DefuseTheBombScreenID) {
                    continue;
                }
                _drawScoreLayout(sb, game.icon, font, game.currentTime == 0 ? "--.--" : game.currentTime.ToString("00.00"), 
                                                      game.bestTime == 0 ? "--.--" : game.bestTime.ToString("00.00"), yOffset);
                yOffset += slide;
            }


            sb.End();

            // Utilities.DebugString(screenManager, viewport.Bounds.Size.ToString(), Vector2.One);

            base.Draw(gameTime);
        }

        private void _drawScoreLayout(SpriteBatch sb, Texture2D icon, SpriteFont font, string currentTime, string bestTime, int yOffset) {
                int xOffset = 10;
                sb.Draw(icon, new Vector2(xOffset, yOffset), Color.White);
                xOffset += icon.Width + 15;
                var strSize = font.MeasureString(currentTime);
                sb.DrawString(font, currentTime, new Vector2(xOffset, yOffset), Color.White);
                xOffset += (int)strSize.X + 50;
                sb.DrawString(font, bestTime, new Vector2(xOffset, yOffset), Color.White);
        }

        //
        // IScoresAndStatsInterface
        //
        public void GameStarted(GameDetail game) {
            if (currentGame != null) {
                // If we got a new game and there was a current game, check to see if they got a high score
                if (currentGame.currentTime < currentGame.bestTime || currentGame.bestTime == 0f) {
                    currentGame.bestTime = currentGame.currentTime;
                }
            }

            currentGame = game;
            newGameSplitTime = bombService.CurrentBombClock();
        }

        public void GameFailed(GameDetail game) {
            currentGame = null;
        }

        public void BombDefused() {
            if (bombCurrentTime < bombBestTime || bombBestTime == 0f) {
                bombBestTime = bombService.StartingBombClockTime() - bombService.CurrentBombClock();;
            }
        }
    }

    public interface IScoresAndStatsInterface {
        void GameStarted(GameDetail game);
        void GameFailed(GameDetail game);
        void BombDefused();
    }
}

