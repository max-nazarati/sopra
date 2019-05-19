using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    public abstract class Entity
    {
        protected Rectangle mContainerRectangle;
        //private GraphicsDeviceManager mGraphics;
        private Texture2D Texture;
        protected MouseState mOldMouseState;
        protected MouseState mMouseState;
        protected KeyboardState mKeyboardState;
        protected KeyboardState mOldKeyboardState;

        protected Entity(int x, int y, int width, int height, Texture2D texture)
        {
            mContainerRectangle = new Rectangle(new Point(x, y), new Point(width, height));
            Texture = texture;
            //mGraphics = graphics;
        }
        public virtual void Update()
        {
            mOldMouseState = mMouseState;
            mOldKeyboardState = mKeyboardState;
            mKeyboardState = Keyboard.GetState();
            mMouseState = Mouse.GetState();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Texture, mContainerRectangle, Color.White);
            spriteBatch.End();
        }
    }
}
