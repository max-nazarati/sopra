using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    internal abstract class State
    {
        protected ContentManager mContent;
        protected readonly GraphicsDeviceManager mGraphics;
        internal readonly StateManager mSManager;
        protected KeyboardState mKeyboardState;
        protected KeyboardState mOldKeyboardState;
        protected MouseState mMouseState;
        protected MouseState mOldMouseState;


        internal State(StateManager stateManager, GraphicsDeviceManager graphics, ContentManager content)
        {
            mContent = content;
            mGraphics = graphics;
            mSManager = stateManager;
        }
        internal virtual void Update()
        {
            mKeyboardState = Keyboard.GetState();
            mOldKeyboardState = new KeyboardState();
            mMouseState = Mouse.GetState();
            mOldMouseState = new MouseState();
        }
        internal abstract void Draw(SpriteBatch spriteBatch);
    }
}
