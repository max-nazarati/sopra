using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class Projectile
    {
        private readonly Vector2 mDirection;
        private float mX, mY;
        private readonly Texture2D mProjectile;
        public Projectile(ContentManager content, Vector2 direction, Vector2 startPoint)
        {
            mDirection = direction;
            mX = (int)startPoint.X+25;
            mY = (int)startPoint.Y+25;
            mProjectile = content.Load<Texture2D>("Projectile");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mProjectile, new Rectangle((int)mX, (int)mY, 8, 8), null, Color.Red);
        }

        public void Update()
        {
            mX += mDirection.X * 7;
            mY += mDirection.Y * 7;
        }
    }
}