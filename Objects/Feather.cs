using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace GameJam2026 {
    public class Feather {
        public static List<Feather> CreateFeathers(ContentManager mgr, int viewportWidth) {
            List<Feather> feathers = new List<Feather>();

            for (int x=1; x<5; x++) {
                Texture2D texture = mgr.Load<Texture2D>("feather" + x);
                Feather f = new Feather(texture, viewportWidth);
                feathers.Add(f);
            }

            return feathers;
        }

        private Texture2D feather;
        public Vector2 featherPosition { get; private set; }
        private Vector2 originalFeatherPosition;
        float featherRotation;
        int oscillatorDirection = -1;
        private Random random = new Random();
        private int viewportWidth;
        // The amount of X distance this feather will travel
        private int featherDistance;

        public Feather(Texture2D texture, int viewportWidth) {
            feather = texture;
            this.viewportWidth = viewportWidth;
        }

        public Feather(Feather sourceObj) {
            this.feather = sourceObj.feather;
            this.viewportWidth = sourceObj.viewportWidth;
            this.featherPosition = new Vector2(random.Next(10, this.viewportWidth - 20), random.Next(-45, -10));
            this.originalFeatherPosition = this.featherPosition;
            this.featherRotation = 0f;
            this.featherDistance = random.Next(50, 125);
        }

        public bool Intersects(Rectangle r) {
            Rectangle f = new Rectangle((int)featherPosition.X, (int)featherPosition.Y, feather.Bounds.Width, feather.Bounds.Height);
            f.Inflate(feather.Bounds.Width / -4, feather.Bounds.Height / -4);

            return f.Intersects(r);
        }

        public void Update(GameTime gameTime) {
            Vector2 featherMovement = Vector2.Zero;
            if (oscillatorDirection < 0) {
                if (featherPosition.X - originalFeatherPosition.X < -featherDistance) {
                    oscillatorDirection = 1;
                }
            }
            else if (oscillatorDirection > 0) {
                if (featherPosition.X - originalFeatherPosition.X > featherDistance) {
                    oscillatorDirection = -1;
                }
            }
            // So normalization has a weird effect... once one value gets too large the other starts to shrink. 
            // Moving X too large will limit the change in Y, and vise versa.
            featherMovement.X = oscillatorDirection * 10;
            // 100 is easy, 10 is faster and makes the rotation/movement slower. Probably would need to jump up X
            featherMovement.Y = gameTime.ElapsedGameTime.Milliseconds / 1f;
            featherMovement.Normalize();

            featherPosition += featherMovement * 4f;
            float delta = featherPosition.X - originalFeatherPosition.X;
            featherRotation = delta / 2 * -1;
        }

        // Do not call Begin and End here, we're inside a context already.
        public void Draw(ScreenManager mgr, GameTime gameTime) {
            SpriteBatch sb = mgr.spriteBatch;
            sb.Draw(feather, featherPosition, null, Color.White, MathHelper.ToRadians(featherRotation), new Vector2(feather.Width / 2, feather.Height / 2), 1f, SpriteEffects.None, 0f);
        }

    }
}