using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    public abstract class InterfaceComponent
    {
        public bool Enabled { get; set; } = true;
        
        public abstract Sprite Sprite { get; }

        public abstract void Update(GameTime gameTime);

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, gameTime);
        }

        /// <summary>
        /// Checks if the mouse is currently positioned inside this component as determined by the rectangle of the component's sprite.
        /// </summary>
        /// <returns><c>True</c> if the mouse is inside this component, <c>False</c> otherwise.</returns>
        protected bool ContainsMouse()
        {
            var mouse = InputManager.Default.MousePosition;
            var origin = Sprite.Position;

            return origin.X <= mouse.X && mouse.X <= origin.X + Sprite.Width
                && origin.Y <= mouse.Y && mouse.Y <= origin.Y + Sprite.Height;
        }
    }
}
