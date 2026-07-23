using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameJam2026 {
    public class ScreenManager : DrawableGameComponent {

        private readonly List<GameScreen> screens = new List<GameScreen>();
        private readonly List<GameScreen> tempScreensList = new List<GameScreen>();

        private readonly InputState input = new InputState();
        public SpriteBatch spriteBatch { get; private set; }
        public SpriteFont font { get; private set; }
        public Texture2D blankTexture { get; private set; }

        public readonly int bombPanelSize = 400;
        public readonly int defusalPanelSize = 150;

        Viewport viewport;

        BombScreen bombScreen;
        ScoresAndStats scoresAndStatsScreen;
        DownFeathers downFeathersScreen;
        DefusalInstructions defusalInstructionsScreen;

        public ScreenManager(Game game) : base(game) {
        }

        // public void AddScreen(GameScreen gameScreen) {
        //     screens.Add(gameScreen);
        // }

        public override void Initialize() {
            bombScreen = new BombScreen(this);
            scoresAndStatsScreen = new ScoresAndStats(this);
            downFeathersScreen = new DownFeathers(this);
            defusalInstructionsScreen = new DefusalInstructions(this);

            base.Initialize();
        }

        protected override void LoadContent() {
            ContentManager content = Game.Content;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            // font = content.Load<SpriteFont>("menuFont");
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

            // For each screen we need to update it. 
            // Since our screens are going to be composed, we need to ensure the subscreens are
            // appropriately marked as who's getting input. And where they can draw.
            // TODO: the bomb screen will need to move to the smaller subscreen to the bigger one
            //       once they finish the run through the minigames. What replaces it? who knows...
        }

        public override void Draw(GameTime gameTime) {
            // foreach (GameScreen screen in screens) {
            //     screen.Draw(gameTime);
            // }

            // Each screen is going to modify the viewport, so we stash the original here and restore it once they're done.
            viewport = GraphicsDevice.Viewport;

            // We draw our screens in a very specific order. And we're going to have a fixed number.
            // 1. Draw the current minigame
            downFeathersScreen.Draw(gameTime);
            
            // 2. Draw the score/state panel
            scoresAndStatsScreen.Draw(gameTime);

            // // 3. Draw the defusal panel
            defusalInstructionsScreen.Draw(gameTime);

            // // 4. Draw the bomb panel
            bombScreen.Draw(gameTime);

            GraphicsDevice.Viewport = viewport;
        }

    }
}