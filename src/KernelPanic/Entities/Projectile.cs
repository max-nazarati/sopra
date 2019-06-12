using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class Projectile
    {
        private readonly Vector2 mDirection, mStartPoint;
        private readonly ImageSprite mSprite;
        private readonly float mRadius;
        
        public Projectile(Vector2 direction, Vector2 startPoint, float radius, SpriteManager sprites)
        {
            mStartPoint = startPoint;
            mDirection = direction;
            mRadius = radius;
            
            mSprite = sprites.CreateProjectile();
            mSprite.Position = startPoint;
            mSprite.TintColor = Color.Red;
            mSprite.ScaleToWidth(8);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Vector2.Distance(mStartPoint, mSprite.Position) < mRadius)
            {
                mSprite.Draw(spriteBatch, gameTime);
            }
        }

        public void Update()
        {
            mSprite.X += mDirection.X * 7;
            mSprite.Y += mDirection.Y * 7;
        }
    }
}
