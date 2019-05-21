using Microsoft.Xna.Framework;

namespace KernelPanic
{
    internal abstract class Entity
    {
        protected Rectangle mContainerRectangle;
        //private GraphicsDeviceManager mGraphics;
        //private Texture2D Texture;

        protected Entity(int x, int y, int width, int height)
        {
            mContainerRectangle = new Rectangle(new Point(x, y), new Point(width, height));
            //Texture = texture;
            //mGraphics = graphics;
        }

        internal virtual void Update()
        {
        }

        /*public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Texture, mContainerRectangle, Color.White);
            spriteBatch.End();
        }*/
    }
}
