using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal class BuildingBuyingMenu : BuyingMenuOverlay
    {
        public delegate Building BuildingSelectedDelegate();
        public event BuildingSelectedDelegate BuildingSelected;
        public BuildingBuyingMenu(float x, float y) : base(x, y)
        {
            var towerLabel = new ImageSprite(SpriteManager.Default.LoadImage("tower"), 0, 0)
            {
                DestinationRectangle = new Rectangle(0, 0, mMenuWidth, mMenuHeight/7)
            };
            mBuyingMenu.Children.Add(towerLabel);
        }
    }
}
