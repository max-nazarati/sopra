using Microsoft.Xna.Framework;

namespace KernelPanic
{
    internal abstract class Entity
    {
        protected Rectangle mContainerRectangle;
        private Rectangle mOldContainerRectangle;
        //private GraphicsDeviceManager mGraphics;
        //private Texture2D Texture;

        protected Entity(int x, int y, int width, int height)
        {
            mContainerRectangle = new Rectangle(new Point(x, y), new Point(width, height));
            mOldContainerRectangle = new Rectangle(new Point(x, y), new Point(width, height));
            //Texture = texture;
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

        internal virtual void Update()
        {
            // store last position
            mOldContainerRectangle = mContainerRectangle;
        }

        /*public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Texture, mContainerRectangle, Color.White);
            spriteBatch.End();
        }*/
    }
}
