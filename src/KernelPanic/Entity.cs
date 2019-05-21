using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    internal abstract class Entity
    {
        protected Rectangle mContainerRectangle;
        //private GraphicsDeviceManager mGraphics;
        //private Texture2D Texture;
        protected MouseState mOldMouseState;
        protected MouseState mMouseState;
        protected KeyboardState mKeyboardState;
        protected KeyboardState mOldKeyboardState;

        protected Entity(int x, int y, int width, int height)
        {
            mContainerRectangle = new Rectangle(new Point(x, y), new Point(width, height));
            //Texture = texture;
            //mGraphics = graphics;
        }

        internal virtual void Update()
        {
            mOldMouseState = mMouseState;
            mOldKeyboardState = mKeyboardState;
            mKeyboardState = Keyboard.GetState();
            mMouseState = Mouse.GetState();
        }

        /*public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Texture, mContainerRectangle, Color.White);
            spriteBatch.End();
        }*/
    }
}
