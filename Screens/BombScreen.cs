using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using nkast.Wasm.Canvas;

namespace GameJam2026 {
    class BombScreen : GameScreen, IBombScreenService {

        Texture2D blue_broken;
        Texture2D blue;
        Texture2D bomb;
        Texture2D dont_press;
        Texture2D green_broken;
        Texture2D green;
        Texture2D orange_broken;
        Texture2D orange;
        Texture2D press;
        Texture2D red_broken;
        Texture2D red;
        Texture2D slider_knob;
        Texture2D stoplight_green;
        Texture2D stoplight_off;
        Texture2D stoplight_red;
        Texture2D stoplight_yellow;
        Texture2D toggle_switch;
        Texture2D twist_knob;
        Texture2D yellow_broken;
        Texture2D yellow;

        int gamesCompleted;
        bool gameIsActive;
        IDefusalInstructionsService defusalService;
        float startingBombClockTime = 60f; // seconds
        float bombClock; // running clock when the game is active
        SoundEffect tickingClock;
        SoundEffectInstance tickingClockInstance;
        List<Button> allClickableThings = new List<Button>();
        string debugString = "";

        Rectangle[] keypadButtons = new Rectangle[10];
        Rectangle[] fuseButtons = new Rectangle[5];
        Rectangle[] sliderKnob = new Rectangle[2];
        Rectangle[] switches = new Rectangle[4];
        Rectangle twistKnob = new Rectangle();
        Rectangle[] stoplights = new Rectangle[2];
        Rectangle dontPressButton = new Rectangle();
        Rectangle[] displays = new Rectangle[2];

        Vector2 toggleSwitchPivotPoint = new Vector2(19, 19);

        private enum FuseColor {
            Red,
            Green,
            Blue,
            Yellow,
            Orange,
            Last
        }

        private enum SliderKnob {
            Up,
            Down,
            Last
        }

        private enum Switches {
            One,
            Two,
            Three,
            Four,
            Last
        }

        private enum Stoplight {
            Button,
            Lights
        }

        private enum Displays {
            Clock,
            Keypad
        }

        private enum OtherButtons {
            TwistKnob,
            Stoplight,
            DontPress
        }



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
            blue_broken = screenManager.contentMgr.Load<Texture2D>("bomb/blue_broken");
            blue = screenManager.contentMgr.Load<Texture2D>("bomb/blue");
            bomb = screenManager.contentMgr.Load<Texture2D>("bomb/bomb");
            dont_press = screenManager.contentMgr.Load<Texture2D>("bomb/dont_press");
            green_broken = screenManager.contentMgr.Load<Texture2D>("bomb/green_broken");
            green = screenManager.contentMgr.Load<Texture2D>("bomb/green");
            orange_broken = screenManager.contentMgr.Load<Texture2D>("bomb/orange_broken");
            orange = screenManager.contentMgr.Load<Texture2D>("bomb/orange");
            press = screenManager.contentMgr.Load<Texture2D>("bomb/press");
            red_broken = screenManager.contentMgr.Load<Texture2D>("bomb/red_broken");
            red = screenManager.contentMgr.Load<Texture2D>("bomb/red");
            slider_knob = screenManager.contentMgr.Load<Texture2D>("bomb/slider_knob");
            stoplight_green = screenManager.contentMgr.Load<Texture2D>("bomb/stoplight_green");
            stoplight_off = screenManager.contentMgr.Load<Texture2D>("bomb/stoplight_off");
            stoplight_red = screenManager.contentMgr.Load<Texture2D>("bomb/stoplight_red");
            stoplight_yellow = screenManager.contentMgr.Load<Texture2D>("bomb/stoplight_yellow");
            toggle_switch = screenManager.contentMgr.Load<Texture2D>("bomb/toggle_switch");
            twist_knob = screenManager.contentMgr.Load<Texture2D>("bomb/twist_knob");
            yellow_broken = screenManager.contentMgr.Load<Texture2D>("bomb/yellow_broken");
            yellow = screenManager.contentMgr.Load<Texture2D>("bomb/yellow");

