using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    class BuyingMenuOverlay
    {
        protected static int mMenuWidth = 50;
        protected static int mMenuHeight = 400;
        protected CompositeSprite mBuyingMenu;
        public IEnumerable<IDrawable> Drawables { get; private set; }
        public IEnumerable<IUpdatable> Updatables { get; private set; }

        internal BuyingMenuOverlay(float x, float y)
        {
            var buyingMenuBackground = new ImageSprite(SpriteManager.Default.LoadImage("Papier"), 0, 0)
            {
                DestinationRectangle = new Rectangle(0, 0, mMenuWidth, mMenuHeight)
            };
            mBuyingMenu = new CompositeSprite(x, y);
            mBuyingMenu.Children.Add(buyingMenuBackground);

        }
        
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mBuyingMenu.Draw(spriteBatch, gameTime);
        }
    }
}
