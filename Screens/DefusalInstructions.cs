using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;


namespace GameJam2026 {

    class DefusalInstructions : GameScreen, IDefusalInstructionsService {
        Texture2D door;
        Texture2D[] doors = new Texture2D[4];
        List<int> doorsToReveal = new List<int>();

        List<Texture2D> door1Textures = new List<Texture2D>();
        List<Texture2D> door2Textures = new List<Texture2D>();
        List<Texture2D> door3Textures = new List<Texture2D>();
        List<Texture2D> door4Textures = new List<Texture2D>();

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
            for (int x=0; x<4; x++) {
                Texture2D aNumber = screenManager.contentMgr.Load<Texture2D>("hint_" + (x+1));

                doors[x] = CompositeTextures(door, aNumber);
            }
        }

        public override void Unload() { }

        public override void Reset() { 
            doorsToReveal.Clear();
            door1Textures.Clear();
            door2Textures.Clear();
            door3Textures.Clear();
            door4Textures.Clear();
        }


        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch sb = screenManager.spriteBatch;
            screenManager.GraphicsDevice.Viewport = viewport;

            int xOffset = 0;
            int shift = doors[0].Width;

            sb.Begin(samplerState: SamplerState.PointClamp);
            // sb.Draw(screenManager.blankTexture, new Rectangle(Point.Zero, viewport.Bounds.Size), Color.Purple);
            for (int x=0; x<4; x++) {
                if (doorsToReveal.Contains(x)) {
                    var newOffset = new Vector2(xOffset, 0);
                    if (x == 0) { DrawHintForDoor(sb, door1Textures, new Rectangle(xOffset, 0, doors[0].Width, doors[0].Height)); }
                    if (x == 1) { DrawHintForDoor(sb, door2Textures, new Rectangle(xOffset, 0, doors[0].Width, doors[0].Height)); }
                    if (x == 2) { DrawHintForDoor(sb, door3Textures, new Rectangle(xOffset, 0, doors[0].Width, doors[0].Height)); }
                    if (x == 3) { DrawHintForDoor(sb, door4Textures, new Rectangle(xOffset, 0, doors[0].Width, doors[0].Height)); }

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

        private void DrawHintForDoor(SpriteBatch sb, List<Texture2D> hints, Rectangle rect) {
            // We will, at most, have two hints.
            if (hints.Count == 1) {
                sb.Draw(hints[0], rect, Color.White);
            }
            else {
                var left = new Rectangle(rect.X, rect.Y, rect.Width / 2, rect.Height);
                var right = new Rectangle(rect.X + rect.Width / 2, rect.Y, rect.Width - rect.Width / 2, rect.Height);

                // This mess is here in case we need to split things differently. Mostly just notes for Future Myself.
                // var top = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height / 2);
                // var bottom = new Rectangle(rect.X, rect.Y + rect.Height / 2, rect.Width, rect.Height - rect.Height / 2)

                // int centeredX = targetRect.X + (targetRect.Width - drawWidth) / 2;
                // int centeredY = targetRect.Y + (targetRect.Height - drawHeight) / 2;

                sb.Draw(hints[0], left, Color.White);
                sb.Draw(hints[1], right, Color.White);
            }
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

        public void SetDefuseScenario(BombScenario successScenario, BombScenario setupScenario) {
            List<Texture2D> hintsToShow = new List<Texture2D>();

            if (setupScenario.keypadString != successScenario.keypadString) {
                string filename = "hints/keypad/" + successScenario.keypadString;
                Texture2D texture = screenManager.contentMgr.Load<Texture2D>(filename);
                hintsToShow.Add(texture);
            }
            if (CheckIfArraysAreEqual(setupScenario.fusesBroken, successScenario.fusesBroken) == false) {
                string filename = "hints/fuses/" + ArrayToFilename(successScenario.fusesBroken);
                Texture2D texture = screenManager.contentMgr.Load<Texture2D>(filename);
                hintsToShow.Add(texture);
            }
            if (CheckIfArraysAreEqual(setupScenario.switchesFlipped, successScenario.switchesFlipped) == false) {
                string filename = "hints/toggles/" + ArrayToFilename(successScenario.switchesFlipped);
                Texture2D texture = screenManager.contentMgr.Load<Texture2D>(filename);
                hintsToShow.Add(texture);
            }
            if (setupScenario.stoplightTextureIdx != successScenario.stoplightTextureIdx) {
                string filename = $"hints/stoplights/{successScenario.stoplightTextureIdx}";
                Texture2D texture = screenManager.contentMgr.Load<Texture2D>(filename);
                hintsToShow.Add(texture);
            }
            if (setupScenario.twistKnobTurned != successScenario.twistKnobTurned) {
                string filename = $"hints/twist/{successScenario.twistKnobTurned}";
                Texture2D texture = screenManager.contentMgr.Load<Texture2D>(filename);
                hintsToShow.Add(texture);
            }
            if (setupScenario.pressTexture != successScenario.pressTexture) {
                string filename = $"hints/dont_press/{successScenario.pressTexture}";
                Texture2D texture = screenManager.contentMgr.Load<Texture2D>(filename);
                hintsToShow.Add(texture);
            }
            if (setupScenario.sliderPosition != successScenario.sliderPosition) {
                string filename = $"hints/slider/" + (int)successScenario.sliderPosition;
                Texture2D texture = screenManager.contentMgr.Load<Texture2D>(filename);
                hintsToShow.Add(texture);
            }

            if (hintsToShow.Count < 4) {
                throw new System.Exception($"SuccessScenario {successScenario.id} provided less than four hints in DefusalInstructions.SetDefuseScenario");
            }

            Random.Shared.Shuffle(CollectionsMarshal.AsSpan(hintsToShow));
            if (hintsToShow.Count == 4) {
                door1Textures.Add(hintsToShow[0]);
                door2Textures.Add(hintsToShow[1]);
                door3Textures.Add(hintsToShow[2]);
                door4Textures.Add(hintsToShow[3]);
            }
            else if (hintsToShow.Count == 5) {
                door1Textures.Add(hintsToShow[0]);
                door2Textures.Add(hintsToShow[1]);

                door3Textures.Add(hintsToShow[2]);
                door3Textures.Add(hintsToShow[3]);

                door4Textures.Add(hintsToShow[4]);
            }
            else if (hintsToShow.Count == 6) {
                door1Textures.Add(hintsToShow[0]);

                door2Textures.Add(hintsToShow[1]);
                door2Textures.Add(hintsToShow[2]);

                door3Textures.Add(hintsToShow[3]);
                door3Textures.Add(hintsToShow[4]);

                door4Textures.Add(hintsToShow[5]);
            }
            else {
                door1Textures.Add(hintsToShow[0]);

                door2Textures.Add(hintsToShow[1]);
                door2Textures.Add(hintsToShow[2]);

                door3Textures.Add(hintsToShow[3]);
                door3Textures.Add(hintsToShow[4]);

                door4Textures.Add(hintsToShow[5]);
                door4Textures.Add(hintsToShow[6]);
            }
        }

        private string ArrayToFilename(bool[] a) {
            string result = "";
            for (int x=0; x<a.Length; x++) {
                result += a[x] ? "1" : "0";
            }
            return result;
        }

        private bool CheckIfArraysAreEqual(bool[] a, bool[] b) {
            for (int x=0; x<a.Length; x++) {
                if (a[x] != b[x]) {
                    return false;
                }
            }
            return true;
        }

    }

    public interface IDefusalInstructionsService {
        // This is 1-4, for caller simplicity
        // Doors are indexed 0-3
        void RevealDoor(int doorNumber);
        void SetDefuseScenario(BombScenario successScenario, BombScenario setupScenario);
    }
}

