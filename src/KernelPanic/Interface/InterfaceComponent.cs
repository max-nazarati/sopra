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

        /// <summary>
        /// Checks if the mouse is currently positioned inside this component as determined by the rectangle of the component's sprite.
        /// </summary>
        /// <returns><c>True</c> if the mouse is inside this component, <c>False</c> otherwise.</returns>
        protected bool ContainsMouse(InputManager inputManager)
        {
            var mouse = inputManager.TranslatedMousePosition;
            var origin = Sprite.Position - Sprite.Origin;

            return origin.X <= mouse.X && mouse.X <= origin.X + Sprite.Width
                && origin.Y <= mouse.Y && mouse.Y <= origin.Y + Sprite.Height;
        }
    }
}
