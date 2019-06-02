using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal class Tower : Building
    {
        private readonly float mRadius;
        private readonly CooldownComponent mFireTimer;
        private readonly List<Projectile> mProjectiles = new List<Projectile>();
        private bool mInRange;

        internal Tower(int price, float radius, TimeSpan cooldown, Sprite sprite, SpriteManager sprites) : base(price, sprite)
        {
            mFireTimer = new CooldownComponent(cooldown);
            mRadius = radius;

            mFireTimer.CooledDown += timer =>
            {
                if (!mInRange)
                {
                    // If the cursor isn't in the range do nothing for now but keep the timer enabled.
                    // If it is enabled it keeps calling this callback.
                    timer.Enabled = true;
                    return;
                }

                var direction = new Vector2(
                    (float) Math.Sin(Sprite.Rotation % (Math.PI * 2)),
                    -(float) Math.Cos(Sprite.Rotation % (Math.PI * 2)));
                mProjectiles.Add(new Projectile(direction, Sprite.Position, mRadius, sprites));

                SoundManager.Instance.PlaySound("shoot");

                if (mProjectiles.Count > 5)
                {
                    mProjectiles.RemoveAt(0);
                }

                timer.Reset();
            };
        }

        internal static Tower Create(Vector2 position, float size, SpriteManager sprites)
        {
            var sprite = sprites.CreateTower();
            sprite.Position = position;
            sprite.ScaleToHeight(size);
            sprite.SetOrigin(RelativePosition.Center);
            return new Tower(0, 300, new TimeSpan(0, 0, 3), sprite, sprites);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            //var s = (ImageSprite) Sprite;
            //spriteBatch.Draw(s.Texture, s.Position, null, Color.White, s.Rotation, new Vector2(s.Width / 2), 0.08f, SpriteEffects.None, 1.0f);
            
            foreach (var projectile in mProjectiles)
            {
                projectile.Draw(spriteBatch, gameTime);
            }
        }

        internal override void Update(GameTime gameTime, Matrix invertedViewMatrix)
        {
            // Turn window coordinates into world coordinates.
            var relativeMouseVector =
                Vector2.Transform(InputManager.Default.MousePosition.ToVector2(), invertedViewMatrix);
            var distance = Vector2.Distance(relativeMouseVector, Sprite.Position);
            mInRange = distance <= mRadius;

            // If the cursor is in range we rotate the tower in its direction, otherwise we let the tower rotate continuously. 
            Sprite.Rotation = mInRange
                ? (float) (Math.Atan2(Sprite.Y - relativeMouseVector.Y, Sprite.X - relativeMouseVector.X) - Math.PI / 2)
                : (float) Math.Sin(gameTime.TotalGameTime.TotalSeconds * 10 % (2 * Math.PI)) / 2;

            mFireTimer.Update(gameTime);
            foreach (var projectile in mProjectiles)
            {
                projectile.Update();
            }
        }
    }
}