using System;
using System.Collections.Generic;
using KernelPanic.Data;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Entities
{
    internal class Projectile
    {
        protected readonly Vector2 mDirection, mStartPoint;
        protected readonly ImageSprite mSprite;
        protected readonly float mRadius;
        protected readonly int mSpeed, mDamage;
        private List<Entity> mHasHitted;
        public bool mHasHit;

        public Projectile(Vector2 direction, Vector2 startPoint, float radius, float rotation
            , int size, int speed, int damage, ImageSprite sprite)
        {
            mStartPoint = startPoint;
            mDirection = direction;
            mRadius = radius;
            mSpeed = speed;
            mDamage = damage;
            mHasHit = false;
            mHasHitted = new List<Entity>();

            mSprite = sprite;
            mSprite.Position = startPoint;
            mSprite.Rotation = rotation;
            mSprite.SetOrigin(RelativePosition.Center);
            mSprite.TintColor = Color.White;
            mSprite.ScaleToWidth(size);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Vector2.Distance(mStartPoint, mSprite.Position) < mRadius)
            {
                mSprite.Draw(spriteBatch, gameTime);
            }
        }

        public void Update(PositionProvider positionProvider)
        {
            mSprite.X += mDirection.X * mSpeed;
            mSprite.Y += mDirection.Y * mSpeed;
            foreach (var entity in positionProvider.NearEntities<Unit>(new Vector2(mSprite.X, mSprite.Y), mRadius))
            {
                if (!entity.Bounds.Intersects(mSprite.Bounds)) continue;
                mHasHit = true;
                if (!mHasHitted.Contains(entity))
                {
                    entity.DealDamage(mDamage);
                    mHasHitted.Add(entity);
                }

                break;
            }
        }
    }
}
