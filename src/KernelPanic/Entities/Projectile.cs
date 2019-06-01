using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class Projectile
    {
        private readonly Vector2 mDirection, mStartpoint;
        private float mX, mY;
        private int mRadius;
        private readonly Texture2D mProjectile;
        public Projectile(ContentManager content, Vector2 direction, Vector2 startPoint, int radius)
        {
            mStartpoint = startPoint;
            mDirection = direction;
            mX = (int)startPoint.X+25;
            mY = (int)startPoint.Y+25;
            mProjectile = content.Load<Texture2D>("Projectile");
            mRadius = radius;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Distance() < mRadius)
            {
                spriteBatch.Draw(mProjectile, new Rectangle((int)mX, (int)mY, 8, 8), null, Color.Red);
            }
        }

        public void Update()
        {
            mX += mDirection.X * 7;
            mY += mDirection.Y * 7;
        }

        private int Distance()
        {
            return (int) Math.Sqrt((int) Math.Pow(mX - mStartpoint.X, 2) +
                                   (int) Math.Pow(mY - mStartpoint.Y, 2));
        }
    }
}