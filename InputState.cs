using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/*
This is a culled down version of the InputStates from GameStateManagementSample-Web. It was 
written to support multiple players which we don't care about. But I left basic support in there
just in case it becomes important later.
*/

namespace GameJam2026 {
    public class InputState {
        public const int maxInputs = 1;
        public readonly KeyboardState[] currentKeyboardStates;
        public readonly KeyboardState[] lastKeyboardStates;
        public readonly MouseState[] currentMouseStates;
        public readonly MouseState[] lastMouseStates;


        public InputState() {
            currentKeyboardStates = new KeyboardState[maxInputs];
            lastKeyboardStates = new KeyboardState[maxInputs];
            currentMouseStates = new MouseState[maxInputs];
            lastMouseStates = new MouseState[maxInputs];
        }

        public void Update() {
            for (int i=0; i<maxInputs; i++) {
                lastKeyboardStates[i] = currentKeyboardStates[i];
                currentKeyboardStates[i] = Keyboard.GetState();

                lastMouseStates[i] = currentMouseStates[i];
                currentMouseStates[i] = Mouse.GetState();
            }
        }

        public bool isKeyPressed(Keys key, PlayerIndex idx = PlayerIndex.One) {
            int x = (int)idx;
            return currentKeyboardStates[x].IsKeyDown(key);
        }

        public bool isNewKeyPress(Keys key, PlayerIndex idx = PlayerIndex.One) {
            int x = (int)idx;
            return (currentKeyboardStates[x].IsKeyDown(key) && 
                    lastKeyboardStates[x].IsKeyUp(key));
        }

        public bool isLeftMouseDown(PlayerIndex idx = PlayerIndex.One) {
            int x = (int)idx;
            return currentMouseStates[x].LeftButton == ButtonState.Pressed;
        }

        public bool isNewLeftMouseDown(PlayerIndex idx = PlayerIndex.One) {
            int x = (int)idx;
            return currentMouseStates[x].LeftButton == ButtonState.Pressed &&
                    lastMouseStates[x].LeftButton == ButtonState.Released;
        }

        public bool isRightMouseDown(PlayerIndex idx = PlayerIndex.One) {
            int x = (int)idx;
            return currentMouseStates[x].RightButton == ButtonState.Pressed;
        }

        public bool isNewRightMouseDown(PlayerIndex idx = PlayerIndex.One) {
            int x = (int)idx;
            return currentMouseStates[x].RightButton == ButtonState.Pressed &&
                    lastMouseStates[x].RightButton == ButtonState.Released;
        }

        public Point mousePosition(PlayerIndex idx = PlayerIndex.One) {
            int x = (int)idx;
            return currentMouseStates[x].Position;
        }

        // Mouse coordinate are in screen space. Translate them to viewport space
        public Point translatedMousePosition(Viewport viewport, PlayerIndex idx = PlayerIndex.One) {
            var p = mousePosition(idx);
            p.X -= viewport.Bounds.X;
            p.Y -= viewport.Bounds.Y;

            return p;
        }

        public int? isNumberKeyPressed(PlayerIndex idx = PlayerIndex.One) {
            int x = (int)idx;
            int? numPressed = null;
            if (isNewKeyPress(Keys.D0) || isNewKeyPress(Keys.NumPad0)) { numPressed = 0; }
            if (isNewKeyPress(Keys.D1) || isNewKeyPress(Keys.NumPad1)) { numPressed = 1; }
            if (isNewKeyPress(Keys.D2) || isNewKeyPress(Keys.NumPad2)) { numPressed = 2; }
            if (isNewKeyPress(Keys.D3) || isNewKeyPress(Keys.NumPad3)) { numPressed = 3; }
            if (isNewKeyPress(Keys.D4) || isNewKeyPress(Keys.NumPad4)) { numPressed = 4; }
            if (isNewKeyPress(Keys.D5) || isNewKeyPress(Keys.NumPad5)) { numPressed = 5; }
            if (isNewKeyPress(Keys.D6) || isNewKeyPress(Keys.NumPad6)) { numPressed = 6; }
            if (isNewKeyPress(Keys.D7) || isNewKeyPress(Keys.NumPad7)) { numPressed = 7; }
            if (isNewKeyPress(Keys.D8) || isNewKeyPress(Keys.NumPad8)) { numPressed = 8; }
            if (isNewKeyPress(Keys.D9) || isNewKeyPress(Keys.NumPad9)) { numPressed = 9; }

            return numPressed;
        }

    }
}