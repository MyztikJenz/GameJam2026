using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameJam2026 {
    // Need to just move a sprite around and check for collision? This is your guy.
    public class SimpleTracker {
        internal Texture2D texture;
        internal Vector2 position;
        Vector2 moveTo;
        // Default speed is 50px/sec
        internal int xSpeed = 50;
        internal int ySpeed = 50;

        public SimpleTracker(Texture2D t, Vector2 initialPosition, Vector2 moveTo) {
            texture = t;
            position = initialPosition;
            this.moveTo = moveTo;
        }

        public void Update(GameTime gameTime) {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Math.Abs(position.X - moveTo.X) > (float)xSpeed * deltaTime) {
                if (moveTo.X < position.X) {
                    position.X -= (float)xSpeed * deltaTime;
                }
                else {
                    position.X += (float)xSpeed * deltaTime;
                }
            }

            if (Math.Abs(position.Y - moveTo.Y) > (float)ySpeed * deltaTime) {
                if (moveTo.Y < position.Y) {
                    position.Y -= (float)ySpeed * deltaTime;
                }
                else {
                    position.Y += (float)ySpeed * deltaTime;
                }
            }
        }

        public bool Intersects(Rectangle r){
            Rectangle t = new Rectangle(position.ToPoint(), texture.Bounds.Size);
            return t.Intersects(r);
        }

        public bool HasArrived() {
            return (Math.Abs(position.X - moveTo.X) < 2 && Math.Abs(position.Y - moveTo.Y) < 2);
        }
    }
}