            tickingClock = screenManager.contentMgr.Load<SoundEffect>("sounds/dragon-studio-clock-ticking-sfx-467486-edited");
            tickingClockInstance = tickingClock.CreateInstance();
            tickingClockInstance.IsLooped = true;
            tickingClockInstance.Volume = 0.1f;

            defusalService = screenManager.Game.Services.GetService(typeof(IDefusalInstructionsService)) as IDefusalInstructionsService;

            keypadButtons[0] = new Rectangle(104, 347, 45, 29);
            keypadButtons[1] = new Rectangle(30, 243, 54, 27);
            keypadButtons[2] = new Rectangle(100, 243, 44, 27);
            keypadButtons[3] = new Rectangle(154, 243, 26, 27);
            keypadButtons[4] = new Rectangle(30, 277, 54, 26);
            keypadButtons[5] = new Rectangle(100, 277, 44, 26);
            keypadButtons[6] = new Rectangle(162, 277, 50, 26);
            keypadButtons[7] = new Rectangle(30, 311, 54, 29);
            keypadButtons[8] = new Rectangle(100, 311, 44, 29);
            keypadButtons[9] = new Rectangle(154, 311, 46, 29);

            fuseButtons[(int)FuseColor.Red]    = new Rectangle(274, 237, 68, 19);
            fuseButtons[(int)FuseColor.Green]  = new Rectangle(275, 257, 67, 19);
            fuseButtons[(int)FuseColor.Blue]   = new Rectangle(274, 279, 67, 17);
            fuseButtons[(int)FuseColor.Yellow] = new Rectangle(273, 299, 68, 18);
            fuseButtons[(int)FuseColor.Orange] = new Rectangle(273, 322, 66, 18);

            twistKnob = new Rectangle(20, 22, 75, 63);

            sliderKnob[(int)SliderKnob.Up]   = new Rectangle(329, 27, 38, 30);
            sliderKnob[(int)SliderKnob.Down] = new Rectangle(329, 213, 38, 30);

            switches[(int)Switches.One]   = new Rectangle(131, 110, 38, 38);
            switches[(int)Switches.Two]   = new Rectangle(169, 110, 38, 38);
            switches[(int)Switches.Three] = new Rectangle(131, 149, 38, 38);
            switches[(int)Switches.Four]  = new Rectangle(169, 149, 38, 38);

            stoplights[(int)Stoplight.Button] = new Rectangle(53, 165, 29, 20);
            stoplights[(int)Stoplight.Lights] = new Rectangle(29, 94, 80, 101);

            dontPressButton = new Rectangle(229, 107, 78, 70);
        
            displays[(int)Displays.Clock] = new Rectangle(104, 18, 195, 63);
            displays[(int)Displays.Keypad] = new Rectangle(30, 213, 172, 26);

            for (int x=0; x<10; x++) {
                var button = new Button(keypadButtons[x], x);
                button.Tapped += Keypad_Tapped;
                allClickableThings.Add(button);
            }

            for (int x=0; x<(int)FuseColor.Last; x++) {
                var button = new Button(fuseButtons[x], x);
                button.Tapped += Fuse_Tapped;
                allClickableThings.Add(button);
            }

            var twistKnobButton = new Button(twistKnob, (int)OtherButtons.TwistKnob);
            twistKnobButton.Tapped += OtherButton_Tapped;
            allClickableThings.Add(twistKnobButton);

            for (int x=0; x<(int)SliderKnob.Last; x++) {
                var sliderButton = new Button(sliderKnob[x], x);
                sliderButton.Tapped += Slider_Tapped;
                allClickableThings.Add(sliderButton);
            }

            for (int x=0; x<(int)Switches.Last; x++) {
                var switchButton = new Button(switches[x], x);
                switchButton.Tapped += Switch_Tapped;
                allClickableThings.Add(switchButton);
            }

