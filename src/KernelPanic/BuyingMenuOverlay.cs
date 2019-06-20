using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using KernelPanic.Entities;

namespace KernelPanic
{
    class BuyingMenuOverlay
    {
        internal IEnumerable<IDrawable> Drawables{ get; private set; }
        internal IEnumerable<IUpdatable> Updatables { get; private set; }

        protected PurchaseButton<Unit, PurchasableAction<Unit>> CreateTrojanPurchaseButton(SpriteManager spriteManager, Player player)
        {
            var btn = new PurchaseButton<Unit, PurchasableAction<Unit>>(player,
                new PurchasableAction<Unit>((Unit)new Trojan(spriteManager)), spriteManager)
            {
                Button = { Title = "Trojan" }
            };
            var trojanSprite = btn.Button.Sprite;
            trojanSprite.Position = new Vector2(spriteManager.ScreenSize.X - trojanSprite.Width, 90 + trojanSprite.Height);
            return btn;
        }
        protected PurchaseButton<Unit, PurchasableAction<Unit>> CreateFirefoxPurchaseButton(SpriteManager spriteManager, Player player)
        {
            var firefoxButton = new PurchaseButton<Unit, PurchasableAction<Unit>>(player,
                new PurchasableAction<Unit>(Firefox.CreateFirefox(Point.Zero, spriteManager)),
                spriteManager)
            {
                Button = { Title = "Firefox" }
            };
            var sprite = firefoxButton.Button.Sprite;
            sprite.Position = new Vector2(spriteManager.ScreenSize.X - sprite.Width, 90);
            return firefoxButton;
        }
    }
}
