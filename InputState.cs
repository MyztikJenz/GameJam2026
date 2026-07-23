using Microsoft.Xna.Framework;
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

        public InputState() {
            currentKeyboardStates = new KeyboardState[maxInputs];
            lastKeyboardStates = new KeyboardState[maxInputs];
        }

        public void Update() {
            for (int i=0; i<maxInputs; i++) {
                lastKeyboardStates[i] = currentKeyboardStates[i];
                currentKeyboardStates[i] = Keyboard.GetState();
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
    }
}