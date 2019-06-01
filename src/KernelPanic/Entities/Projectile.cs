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
        private readonly float mRadius;
        private readonly Texture2D mProjectile;
        
        public Projectile(ContentManager content, Vector2 direction, Vector2 startPoint, float radius)
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

        private float Distance()
        {
            return (float) Math.Sqrt(Math.Pow(mX - mStartpoint.X, 2) +
                                     Math.Pow(mY - mStartpoint.Y, 2));
        }
    }
}
