using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Interface
{
    public abstract class InterfaceComponent: IDrawable
    {
        internal bool Enabled { get; set; } = true;

        internal abstract Sprite Sprite { get; }

        internal abstract void Update(GameTime gameTime, InputManager inputManager);

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, gameTime);
        }
    }
}
