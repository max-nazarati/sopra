using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Interface
{
    internal abstract class InterfaceComponent: IBounded, IDrawable, IUpdatable
    {
        internal virtual bool Enabled { get; set; } = true;

        internal abstract Sprite Sprite { get; }

        public Rectangle Bounds => Sprite.Bounds;

        public abstract void Update(InputManager inputManager, GameTime gameTime);

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, gameTime);
        }
    }
}
