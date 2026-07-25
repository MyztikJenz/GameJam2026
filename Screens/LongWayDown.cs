using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameJam2026 {

    class Crate : SimpleTracker {
        public string value;

        public Crate(Texture2D t, Vector2 initialPosition, Vector2 moveTo, string value) : base(t, initialPosition, moveTo) {
            this.value = value;
        }
    }


    class LongWayDown : GameScreen {
        Vector2 playerPosition;
        Texture2D dude;
        Texture2D dudeLanded;
        Texture2D crate;
        Texture2D background;
        Texture2D grass;
        TitleString titleString;
        List<Crate> activeCrates = null;
        List<string> equation = new List<string>();
        private Random random = new Random();
        Crates nextCrateType = Crates.numeric;
        bool displayQuestion = false;
        SimpleTracker playerTracker;
        SimpleTracker grassTracker;
        bool touchedCrate = false;
        float blinkTimer;
        Rectangle blinkRect;
        string typedAnswer = "";
        float typedAnswerTimer;
        int answer;
        string completedEquation;

        private enum Crates {
            numeric,
            symbols
        }

        public LongWayDown(ScreenManager manager) : base(manager) {
            instructions = "Move: <- ->, Type: numbers, then Enter";
        }

        public override void Load() {
            dude = screenManager.contentMgr.Load<Texture2D>("fall_down_guy");
            dudeLanded = screenManager.contentMgr.Load<Texture2D>("fall_down_landed");
            crate = screenManager.contentMgr.Load<Texture2D>("crate");
            grass = screenManager.contentMgr.Load<Texture2D>("grass");
            background = Utilities.CreateLinearGradient(screenManager.GraphicsDevice, viewport.Bounds.Width, viewport.Bounds.Height, Color.DarkSlateBlue, Color.LightSteelBlue);
            titleString = new TitleString("Long Way Down!", Color.Yellow, Color.Gray);
            playerPosition = new Vector2(viewport.Bounds.Width / 2, 25);
            blinkRect = new Rectangle(viewport.Bounds.Right - 250, viewport.Bounds.Bottom - 40, 100, 10);

            base.Load();
        }

        public override void Update(GameTime gameTime) {
            titleString.Update(gameTime);

            if (displayQuestion) {
                playerTracker.Update(gameTime);
                grassTracker.Update(gameTime);

                blinkTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (typedAnswerTimer > 0f) {
                    typedAnswerTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (typedAnswerTimer <= 0) {
                        typedAnswer = "";
                    }
                }

            }
            else {
                if (activeCrates == null || activeCrates.Count == 0) {
                    touchedCrate = false;
                    activeCrates = CreateCrates(nextCrateType, 5);
                    nextCrateType = nextCrateType == Crates.numeric ? Crates.symbols : Crates.numeric;
                }

                List<Crate> cratesToRemove = new List<Crate>();
                Rectangle dudeRect = new Rectangle(playerPosition.ToPoint(), dude.Bounds.Size);
                foreach (Crate crate in activeCrates) {
                    if (crate.Intersects(dudeRect) && !touchedCrate) {
                        touchedCrate = true;
                        cratesToRemove.Add(crate);
                        equation.Add(crate.value);

                        if (equation.Count == 5) {
                            displayQuestion = true;
                            var moveTo = new Vector2(90, viewport.Bounds.Height - dude.Height - (dude.Height - dudeLanded.Height));
                            playerTracker = new SimpleTracker(dude, playerPosition, moveTo);
                            playerTracker.SetSpeed(250);

                            moveTo = new Vector2(0, viewport.Bounds.Bottom - grass.Height);
                            grassTracker = new SimpleTracker(grass, new Vector2(0, viewport.Bounds.Bottom + grass.Height), moveTo);
                            grassTracker.SetSpeed(250);

                            // There's only ever one pattern here: # +/- # +/- #
                            foreach (string s in equation) {
                                completedEquation += $"{s} ";
                            }
                            answer = (int)new DataTable().Compute(completedEquation, null);
                        }
                    }
                    else if (crate.HasArrived()) {
                        cratesToRemove.Add(crate);
                    }
                    else {
                        crate.Update(gameTime);
                    }
                }
                
                foreach (Crate crate in cratesToRemove) {
                    activeCrates.Remove(crate);
                }

                if (touchedCrate) {
                    // Zoom them out of here!
                    foreach (Crate crate in activeCrates) {
                        crate.SetSpeed(500);
                        crate.Update(gameTime); // Second update gets them bookin' immediately
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void HandleInput(GameTime gameTime, InputState input) {

            if (displayQuestion) {
                if (typedAnswerTimer > 0) { return; } // to prevent typing while we show the wrong answer

                int? numPressed = input.isNumberKeyPressed();
                if (input.isNewKeyPress(Keys.OemMinus)) {
                    typedAnswer += "-";
                }
                if (numPressed.HasValue) {
                    typedAnswer += numPressed.Value;
                }
                if (input.isNewKeyPress(Keys.Enter)) {
                    typedAnswerTimer = 1f;
                    if (int.TryParse(typedAnswer, out int result)) {
                        if (result == answer) {
                            screenManager.GameHasFinished(this);
                        }
                        else {
                            screenManager.buzzer.Play();
                        }
                    }
                    else {
                        screenManager.buzzer.Play();
                    }
                }
            }
            else {
                int playerIdx = (int)PlayerIndex.One;
                KeyboardState kState = input.currentKeyboardStates[playerIdx];

                Vector2 movement = Vector2.Zero;
                if (kState.IsKeyDown(Keys.Left) || kState.IsKeyDown(Keys.A)) {
                    movement.X--;
                }
                if (kState.IsKeyDown(Keys.Right) || kState.IsKeyDown(Keys.D)) {
                    movement.X++;
                }

                if (movement.Length() > 1) {
                    movement.Normalize();
                }

                playerPosition += movement * 8f;

                if (playerPosition.X < 0) {
                    playerPosition.X = 0;
                }
                if (playerPosition.X > viewport.Bounds.Width - dude.Width) {
                    playerPosition.X = viewport.Bounds.Width - dude.Width;
                }
            }
        }


        public override void Draw(GameTime gameTime) {
            screenManager.GraphicsDevice.Viewport = viewport;
            SpriteBatch sb = screenManager.spriteBatch;
            sb.Begin();
            sb.Draw(background, Vector2.Zero, Color.White);

            if (displayQuestion) {
                var destRect = new Rectangle((int)grassTracker.position.X, 
                                             (int)grassTracker.position.Y, 
                                             viewport.Bounds.Width, grassTracker.texture.Height);
                sb.Draw(grassTracker.texture, destRect, Color.White);
                if (playerTracker.HasArrived()) {
                    // sb.Draw(dudeLanded, playerTracker.position, Color.White);
                    sb.Draw(dudeLanded, new Vector2(80, viewport.Bounds.Height - 136 - dudeLanded.Height / 2), Color.White);
                    sb.End();
                    sb.Begin(samplerState: SamplerState.PointClamp);
                    sb.DrawString(screenManager.font, completedEquation + "=", new Vector2(400, viewport.Bounds.Bottom - 100), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);

                    if ((int)blinkTimer % 2 == 0) {
                        sb.Draw(screenManager.blankTexture, blinkRect, Color.Black);
                    }

                    if (typedAnswer.Length > 0) {
                        Vector2 textSize = screenManager.font.MeasureString(typedAnswer);
                        var answerRect = blinkRect;
                        // answerRect.Location = new Point(answerRect.Center.X - (int)(textSize.X / 2.0), answerRect.Location.Y - (int)textSize.Y);
                        answerRect.Location = new Point(answerRect.Center.X - (int)textSize.X, answerRect.Location.Y - (int)textSize.Y * 2);
                        
                        sb.DrawString(screenManager.font, typedAnswer, answerRect.Location.ToVector2(), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                    }
                }
                else {
                    sb.Draw(playerTracker.texture, playerTracker.position, Color.White);
                }
                sb.End();           
            }
            else {
                foreach (Crate crate in activeCrates) {
                    sb.Draw(crate.texture, crate.position, Color.White);
                }

                sb.Draw(dude, playerPosition, Color.White);
                sb.End();

                titleString.Draw(screenManager, gameTime);
            }

            base.Draw(gameTime);
        }

        private List<Crate> CreateCrates(Crates type, int count) {
            var newCrates = new List<Crate>();

            // Crates always start offscreen at the bottom and end up offscreen at the top
            // count + 1 so that we get "count" number of offsets that aren't margin-aligned
            int offset = viewport.Bounds.Width / (count + 1);
            int xOffset = offset - crate.Width / 2;

            var overlays = new List<string>();

            switch (type) {
                case Crates.numeric:
                    for (int x=0; x<count; x++) {
                        overlays.Add(random.Next(5, 25).ToString());
                    }
                    break;

                case Crates.symbols:
                    // string[] symbols = ["+", "-", "*", "/"];
                    string[] symbols = ["+", "-"];

                    for (int x=0; x<count; x++) {
                        overlays.Add(symbols[random.Next(0,symbols.Length)]);
                    }
                    break;

                default:
                    break;
            }

            for (int x=0; x<count; x++) {
                var startPos = new Vector2(xOffset, viewport.Bounds.Height - crate.Height);
                var endPos = new Vector2(xOffset, -crate.Height);

                // var c = new SimpleTracker(crate, startPos, endPos);
                var c = new Crate(CompositeCrate(crate, overlays[x]), startPos, endPos, overlays[x]);
                c.ySpeed = 300;
                c.xSpeed = 0;
                newCrates.Add(c);

                xOffset += offset;
            }

            return newCrates;
        }

        private Texture2D CompositeCrate(Texture2D crate, string overlay) {
            GraphicsDevice gd = screenManager.GraphicsDevice;
            SpriteBatch sb = screenManager.spriteBatch;
            RenderTarget2D renderTarget = new RenderTarget2D(gd, crate.Width, crate.Height, false,
                                                             gd.PresentationParameters.BackBufferFormat,
                                                             DepthFormat.Depth24);

            screenManager.GraphicsDevice.SetRenderTarget(renderTarget);
            screenManager.GraphicsDevice.Clear(Color.Red * 0f);

            Vector2 textSize = screenManager.font.MeasureString(overlay);
            Vector2 textPos = new Vector2(crate.Bounds.Center.X, crate.Bounds.Center.Y) - textSize / 2f;

            sb.Begin(samplerState: SamplerState.PointClamp);
            sb.Draw(crate, Vector2.Zero, Color.White);
            sb.DrawString(screenManager.font, overlay, textPos, Color.Black, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            sb.DrawString(screenManager.font, overlay, new Vector2(textPos.X + 3, textPos.Y + 3), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            sb.End();

            gd.SetRenderTarget(null);

            return renderTarget;
        }
    }
}
