using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace GameJam2026 {
    public class ScreenManager : DrawableGameComponent {

        private readonly List<GameScreen> screens = new List<GameScreen>();
        private readonly List<GameScreen> tempScreensList = new List<GameScreen>();
        private readonly List<GameScreen> gamesToPlay = new List<GameScreen>();


        private readonly InputState input = new InputState();
        public SpriteBatch spriteBatch { get; private set; }
        public SpriteFont font { get; private set; }
        public Texture2D blankTexture { get; private set; }

        public ContentManager contentMgr { get; private set; }


        public readonly int bombPanelSize = 400;
        public readonly int defusalPanelSize = 150;

        bool gameIsActive = false;

        Viewport viewport;

        StartAndEndScreen startAndEndScreen;
        BombScreen bombScreen;
        ScoresAndStats scoresAndStatsScreen;
        DownFeathers downFeathersScreen;
        DefusalInstructions defusalInstructionsScreen;
        CountTakedown countTakedownScreen;
        DisarmTheBomb disarmTheBombScreen;
        Hangman hangmanScreen;
        LongWayDown longWayDownScreen;
        WordCount wordCountScreen;

        GameScreen currentGameScreen;

        public SoundEffect buzzer;

        public ScreenManager(Game game) : base(game) {
        }

        // public void AddScreen(GameScreen gameScreen) {
        //     screens.Add(gameScreen);
        // }

        public override void Initialize() {
            contentMgr = new ContentManager(this.Game.Services, "Content");

            startAndEndScreen = new StartAndEndScreen(this)
            {
                isActive = true
            };
            bombScreen = new BombScreen(this) { 
                isActive = true 
            };
            scoresAndStatsScreen = new ScoresAndStats(this);
            downFeathersScreen = new DownFeathers(this);
            defusalInstructionsScreen = new DefusalInstructions(this);
            countTakedownScreen = new CountTakedown(this);
            disarmTheBombScreen = new DisarmTheBomb(this);
            hangmanScreen = new Hangman(this);
            longWayDownScreen = new LongWayDown(this);
            wordCountScreen = new WordCount(this);


            // Register screens so their Load() methods are called from LoadContent
            screens.Add(startAndEndScreen);
            screens.Add(bombScreen);
            screens.Add(scoresAndStatsScreen);
            screens.Add(downFeathersScreen);
            screens.Add(defusalInstructionsScreen);
            screens.Add(countTakedownScreen);
            screens.Add(disarmTheBombScreen);
            screens.Add(hangmanScreen);
            screens.Add(longWayDownScreen);
            screens.Add(wordCountScreen);

            buzzer = contentMgr.Load<SoundEffect>("sounds/yusuf_sfx-wrong-buzzer-double-491796");

            base.Initialize();
        }

        protected override void LoadContent() {
            ContentManager content = Game.Content;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = content.Load<SpriteFont>("PressStart2P");
            blankTexture = new Texture2D(GraphicsDevice, 1, 1);
            blankTexture.SetData(new[] { Color.White.PackedValue });

            foreach (GameScreen screen in screens) {
                screen.Load();
            }
        }

        protected override void UnloadContent() {
            // Maybe tell each screen to unload?
        }

        public override void Update(GameTime gameTime) {
            input.Update();

            startAndEndScreen.HandleInput(gameTime, input);
            if (currentGameScreen != null) {
                currentGameScreen.HandleInput(gameTime, input);
            }
            bombScreen.HandleInput(gameTime, input);

            foreach (GameScreen screen in screens) {
                if (screen.isActive) {
                    screen.Update(gameTime);
                }
            }
            // For each screen we need to update it. 
            // Since our screens are going to be composed, we need to ensure the subscreens are
            // appropriately marked as who's getting input. And where they can draw.
            // TODO: the bomb screen will need to move to the smaller subscreen to the bigger one
            //       once they finish the run through the minigames. What replaces it? who knows...
        }

        public override void Draw(GameTime gameTime) {
            // Each screen is going to modify the viewport, so we stash the original here and restore it once they're done.
            viewport = GraphicsDevice.Viewport;

            try {
                if (currentGameScreen != null) {
                    // We draw our screens in a very specific order. And we're going to have a fixed number.
                    // 1. Draw the current minigame
                    currentGameScreen.Draw(gameTime);
                    
                    // 2. Draw the score/state panel
                    scoresAndStatsScreen.Draw(gameTime);

                    // // 3. Draw the defusal panel
                    defusalInstructionsScreen.Draw(gameTime);

                    // // 4. Draw the bomb panel
                    bombScreen.Draw(gameTime);
                }

                if (!gameIsActive) {
                    GraphicsDevice.Viewport = viewport;
                    startAndEndScreen.Draw(gameTime);
                }
            }
            finally {
                GraphicsDevice.Viewport = viewport;
            }

            Utilities.DebugString(this, viewport.Bounds.ToString(), Vector2.One);
        }

        // Called when a game has completed. Time to move on.
        internal void GameHasFinished(GameScreen screen) {
            if (screen == startAndEndScreen) {
                if (startAndEndScreen.isEndScreen) {

                }
                else {
                    gameIsActive = true;
                    startAndEndScreen.isActive = false;
                    WhichGamesToPlay();
                    currentGameScreen = GetNextGame();
                    currentGameScreen.isActive = true;
                }
            }
            else if (screen == disarmTheBombScreen) {
                // Just a placeholder for a real end-game
                gameIsActive = false;
                currentGameScreen.isActive = false;
                startAndEndScreen.isActive = true;
                startAndEndScreen.isEndScreen = true;
            }
            else {
                currentGameScreen.isActive = false;
                currentGameScreen = GetNextGame();
                currentGameScreen.isActive = true;
            }
        }

        internal GameScreen GetNextGame() {
            if (gamesToPlay.Count == 0) {
                // They just need to disarm the bomb
                // We'll eventually draw something pointing them to the bomb
                return disarmTheBombScreen;
            }
            else {
                GameScreen gs = gamesToPlay[0];
                gamesToPlay.RemoveAt(0);
                return gs;
            }
        }

        internal void WhichGamesToPlay() {
            gamesToPlay.Clear();

            // TODO: This will need to be smarter once we get more games finished
            gamesToPlay.Add(longWayDownScreen);
            gamesToPlay.Add(wordCountScreen);
            gamesToPlay.Add(downFeathersScreen);
            gamesToPlay.Add(countTakedownScreen);

        }

    }
}
