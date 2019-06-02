using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal abstract class Entity : IPriced
    {
        //private GraphicsDeviceManager mGraphics;

        internal Sprite Sprite { get; }

        protected Entity(int price, Sprite sprite)
        {
            Price = price;
            Sprite = sprite;
        }

        /// <summary>
        /// Below, redundant stuff, perhaps?
        /// </summary>
        /// 
        /* To select a unit, left-click on it.
         * Further left-clicks place the unit on different positions.
         * Right-clicks make the unit float to a different position.
         * Use Space to unselect the unit.
         */

        public bool Selected { get; set; }

        internal virtual void Update(GameTime gameTime, Matrix invertedViewMatrix)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, gameTime);
        }

        public int Price { get; }
        public Currency Currency => Currency.Bitcoin;
    }
}
