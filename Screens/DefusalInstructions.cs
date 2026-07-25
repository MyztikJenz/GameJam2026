using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameJam2026 {

    class DefusalInstructions : GameScreen, IDefusalInstructionsService {
        Texture2D door;
        Texture2D reveal_background;
        Texture2D[] doors = new Texture2D[4];
        List<int> doorsToReveal = new List<int>();


        public DefusalInstructions(ScreenManager manager) : base(manager) { 
            PresentationParameters pp = screenManager.GraphicsDevice.PresentationParameters;
            viewport = new Viewport(0, pp.BackBufferHeight - screenManager.defusalPanelSize,
                                    pp.BackBufferWidth - screenManager.bombPanelSize, screenManager.defusalPanelSize);

        }

        public override void Initialize() {
            screenManager.Game.Services.AddService(typeof(IDefusalInstructionsService), this);
        }

        public override void Load() {
            door = screenManager.contentMgr.Load<Texture2D>("hint_door");
            reveal_background = screenManager.contentMgr.Load<Texture2D>("hint_reveal_background");
            for (int x=0; x<4; x++) {
                Texture2D aNumber = screenManager.contentMgr.Load<Texture2D>("hint_" + (x+1));

                doors[x] = CompositeTextures(door, aNumber);
            }
        }

        public override void Unload() { }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch sb = screenManager.spriteBatch;
            screenManager.GraphicsDevice.Viewport = viewport;

            int xOffset = 0;
            int shift = doors[0].Width;

            sb.Begin();
            // sb.Draw(screenManager.blankTexture, new Rectangle(Point.Zero, viewport.Bounds.Size), Color.Purple);
            for (int x=0; x<4; x++) {
                if (doorsToReveal.Contains(x)) {
                    var newOffset = new Vector2(xOffset, 0);
                    sb.Draw(screenManager.blankTexture, new Rectangle(newOffset.ToPoint(), reveal_background.Bounds.Size), Color.Red);
                    sb.Draw(reveal_background, newOffset, Color.White);
                }
                else {
                    sb.Draw(doors[x], new Vector2(xOffset, 0), Color.White);
                }
                xOffset += shift;
            }

            // Needs a separator, I think.
            sb.Draw(screenManager.blankTexture, new Rectangle(0,0,viewport.Bounds.Width,2), Color.Black);

            sb.End();
            // Utilities.DebugString(screenManager, viewport.Bounds.ToString(), Vector2.One);

            base.Draw(gameTime);
        }


        private Texture2D CompositeTextures(Texture2D firstLayer, Texture2D secondLayer) {
            GraphicsDevice gd = screenManager.GraphicsDevice;
            SpriteBatch sb = screenManager.spriteBatch;
            RenderTarget2D renderTarget = new RenderTarget2D(gd, firstLayer.Width, firstLayer.Height, false,
                                                             gd.PresentationParameters.BackBufferFormat,
                                                             DepthFormat.Depth24);

            screenManager.GraphicsDevice.SetRenderTarget(renderTarget);
            screenManager.GraphicsDevice.Clear(Color.Red * 0f);

            Vector2 secondLayerPos = new Vector2(firstLayer.Bounds.Center.X - secondLayer.Bounds.Width / 2,
                                                 firstLayer.Bounds.Center.Y - secondLayer.Bounds.Height / 2);

            sb.Begin(samplerState: SamplerState.PointClamp);
            sb.Draw(firstLayer, Vector2.Zero, Color.White);
            sb.Draw(secondLayer, secondLayerPos, Color.White);
            sb.End();

            gd.SetRenderTarget(null);

            return renderTarget;
        }

        // IDefusalInstructionsService
        public void RevealDoor(int doorNumber) {
            doorsToReveal.Add(doorNumber - 1);
        }
    }

    public interface IDefusalInstructionsService {
        // This is 1-4, for caller simplicity
        // Doors are indexed 0-3
        void RevealDoor(int doorNumber);
    }
}

