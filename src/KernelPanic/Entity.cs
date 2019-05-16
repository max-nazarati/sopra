using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    public abstract class Entity
    {
        protected Rectangle ContainerRectangle;
        protected GraphicsDeviceManager Graphics;
        protected Texture2D Texture;
        protected MouseState OldMouseState;
        protected MouseState MouseState;
        protected KeyboardState KeyboardState;
        protected KeyboardState OldKeyboardState;

        protected Entity(int x, int y, int width, int height, Texture2D texture, GraphicsDeviceManager graphics)
        {
            ContainerRectangle = new Rectangle(new Point(x, y), new Point(width, height));
            Texture = texture;
            Graphics = graphics;
        }
        public virtual void Update(GameTime gameTime)
        {
            OldMouseState = MouseState;
            OldKeyboardState = KeyboardState;
            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Texture, ContainerRectangle, Color.White);
            spriteBatch.End();
        }
    }
}
