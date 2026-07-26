using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Media;

namespace GameJam2026 {
    public class ScreenManager : DrawableGameComponent {

        private readonly List<GameScreen> screens = new List<GameScreen>();
        private readonly List<GameScreen> tempScreensList = new List<GameScreen>();
        private readonly List<GameScreen> gamesToPlay = new List<GameScreen>();


        private readonly InputState input = new InputState();
        public SpriteBatch spriteBatch { get; private set; }
        public SpriteFont font { get; private set; }
        internal SpriteFont cursedTimerFont { get; private set; }
        internal SpriteFont cursedTimerSmallFont { get; private set; }
        internal SpriteFont cursedTimer12ptFont { get; private set; }
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

        List<GameScreen> allGames = new List<GameScreen>();

        IBombScreenService bombService;
        IScoresAndStatsInterface scoresAndStatsInterface;

        public SoundEffect buzzer;
        public SoundEffect dopeAssBombTrack;
        SoundEffectInstance dopeAssBombTrackInstance;

        public ScreenManager(Game game) : base(game) {
        }

        public override void Initialize() {
            contentMgr = new ContentManager(this.Game.Services, "Content");

            startAndEndScreen = new StartAndEndScreen(this) {
                isActive = true
            };
            bombScreen = new BombScreen(this) { 
                isActive = true 
            };
            scoresAndStatsScreen = new ScoresAndStats(this) {
                isActive = true
            };
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

            allGames.Add(downFeathersScreen);
            allGames.Add(countTakedownScreen);
            allGames.Add(longWayDownScreen);
            allGames.Add(wordCountScreen);

            foreach (GameScreen screen in screens) {
                screen.Initialize();
            }

            bombService = Game.Services.GetService(typeof(IBombScreenService)) as IBombScreenService;
            scoresAndStatsInterface = Game.Services.GetService(typeof(IScoresAndStatsInterface)) as IScoresAndStatsInterface;

            base.Initialize();
        }

        protected override void LoadContent() {
            ContentManager content = Game.Content;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = content.Load<SpriteFont>("PressStart2P");
            cursedTimerFont = content.Load<SpriteFont>("CursedTimer");
            cursedTimerSmallFont = content.Load<SpriteFont>("CursedTimerSmall");
            cursedTimer12ptFont = content.Load<SpriteFont>("CursedTimer12pt");
            blankTexture = new Texture2D(GraphicsDevice, 1, 1);
            blankTexture.SetData(new[] { Color.White.PackedValue });

            buzzer = contentMgr.Load<SoundEffect>("sounds/yusuf_sfx-wrong-buzzer-double-491796");
            dopeAssBombTrack = contentMgr.Load<SoundEffect>("sounds/DopeAssBombTrack");
            dopeAssBombTrackInstance = dopeAssBombTrack.CreateInstance();
            dopeAssBombTrackInstance.IsLooped = true;
            dopeAssBombTrackInstance.Volume = 0.03f;
            dopeAssBombTrackInstance.Play();

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

            // Utilities.DebugString(this, debugString, Vector2.One);
        }

        // string debugString = "";
        public void BombExploded() {
            gameIsActive = false;
            currentGameScreen.isActive = false;
            startAndEndScreen.isActive = true;
            startAndEndScreen.isEndScreen = true;
            startAndEndScreen.bombWasDefused = false;

            scoresAndStatsInterface.GameFailed(GameDetails.FindGameWithID(currentGameScreen.id));
        }

        public void BombDefused() {
            gameIsActive = false;
            currentGameScreen.isActive = false;
            startAndEndScreen.isActive = true;
            startAndEndScreen.isEndScreen = true;
            startAndEndScreen.bombWasDefused = true;

            scoresAndStatsInterface.BombDefused();
        }


        // Called when a game has completed. Time to move on.
        internal void GameHasFinished(GameScreen screen) {
            if (screen == startAndEndScreen) {
                // if (startAndEndScreen.isEndScreen) {
                    
                // }
                // else {
                if (startAndEndScreen.isEndScreen) {
                    foreach (GameScreen s in screens) {
                        s.Reset();
                    }
                }
                

                    // Starting a new game
                    bombService.GameStarted();
                    gameIsActive = true;
                    startAndEndScreen.isActive = false;
                    WhichGamesToPlay();
                    currentGameScreen = GetNextGame();
                    currentGameScreen.isActive = true;

                    scoresAndStatsInterface.GameStarted(GameDetails.FindGameWithID(currentGameScreen.id));
                // }
            }
            else if (screen == disarmTheBombScreen) {
                // Just a placeholder for a real end-game
                gameIsActive = false;
                currentGameScreen.isActive = false;
                startAndEndScreen.isActive = true;
                startAndEndScreen.isEndScreen = true;
            }
            else {
                bombService.GameCompleted();
                currentGameScreen.isActive = false;
                currentGameScreen = GetNextGame();
                currentGameScreen.isActive = true;

                scoresAndStatsInterface.GameStarted(GameDetails.FindGameWithID(currentGameScreen.id));
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

            gamesToPlay.AddRange(allGames);
            Random.Shared.Shuffle(CollectionsMarshal.AsSpan(gamesToPlay));

            // gamesToPlay.Add(disarmTheBombScreen);
        }

        internal void ToggleBackgroundMusic() {
            if (dopeAssBombTrackInstance.State == SoundState.Playing) {
                dopeAssBombTrackInstance.Stop();
            }
            else {
                dopeAssBombTrackInstance.Play();
            }
        }

    }
}
