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
        public bool mHasHit;
        
        public Projectile(Vector2 direction, Vector2 startPoint, float radius, float rotation
            , int size, ImageSprite sprite)
        {
            mStartPoint = startPoint;
            mDirection = direction;
            mRadius = radius;
            mHasHit = false;

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
            mSprite.X += mDirection.X * 7;
            mSprite.Y += mDirection.Y * 7;
            foreach (var entity in positionProvider.NearEntities<Unit>(new Vector2(mSprite.X, mSprite.Y), mRadius))
            {
                if (!entity.Bounds.Intersects(mSprite.Bounds)) continue;
                mHasHit = true;
                entity.DealDamage(2);
                break;
            }
        }
    }
}
