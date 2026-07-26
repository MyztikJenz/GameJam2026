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
        Texture2D green_broken;
        Texture2D green;
        Texture2D orange_broken;
        Texture2D orange;
        Texture2D red_broken;
        Texture2D red;
        Texture2D slider_knob;
        Texture2D toggle_switch;
        Texture2D twist_knob;
        Texture2D yellow_broken;
        Texture2D yellow;

        Texture2D[] stoplightTexs = new Texture2D[4];
        Texture2D[] pressTexs = new Texture2D[2];

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

        SoundEffect beepSfx;
        SoundEffect crankSfx;
        SoundEffect clickSfx;
        SoundEffect glassBreakSfx;
        SoundEffectInstance glassBreakSfxInstance;
        SoundEffect explosionSfx;
        SoundEffect clappingSfx;

        BombScenario defuseScenario;

        Button debugButton;

        private enum FuseColor {
            Red,
            Green,
            Blue,
            Yellow,
            Orange,
            Last
        }

        internal enum SliderKnob {
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

        internal enum StoplightTextures {
            Off,
            Red,
            Yellow,
            Green,
            Last
        }

        internal enum PressTextures {
            Press,
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
            green_broken = screenManager.contentMgr.Load<Texture2D>("bomb/green_broken");
            green = screenManager.contentMgr.Load<Texture2D>("bomb/green");
            orange_broken = screenManager.contentMgr.Load<Texture2D>("bomb/orange_broken");
            orange = screenManager.contentMgr.Load<Texture2D>("bomb/orange");
            red_broken = screenManager.contentMgr.Load<Texture2D>("bomb/red_broken");
            red = screenManager.contentMgr.Load<Texture2D>("bomb/red");
            slider_knob = screenManager.contentMgr.Load<Texture2D>("bomb/slider_knob");
            toggle_switch = screenManager.contentMgr.Load<Texture2D>("bomb/toggle_switch");
            twist_knob = screenManager.contentMgr.Load<Texture2D>("bomb/twist_knob");
            yellow_broken = screenManager.contentMgr.Load<Texture2D>("bomb/yellow_broken");
            yellow = screenManager.contentMgr.Load<Texture2D>("bomb/yellow");

            stoplightTexs[(int)StoplightTextures.Off] = screenManager.contentMgr.Load<Texture2D>("bomb/stoplight_off");
            stoplightTexs[(int)StoplightTextures.Red] = screenManager.contentMgr.Load<Texture2D>("bomb/stoplight_red");
            stoplightTexs[(int)StoplightTextures.Yellow] = screenManager.contentMgr.Load<Texture2D>("bomb/stoplight_yellow");
            stoplightTexs[(int)StoplightTextures.Green] = screenManager.contentMgr.Load<Texture2D>("bomb/stoplight_green");

            pressTexs[(int)PressTextures.Press] = screenManager.contentMgr.Load<Texture2D>("bomb/press");
            pressTexs[(int)PressTextures.DontPress] = screenManager.contentMgr.Load<Texture2D>("bomb/dont_press");


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
            sliderKnob[(int)SliderKnob.Down] = new Rectangle(329, 160, 38, 30);

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

            beepSfx = screenManager.contentMgr.Load<SoundEffect>("sounds/freesound_community-beep-beep-43875-edited");
            crankSfx = screenManager.contentMgr.Load<SoundEffect>("sounds/freesound_community-wind-up-toy-107907-edited");
            clickSfx = screenManager.contentMgr.Load<SoundEffect>("sounds/homemade_sfx-light-switch-flip-272436-edited");
            glassBreakSfx = screenManager.contentMgr.Load<SoundEffect>("sounds/dragon-studio-glass-breaking-386153-edited");
            glassBreakSfxInstance = glassBreakSfx.CreateInstance();
            explosionSfx = screenManager.contentMgr.Load<SoundEffect>("sounds/freesound_community-explosion-5981");
            clappingSfx = screenManager.contentMgr.Load<SoundEffect>("sounds/freesound_community-clapping-6474-edited");

            debugButton = new Button("debug");
            debugButton.Size = new Vector2(50, 30);
            debugButton.Position = new Vector2(200, 10);
            debugButton.Tapped += Debug_Tapped;
        }

        public override void HandleInput(GameTime gameTime, InputState input) {
            if (!isActive) { return; }

            if (input.isNewLeftMouseDown()) {
                foreach (Button b in allClickableThings) {
                    if (b.HandleTap(input.translatedMousePosition(viewport).ToVector2())) {
                        break;
                    }
                }

                debugButton.HandleTap(input.translatedMousePosition(viewport).ToVector2());
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
                    bombClock = 0f;
                    DetonateBomb();
                }

                if (twistKnobDegrees > 0) {
                    twistKnobDegrees += 5;
                    if (twistKnobDegrees > 360) {
                        twistKnobDegrees = 0;    
                    }
                }

                // This will end the game if the bomb is either in a detonate state or is successfully defused.
                CheckBombState();
            }
            base.Update(gameTime);
        }

        // Should we go boom?
        public void CheckBombState() {
            // if (bombClock > 999f) { return; } // we're debugging
            // Check for things that can cause an immediate boom
            if (twistKnobTurned > defuseScenario.twistKnobTurned) { 
                DetonateBomb(); 
            }
            for (int x=0; x<(int)FuseColor.Last; x++) {
                if (fusesBroken[x] && defuseScenario.fusesBroken[x] == false) {
                    DetonateBomb();
                }
            }
            if (keypadString.Length > defuseScenario.keypadString.Length) {
                DetonateBomb();
            }
            if (defuseScenario.keypadString.StartsWith(keypadString) == false) {
                DetonateBomb();
            }

            // Now check to see if the bomb has been defused
            if (stoplightTextureIdx != defuseScenario.stoplightTextureIdx) { return; }
            if (twistKnobTurned < defuseScenario.twistKnobTurned) { return; }
            for (int x=0; x<(int)FuseColor.Last; x++) {
                if (fusesBroken[x] != defuseScenario.fusesBroken[x]) {
                    return;
                }
            }
            for (int x=0; x<(int)Switches.Last; x++) {
                if (switchesFlipped[x] != defuseScenario.switchesFlipped[x]) {
                    return;
                }
            }
            if (pressTexture != defuseScenario.pressTexture) { return; }
            if (sliderPosition != defuseScenario.sliderPosition) { return; }

            // If we're here, the bomb was successfully defused!
            BombDefused();
        }
        private void BombDefused() {
            clappingSfx.Play();
            gameIsActive = false;
            tickingClockInstance.Stop();
            screenManager.BombDefused();
        }

        private void DetonateBomb() {
            explosionSfx.Play();
            gameIsActive = false;
            tickingClockInstance.Stop();
            screenManager.BombExploded();
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
            var keypadRect = displays[(int)Displays.Keypad];
            stringSize = screenManager.cursedTimer12ptFont.MeasureString(keypadString);
            smallYAxisFontAdjustment = 3;
            sb.Draw(screenManager.blankTexture, keypadRect, Color.Black);
            Color keypadTextColor = Color.Green;
            sb.DrawString(screenManager.cursedTimer12ptFont, keypadString, 
                            new Vector2(keypadRect.Left + keypadRect.Width / 2 - stringSize.X / 2, 
                                        keypadRect.Top + keypadRect.Height / 2 - stringSize.Y / 2 + smallYAxisFontAdjustment), 
                            keypadTextColor);

            // Now the bomb. Everything else is on top
            sb.Draw(bomb, Point.Zero.ToVector2(), Color.White);

            // Draw the twist knob
            Vector2 twistKnobPivot = new Vector2(twist_knob.Width / 2f, twist_knob.Height / 2f);
            sb.Draw(twist_knob, twistKnob.Center.ToVector2(), null, Color.White, MathHelper.ToRadians(twistKnobDegrees), twistKnobPivot, 1f, SpriteEffects.None, 1f);
            if (twistOverlay > 0) {
                sb.DrawString(screenManager.font, twistOverlay.ToString(), new Vector2(twistKnob.Right - 10, twistKnob.Bottom - 20), Color.Navy);
            }

            // Draw fuses
            for (int x=0; x<(int)FuseColor.Last; x++) {
                var fuse = drawForFuse((FuseColor)x);
                sb.Draw(fuse.t, fuse.r, Color.White);
            }

            // Draw Stoplights
            sb.Draw(textureForStoplight(), stoplights[(int)Stoplight.Lights], Color.White);

            // Draw Don't Press button
            sb.Draw(textureForDontPress(), dontPressButton, Color.White);

            // Draw slider knob
            sb.Draw(slider_knob, sliderKnob[(int)sliderPosition], Color.White);

            // Draw switches
            for (int x=0; x<(int)Switches.Last; x++) {
                var effect = switchesFlipped[x] ? SpriteEffects.FlipVertically : SpriteEffects.None;
                sb.Draw(toggle_switch, switches[x], null, Color.White, 0f, Vector2.Zero, effect, 0f);
            }

            // debugButton.Draw(this);

            sb.End();

            if (debugString.Length > 0) {
                Utilities.DebugString(screenManager, debugString, Vector2.One);
            }
            base.Draw(gameTime);
        }

        string keypadString = "";
        void Keypad_Tapped(object sender, EventArgs e) {
            keypadString += (sender as Button).intValue;
            beepSfx.Play();
        }

        bool[] fusesBroken = [false, false, false, false, false];
        void Fuse_Tapped(object sender, EventArgs e) {
            FuseColor fc = (FuseColor)(sender as Button).intValue;
            fusesBroken[(int)fc] = true;
            glassBreakSfxInstance.Play();
        }

        SliderKnob sliderPosition = SliderKnob.Up;
        void Slider_Tapped(object sender, EventArgs e) {
            sliderPosition = (sliderPosition == SliderKnob.Up) ? SliderKnob.Down : SliderKnob.Up;
            clickSfx.Play();
        }

        internal bool[] switchesFlipped = [false, false, false, false];
        void Switch_Tapped(object sender, EventArgs e) {
            Switches sw = (Switches)(sender as Button).intValue;
            switchesFlipped[(int)sw] = !switchesFlipped[(int)sw];
            clickSfx.Play();
        }

        internal int twistKnobTurned = 0;
        internal float twistKnobDegrees = 0f;
        void OtherButton_Tapped(object sender, EventArgs e) {
            OtherButtons ob = (OtherButtons)(sender as Button).intValue;

            switch (ob) {
                case OtherButtons.DontPress:
                    pressTexture = (pressTexture + 1) % 2;
                    clickSfx.Play();
                    break;
                case OtherButtons.Stoplight:
                    beepSfx.Play();
                    stoplightTextureIdx = (stoplightTextureIdx + 1) % (int)StoplightTextures.Last;
                    break;
                case OtherButtons.TwistKnob:
                    if (twistKnobDegrees == 0) {
                        twistKnobTurned += 1;
                        twistKnobDegrees = 1;
                        crankSfx.Play();
                    }
                    break;
            }
        }

        private (Texture2D t, Rectangle r) drawForFuse(FuseColor c) {
            Texture2D texture;
            Rectangle rectangle = fuseButtons[(int)c];

            switch (c) {
                case FuseColor.Red:
                    texture = fusesBroken[(int)c] ? red_broken : red;
                    break;
                case FuseColor.Green:
                    texture = fusesBroken[(int)c] ? green_broken : green;
                    break;
                case FuseColor.Blue:
                    texture = fusesBroken[(int)c] ? blue_broken : blue;
                    break;
                case FuseColor.Yellow:
                    texture = fusesBroken[(int)c] ? yellow_broken : yellow;
                    break;
                case FuseColor.Orange:
                    texture = fusesBroken[(int)c] ? orange_broken : orange;
                    break;
                default:
                    texture = red;
                    break;
            }
            return (texture, rectangle);
        }

        internal int stoplightTextureIdx = 0;
        private Texture2D textureForStoplight() {
            return stoplightTexs[stoplightTextureIdx];
        }

        internal int pressTexture = 0;
        private Texture2D textureForDontPress() {
            return pressTexs[pressTexture];
        }

        public override void Reset() {
        }

        int bombIdx = 1;
        int twistOverlay = 0;
        internal void Debug_Tapped(object sender, EventArgs e) {
            bombClock = 99999f;
            var s = BombScenario.ScenarioWithID(5);
            // defusalService.SetDefuseScenario(s);

            debugString = $"setup for {s.id}";
            bombIdx += 1;

            stoplightTextureIdx = s.stoplightTextureIdx;
            for (int x=0; x<(int)FuseColor.Last; x++) {
                fusesBroken[x] = s.fusesBroken[x];
            }
            for (int x=0; x<(int)Switches.Last; x++) {
                switchesFlipped[x] = s.switchesFlipped[x];
            }
            pressTexture = s.pressTexture;
            sliderPosition = s.sliderPosition;
            keypadString = s.keypadString;

            twistOverlay = s.twistKnobTurned;
        }

        public void ConfigureBomb(BombScenario setup) {
            stoplightTextureIdx = setup.stoplightTextureIdx;
            twistKnobTurned = setup.twistKnobTurned;
            twistKnobDegrees = 0;
            fusesBroken = setup.fusesBroken;
            switchesFlipped = setup.switchesFlipped;
            pressTexture = setup.pressTexture;
            sliderPosition = setup.sliderPosition;
            keypadString = setup.keypadString;
        }


        //
        // IBombScreenService
        //
        public void GameCompleted() {
            gamesCompleted += 1;
            defusalService.RevealDoor(gamesCompleted);
        }

        public void GameStarted() {
            defuseScenario = BombScenario.ChooseRandomScenario();
            var setupScenario = BombScenario.CreateSetupScenario();
            ConfigureBomb(setupScenario);
            defusalService.SetDefuseScenario(defuseScenario, setupScenario);
            gamesCompleted = 0;
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

    // What blows you up immediately:
    //      too many turns of the twist knob
    //      breaking a fuse you shouldn't have
    //      typing a passkey that is
    //          too long
    //          not correct
    public class BombScenario() {
        internal int id = 0;
        internal int stoplightTextureIdx = 0;
        internal int twistKnobTurned = 0;
        internal bool[] fusesBroken = [false, false, false, false, false];
        internal bool[] switchesFlipped = [false, false, false, false];
        internal int pressTexture = (int)BombScreen.PressTextures.DontPress;
        internal BombScreen.SliderKnob sliderPosition = BombScreen.SliderKnob.Up;
        internal string keypadString = "";

        internal static BombScenario ChooseRandomScenario() {
            int numScenarios = 6;
            return BombScenario.ScenarioWithID(Random.Shared.Next(1,numScenarios+1));
        }

        internal static BombScenario ScenarioWithID(int id) {
            string name = "BombScenario" + id;
            Type type = typeof(BombScenario).Assembly.GetType($"{typeof(BombScenario).Namespace}.{name}");
            if (type == null) {
                throw new System.Exception("ChooseRandomScenario could not find a scenario named: " + $"{typeof(BombScenario).Namespace}.{name}");
            }

            object instance = Activator.CreateInstance(type);
            BombScenario myObj = (BombScenario)instance;
            return myObj;
        }

        // Used by both the BombScene to give an initial setup of the board and by DefusalInstructions to know what
        // hints will be need to defuse the bomb.
        // This should be random, but only for things that don't immediatly trip the bomb.
        // See comment above BombScenario for what those are.
        internal static BombScenario CreateSetupScenario() {
            return new BombScenario() {
            stoplightTextureIdx = Random.Shared.Next(0, (int)BombScreen.StoplightTextures.Last),
            twistKnobTurned = 0,
            fusesBroken = [false, false, false, false, false],
            switchesFlipped = [Random.Shared.Next(0,2) == 1, 
                               Random.Shared.Next(0,2) == 1,
                               Random.Shared.Next(0,2) == 1,
                               Random.Shared.Next(0,2) == 1],
            pressTexture = Random.Shared.Next(0,2) == 1 ? (int)BombScreen.PressTextures.DontPress : (int)BombScreen.PressTextures.Press,
            sliderPosition = Random.Shared.Next(0,2) == 1 ? BombScreen.SliderKnob.Up : BombScreen.SliderKnob.Down,
            keypadString = ""
            };
        }
    }

    // These are what the values should be to consider the bomb defused
    // Please read these notes! There are some limitations.
    /*
    // When setting switcheFlipped, you can only choose one of these. And you must choose one, not flipping any switches wont' work.
        [true,false,false,false]
        [true,true,false,false]
        [true,true,true,false]
        [true,true,true,true]
        [false,true,false,false]
        [false,true,true,false]
        [false,true,true,true]
        [false,false,true,false]
        [false,false,true,true]
        [false,false,false,true]
    */
    // Fuses: You can only break two. Any two, but only two.
    // Random values: they'd work, but then you'd never know what the scenario would be to validate.  Maybe that's not a bad thing.
    public class BombScenario1 : BombScenario {
        public BombScenario1() : base() {
            id = 1;
            fusesBroken = [true, false, false, true, false];
            keypadString = "88913";
            switchesFlipped = [true, false, false, false];
            pressTexture = (int)BombScreen.PressTextures.DontPress;
            stoplightTextureIdx = (int)BombScreen.StoplightTextures.Yellow;
        }
    }

    public class BombScenario2 : BombScenario {
        public BombScenario2() : base() {
            id = 2;
            fusesBroken = [true, true, false, false, false];
            keypadString = "999999";
            switchesFlipped = [false, false, false, true];
            pressTexture = (int)BombScreen.PressTextures.Press;
            stoplightTextureIdx = (int)BombScreen.StoplightTextures.Red;
            twistKnobTurned = 1;
        }
    }

    public class BombScenario3 : BombScenario {
        public BombScenario3() : base() {
            id = 3;
            fusesBroken = [false, true, false, false, true];
            keypadString = "107734";
            switchesFlipped = [false,true,true,false];
            pressTexture = (int)BombScreen.PressTextures.DontPress;
            twistKnobTurned = 2;
        }
    }

    public class BombScenario4 : BombScenario {
        public BombScenario4() : base() {
            id = 4;
            fusesBroken = [false, false, false, true, true];
            keypadString = "67";
            switchesFlipped = [false,true,true,true];
            pressTexture = (int)BombScreen.PressTextures.DontPress;
            sliderPosition = Random.Shared.Next(0,2) == 1 ? BombScreen.SliderKnob.Up : BombScreen.SliderKnob.Down;
            twistKnobTurned = 3;
        }
    }

    public class BombScenario5 : BombScenario {
        public BombScenario5() : base() {
            id = 5;
            sliderPosition = Random.Shared.Next(0,2) == 1 ? BombScreen.SliderKnob.Up : BombScreen.SliderKnob.Down;
            fusesBroken = [true, false, false, true, false];
            switchesFlipped = [true,true,true,true];
            keypadString = "8675309";
            twistKnobTurned = 1;
        }
    }

    public class BombScenario6 : BombScenario {
        public BombScenario6() : base() {
            id = 6;
            sliderPosition = Random.Shared.Next(0,2) == 1 ? BombScreen.SliderKnob.Up : BombScreen.SliderKnob.Down;
            keypadString = "123467890";
            fusesBroken = [false, false, true, true, false];
            switchesFlipped = [true,true,true,false];
            pressTexture = (int)BombScreen.PressTextures.Press;
            twistKnobTurned = 2;
        }
    }

}
