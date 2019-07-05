using System;
using System.Collections.Generic;
using KernelPanic.Data;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Entities.Projectiles
{
    internal class Projectile
    {
        protected readonly Vector2 mDirection, mStartPoint;
        protected readonly float mRadius;
        protected readonly int mSpeed, mDamage;
        private List<Entity> mHasHitted;
        public bool mHasHit;

        internal ImageSprite Sprite { get; }

        public Projectile(Vector2 direction, Vector2 startPoint, float radius, int size, int speed, int damage, ImageSprite sprite)
        {
            mStartPoint = startPoint;
            direction.Normalize();
            mDirection = direction;
            mRadius = radius;
            mSpeed = speed;
            mDamage = damage;
            mHasHit = false;
            mHasHitted = new List<Entity>();

            Sprite = sprite;
            Sprite.Position = startPoint;
            Sprite.Rotation = direction.Angle(0.5);
            Sprite.SetOrigin(RelativePosition.Center);
            Sprite.ScaleToWidth(size);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Vector2.Distance(mStartPoint, Sprite.Position) < mRadius)
            {
                Sprite.Draw(spriteBatch, gameTime);
            }
        }

        public void Update(PositionProvider positionProvider)
        {
            Sprite.X += mDirection.X * mSpeed;
            Sprite.Y += mDirection.Y * mSpeed;
            foreach (var entity in positionProvider.NearEntities<Unit>(new Vector2(Sprite.X, Sprite.Y), mRadius))
            {
                if (!entity.Bounds.Intersects(Sprite.Bounds)) continue;
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
