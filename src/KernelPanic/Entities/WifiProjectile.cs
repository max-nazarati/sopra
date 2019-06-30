using System;
using KernelPanic.Data;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Entities
{
    internal class WifiProjectile : Projectile
    {
        private readonly Vector2 mDirection2, mDirection3;
        private readonly Sprite mSprite2, mSprite3;
        public bool mHasHit2, mHasHit3;
        public WifiProjectile(Vector2 direction, Vector2 startPoint, float radius, float rotation, int size, int speed
            , ImageSprite sprite) : base(direction, startPoint, radius, rotation, size, speed, sprite)
        {
            mHasHit2 = false;
            mHasHit3 = false;
            mSprite.Rotation = (float)(rotation % (Math.PI * 2));
            mSprite2 = sprite.Clone();
            mSprite2.Rotation = (float)(rotation % (Math.PI * 2)+0.4);
            mSprite3 = sprite.Clone();
            mSprite2.SetOrigin(RelativePosition.Center);
            mSprite3.SetOrigin(RelativePosition.Center);
            mSprite3.Rotation = (float)(rotation % (Math.PI * 2)-0.4);;
            mDirection2 = new Vector2(
                (float) Math.Sin(rotation % (Math.PI * 2)+0.4),
                -(float) Math.Cos(rotation % (Math.PI * 2)+0.4));
            mDirection3 = new Vector2(
                (float) Math.Sin(rotation % (Math.PI * 2)-0.4),
                -(float) Math.Cos(rotation % (Math.PI * 2)-0.4));
            mDirection.Normalize();
            mDirection2.Normalize();
            mDirection3.Normalize();
        }
        
        public new void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!(Vector2.Distance(mStartPoint, mSprite.Position) < mRadius) ||
                !(Vector2.Distance(mStartPoint, mSprite2.Position) < mRadius) ||
                !(Vector2.Distance(mStartPoint, mSprite3.Position) < mRadius)) return;
            if (!mHasHit) mSprite.Draw(spriteBatch, gameTime);
            if (!mHasHit2) mSprite2.Draw(spriteBatch, gameTime);
            if (!mHasHit3) mSprite3.Draw(spriteBatch, gameTime);
        }

        public new void Update(PositionProvider positionProvider)
        {
            mSprite.Position += mDirection * mSpeed;
            mSprite2.Position += mDirection2 * mSpeed;
            mSprite3.Position += mDirection3 * mSpeed;
            
            foreach (var entity in positionProvider.NearEntities<Unit>(new Vector2(mSprite.X, mSprite.Y), mRadius))
            {
                if (!entity.Bounds.Intersects(mSprite.Bounds) || mHasHit) continue;
                mHasHit = true;
                entity.DealDamage(2);
            }
            
            foreach (var entity in positionProvider.NearEntities<Unit>(new Vector2(mSprite2.X, mSprite2.Y), mRadius))
            {
                if (!entity.Bounds.Intersects(mSprite2.Bounds) || mHasHit2) continue;
                mHasHit2 = true;
                entity.DealDamage(2);
            }
            
            foreach (var entity in positionProvider.NearEntities<Unit>(new Vector2(mSprite3.X, mSprite3.Y), mRadius))
            {
                if (!entity.Bounds.Intersects(mSprite3.Bounds) || mHasHit3) continue;
                mHasHit3 = true;
                entity.DealDamage(2);
            }
        }
    }
}