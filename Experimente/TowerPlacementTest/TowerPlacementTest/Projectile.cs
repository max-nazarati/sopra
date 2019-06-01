using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TowerPlacementTest
{
    internal sealed class Projectile
    {
        private readonly Vector2 mDirection;
        private float mX, mY;
        private readonly Texture2D mProjectile;
        public Projectile(ContentManager content, Vector2 direction, Vector2 startPoint)
        {
            this.mDirection = direction;
            this.mX = (int)startPoint.X;
            this.mY = (int)startPoint.Y;
            mProjectile = content.Load<Texture2D>("Projectile");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mProjectile, new Rectangle((int)mX, (int)mY, 20, 20), null, Color.Red);
        }

        public void Update()
        {
            mX += mDirection.X * 15;
            mY += mDirection.Y * 15;
        }
    }
}
