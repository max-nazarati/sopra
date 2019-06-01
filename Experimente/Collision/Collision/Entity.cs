using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Collision
{
    public abstract class Entity
    {
        protected Rectangle mContainerRectangle;

        private Rectangle mOldContainerRectangle;
        //private GraphicsDeviceManager mGraphics;
        private readonly Texture2D mTexture;
        protected MouseState mOldMouseState;
        protected MouseState mMouseState;
        protected KeyboardState mKeyboardState;
        protected KeyboardState mOldKeyboardState;

        protected Entity(int x, int y, int width, int height, Texture2D texture)
        {
            mContainerRectangle = new Rectangle(new Point(x, y), new Point(width, height));
            mOldContainerRectangle = new Rectangle(new Point(x, y), new Point(width, height));
            mTexture = texture;
            //mGraphics = graphics;
        }

        protected void Update()
        {
            mOldMouseState = mMouseState;
            mOldKeyboardState = mKeyboardState;
            mKeyboardState = Keyboard.GetState();
            mMouseState = Mouse.GetState();

            // store last position
            mOldContainerRectangle = mContainerRectangle;
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

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Begin();
            spriteBatch.Draw(mTexture, mContainerRectangle, Color.White);
            //spriteBatch.End();
        }
    }
}
