using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Interface
{
    internal abstract class InterfaceComponent: IPositioned, IBounded, IDrawable, IUpdatable
    {
        internal /*virtual*/ bool Enabled { get; set; } = true;

        internal abstract Sprite Sprite { get; }

        public abstract void Update(InputManager inputManager, GameTime gameTime);

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, gameTime);
        }

        #region IBounded/IPositioned

        public Vector2 Position
        {
            get => Sprite.Position;
            set => Sprite.Position = value;
        }

        public Vector2 Size => Sprite.Size;

        public Rectangle Bounds => Sprite.Bounds;

        #endregion
    }
}
