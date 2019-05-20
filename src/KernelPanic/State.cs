using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    internal abstract class State
    {
        protected ContentManager mContent;
        protected GraphicsDeviceManager Graphics;
        internal StateManager SManager;
        protected KeyboardState mKeyboardState;
        protected KeyboardState mOldKeyboardState;
        protected MouseState mMouseState;
        protected MouseState mOldMouseState;


        internal State(StateManager stateManager, GraphicsDeviceManager graphics, ContentManager content)
        {
            mContent = content;
            Graphics = graphics;
            SManager = stateManager;
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
