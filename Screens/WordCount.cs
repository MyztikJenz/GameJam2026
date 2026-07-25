using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;


// To see some the possible results, copy the text of words.txt to the pasteboard, then:
// pbpaste | sort | grep -E -w '\w{5}'
// pbpaste | sort | grep ^s
// pbpaste | sort | grep a

namespace GameJam2026 {
    class WordCount : GameScreen {
        List<string> words = new List<string>();
        List<string> gameWords = new List<string>();
        Strategies strategy;
        int answer;
        public SpriteFont chalkFont { get; private set; }
        float blinkTimer;
        Rectangle blinkRect;
        int typedAnswer = 999;
        float typedAnswerTimer;
        TitleString titleString;
        
        int leftColumn = 0;
        int rightColumn = 0;

        SoundEffect buzzer;

        // Count of words with N letters
        // Count of words with [AEIOU]
        // Count of words that start with D
        private enum Strategies {
            WordsWithNLetters,
            WordsContainingA,
            WordsContainingE,
            WordsContainingI,
            WordsContainingO,
            WordsContainingU,
            WordsStartingWithD
        }        

        public WordCount(ScreenManager manager) : base(manager) { 
            id = "8D656081-A9CB-4465-99E6-8B1C01CC0738";
        }

        public override void Load() {
            using Stream stream = TitleContainer.OpenStream("Content/words.txt");
            using StreamReader reader = new StreamReader(stream);

            while (!reader.EndOfStream) {
                string word = reader.ReadLine();
                if (!string.IsNullOrWhiteSpace(word) && !word.StartsWith("#")) {
                    words.Add(word.Trim());
                }
            }

            titleString = new TitleString("Can You Count?!");

            chalkFont = screenManager.contentMgr.Load<SpriteFont>("Chalkduster");
            blinkRect = new Rectangle(viewport.Bounds.Right - 250, viewport.Bounds.Bottom - 40, 100, 10);

            buzzer = screenManager.contentMgr.Load<SoundEffect>("sounds/yusuf_sfx-wrong-buzzer-double-491796");
        }

        public override void Update(GameTime gameTime) {
            if (gameWords.Count == 0) {
                PrepareGame();
            }

            if (leftColumn == 0) {
                leftColumn = viewport.Bounds.Center.X / 2;
                rightColumn = viewport.Bounds.Center.X + leftColumn;
            }

            base.Update(gameTime);

            blinkTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (typedAnswerTimer > 0f) {
                typedAnswerTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public override void HandleInput(GameTime gameTime, InputState input) {
            int playerIdx = (int)PlayerIndex.One;
            KeyboardState kState = input.currentKeyboardStates[playerIdx];

            int? numPressed = input.isNumberKeyPressed();
            if (numPressed.HasValue) {
                typedAnswer = numPressed.Value;
                typedAnswerTimer = 1f;
                if (typedAnswer != answer) {
                    buzzer.Play();
                }
            }

            if (typedAnswer == answer) {
                screenManager.GameHasFinished(this);
            }
        }

        public override void Draw(GameTime gameTime) {
            screenManager.GraphicsDevice.Viewport = viewport;
            screenManager.GraphicsDevice.Clear(new Color(39, 76, 67));

            SpriteBatch sb = screenManager.spriteBatch;
            sb.Begin(samplerState: SamplerState.PointClamp);


            int yOffset = 10;
            for (int x=0; x<9; x++) {
                string word = gameWords[x];
                int column = (x % 2 == 0) ? rightColumn : leftColumn;
                if (x == 8) {
                    column = viewport.Bounds.Center.X;
                }
                Vector2 textSize = chalkFont.MeasureString(word);
                Vector2 location = new Vector2(column - textSize.X / 2, yOffset);
                sb.DrawString(chalkFont, word, location, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                if (x>0 && x % 2 != 0) {
                    yOffset += (int)textSize.Y + 10;
                }
            }

            if ((int)blinkTimer % 2 == 0) {
                sb.Draw(screenManager.blankTexture, blinkRect, Color.Black);
            }

            if (typedAnswerTimer > 0) {
                Vector2 textSize = chalkFont.MeasureString(typedAnswer.ToString());
                var answerRect = blinkRect;
                answerRect.Location = new Point(answerRect.Center.X - (int)(textSize.X / 2.0), answerRect.Location.Y - (int)textSize.Y);
                
                sb.DrawString(chalkFont, typedAnswer.ToString(), answerRect.Location.ToVector2(), Color.White);
            }
            sb.End();

            base.Draw(gameTime);
        }

        private void PrepareGame() {
            Strategies[] values = (Strategies[])Enum.GetValues(typeof(Strategies));
            strategy = values[Random.Shared.Next(values.Length)];

            string[] randomWords = Random.Shared.GetItems(words.ToArray(), 9);
            gameWords.AddRange(randomWords);

            switch (strategy) {
                case Strategies.WordsWithNLetters:
                    int tgtCount = Random.Shared.Next(3, 6); // 3, 4, or 5
                    answer = gameWords.FindAll(s => s.Length == tgtCount).Count();
                    instructions = $"How many words have {tgtCount} letters?";
                    break;
                case Strategies.WordsContainingA:
                    answer = gameWords.FindAll(s => s.Contains('a')).Count();
                    instructions = "How many words contain an 'a'?";
                    break;
                case Strategies.WordsContainingE:
                    answer = gameWords.FindAll(s => s.Contains('e')).Count();
                    instructions = "How many words contain an 'e'?";
                    break;
                case Strategies.WordsContainingI:
                    answer = gameWords.FindAll(s => s.Contains('i')).Count();
                    instructions = "How many words contain an 'i'?";
                    break;
                case Strategies.WordsContainingO:
                    answer = gameWords.FindAll(s => s.Contains('o')).Count();
                    instructions = "How many words contain an 'o'?";
                    break;
                case Strategies.WordsContainingU:
                    answer = gameWords.FindAll(s => s.Contains('u')).Count();
                    instructions = "How many words contain a 'u'?";
                    break;
                case Strategies.WordsStartingWithD:
                    answer = gameWords.FindAll(s => s.StartsWith('d')).Count();
                    instructions = "How many words start with 'd'?";
                    break;
                default:
                    instructions = "uh oh... the answer is 0";
                    answer = 0;
                    break;
            }
        }
    }
}
