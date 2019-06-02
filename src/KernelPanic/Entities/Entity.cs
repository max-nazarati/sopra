using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal abstract class Entity : IPriced
    {
        protected Rectangle mContainerRectangle;
        private Rectangle mOldContainerRectangle;
        //private GraphicsDeviceManager mGraphics;

        internal Sprite Sprite { get; }

        protected Entity(int price, Sprite sprite)
        {
            Price = price;
            Sprite = sprite;
        }
        
        protected Entity(int x, int y, int width, int height, Texture2D texture)
            : this(x, y, width, height, new ImageSprite(texture, x, y))
        {
        }

        protected Entity(int x, int y, int width, int height, Sprite sprite)
        {
            mContainerRectangle = new Rectangle(new Point(x, y), new Point(width, height));
            mOldContainerRectangle = new Rectangle(new Point(x, y), new Point(width, height));
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

        public bool IsColliding(Entity Object)
        {
            return mContainerRectangle.Intersects(Object.mContainerRectangle);
        }

        public void CollisionDetected()
        {
            // moves the object back to the last position
            mContainerRectangle = mOldContainerRectangle;
        }

        public void CooledDownDelegate(CooldownComponent source)
        {
            // TODO: mTexture.SetData(new[] { Color.Blue });
        }

        internal virtual void Update(GameTime gameTime, Matrix invertedViewMatrix)
        {
            // store last position
            mOldContainerRectangle = mContainerRectangle;
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, gameTime);
        }

        public int Price { get; }
        public Currency Currency => Currency.Bitcoin;
    }
}
