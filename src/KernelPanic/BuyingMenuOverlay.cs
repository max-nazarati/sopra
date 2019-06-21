using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using KernelPanic.Entities;
using KernelPanic.Sprites;
using KernelPanic.Interface;

namespace KernelPanic
{
    class BuyingMenuOverlay
    {
        internal IEnumerable<IDrawable> Drawables{ get; private set; }
        internal IEnumerable<IUpdatable> Updatables { get; private set; }

        protected PurchaseButton<ImageButton, Unit, PurchasableAction<Unit>> CreateTrojanPurchaseButton(SpriteManager spriteManager, Player player)
        {
            var sprite = spriteManager.CreateTrojan();
            var btn = new PurchaseButton<ImageButton, Unit, PurchasableAction<Unit>>(player,
                new PurchasableAction<Unit>(new Trojan(spriteManager)), new ImageButton(spriteManager, 
                sprite.getSingleFrame(spriteManager), 70, 70));
            {
                
            };
            var trojanSprite = btn.Button.Sprite;
            trojanSprite.Position = new Vector2(spriteManager.ScreenSize.X - 2*trojanSprite.Width, 90 + trojanSprite.Height);
            return btn;
        }
        protected PurchaseButton<ImageButton, Unit, PurchasableAction<Unit>> CreateFirefoxPurchaseButton(SpriteManager spriteManager, Player player)
        {
            var sprite = spriteManager.CreateFirefox();
            var firefoxButton = new PurchaseButton<ImageButton, Unit, PurchasableAction<Unit>>(player,
                new PurchasableAction<Unit>(new Firefox(spriteManager)),
                new ImageButton(spriteManager, sprite.getSingleFrame(spriteManager), 70, 70));
            var tmpsprite = firefoxButton.Button.Sprite;
            tmpsprite.Position = new Vector2(spriteManager.ScreenSize.X - 2*tmpsprite.Width, 90);
            return firefoxButton;
        }
    }
}