            var stoplightButton = new Button(stoplights[(int)Stoplight.Button], (int)OtherButtons.Stoplight);
            stoplightButton.Tapped += OtherButton_Tapped;
            allClickableThings.Add(stoplightButton);

            var dontPress = new Button(dontPressButton, (int)OtherButtons.DontPress);
            dontPress.Tapped += OtherButton_Tapped;
            allClickableThings.Add(dontPress);


        }

        public override void HandleInput(GameTime gameTime, InputState input) {
            if (!isActive) { return; }

            if (input.isNewLeftMouseDown()) {
                foreach (Button b in allClickableThings) {
                    b.HandleTap(input.translatedMousePosition(viewport).ToVector2());
                }
            }
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
            var clockRect = displays[(int)Displays.Clock];
            string displayString = bombClock.ToString("00.00");
            var stringSize = screenManager.cursedTimerFont.MeasureString(displayString);
            sb.Draw(screenManager.blankTexture, clockRect, Color.Black);
            int smallYAxisFontAdjustment = 5;
            Color textColor = Color.Green;
            if (bombClock < 10f) { textColor = Color.Red; }
            else if (bombClock < 30f) { textColor = Color.Yellow; }
            sb.DrawString(screenManager.cursedTimerFont, displayString, 
                            new Vector2(clockRect.Left + clockRect.Width / 2 - stringSize.X / 2, 
                                        clockRect.Top + clockRect.Height / 2 - stringSize.Y / 2 + smallYAxisFontAdjustment), 
                            textColor);

            // Draw the keypad screen

            // Now the bomb. Everything else is on top
            sb.Draw(bomb, Point.Zero.ToVector2(), Color.White);

            // Draw the twist knob
            sb.Draw(twist_knob, twistKnob, Color.White);

            // Draw fuses
            for (int x=0; x<(int)FuseColor.Last; x++) {
                var fuse = drawForFuse((FuseColor)x);
                sb.Draw(fuse.t, fuse.r, Color.White);
            }

            // Draw Stoplights
            sb.Draw(stoplight_off, stoplights[(int)Stoplight.Lights], Color.White);

            // Draw Don't Press button
            sb.Draw(dont_press, dontPressButton, Color.White);

            // Draw slider knob
            sb.Draw(slider_knob, sliderKnob[(int)SliderKnob.Up], Color.White);

            // Draw switches
            for (int x=0; x<(int)Switches.Last; x++) {
                sb.Draw(toggle_switch, switches[x], Color.White);
            }

            sb.End();

            if (debugString.Length > 0) {
                Utilities.DebugString(screenManager, debugString, Vector2.One);
            }
            base.Draw(gameTime);
        }

        void Keypad_Tapped(object sender, EventArgs e) {
            debugString = $"{(sender as Button).intValue} tapped";
        }

        void Fuse_Tapped(object sender, EventArgs e) {
            debugString = $"fuse {(sender as Button).intValue} tapped";
        }

        void Slider_Tapped(object sender, EventArgs e) {
            debugString = $"slider {(sender as Button).intValue} tapped";
        }

        void Switch_Tapped(object sender, EventArgs e) {
            debugString = $"switch {(sender as Button).intValue} tapped";
        }

        void OtherButton_Tapped(object sender, EventArgs e) {
            debugString = $"other {(sender as Button).intValue} tapped";
        }

        private (Texture2D t, Rectangle r) drawForFuse(FuseColor c) {
            Texture2D texture;
            Rectangle rectangle = fuseButtons[(int)c];

            switch (c) {
                case FuseColor.Red:
                    texture = red;
                    break;
                case FuseColor.Green:
                    texture = green;
                    break;
                case FuseColor.Blue:
                    texture = blue;
                    break;
                case FuseColor.Yellow:
                    texture = yellow;
                    break;
                case FuseColor.Orange:
                    texture = orange;
                    break;
                default:
                    texture = red;
                    break;
            }
            return (texture, rectangle);
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