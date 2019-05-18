using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    public abstract class State
    {
        protected ContentManager Content;
        protected GraphicsDeviceManager Graphics;
        protected StateManager SManager;
        protected KeyboardState KeyboardState;
        protected KeyboardState OldKeyboardState;
        protected MouseState MouseState;
        protected MouseState OldMouseState;


        protected State(StateManager stateManager, GraphicsDeviceManager graphics, ContentManager content)
        {
            Content = content;
            Graphics = graphics;
            SManager = stateManager;
        }
        public virtual void Update(GameTime gameTime)
        {
            KeyboardState = Keyboard.GetState();
            OldKeyboardState = new KeyboardState();
            MouseState = Mouse.GetState();
            OldMouseState = new MouseState();
        }
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
