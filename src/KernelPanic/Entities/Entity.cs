using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal abstract class Entity
    {
        protected Rectangle mContainerRectangle;
        private Rectangle mOldContainerRectangle;
        //private GraphicsDeviceManager mGraphics;
        private readonly Texture2D mTexture;

        // ReSharper disable once UnusedParameter.Local
        protected Entity(int param)
        {

        }

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once UnusedParameter.Local
        protected Entity(TimeSpan timeSpan)
        {

        }
        protected Entity(int x, int y, int width, int height, Texture2D texture)
        {
            mContainerRectangle = new Rectangle(new Point(x, y), new Point(width, height));
            mOldContainerRectangle = new Rectangle(new Point(x, y), new Point(width, height));
            mTexture = texture;
            //mGraphics = graphics;
        }

        public bool IsColliding(Entity Object)
        {
            return mContainerRectangle.Intersects(Object.mContainerRectangle);
        }

        public void CollisionDetected()
        {
            // moves the object back to the last position
            mContainerRectangle = mOldContainerRectangle;
        }

        public void CooledDownDelegate(CooldownComponent source)
        {
            mTexture.SetData(new[] { Color.Blue });
        }

        internal virtual void Update()
        {
            // store last position
            mOldContainerRectangle = mContainerRectangle;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mTexture, mContainerRectangle, Color.White);
        }
    }
}
