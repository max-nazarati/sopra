using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    class UnitBuyingMenu : BuyingMenuOverlay
    {
        private Tuple<int, PurchaseButton<Entities.Unit, PurchasableAction<Entities.Unit>>>[] mChoices;
        internal delegate void UnitPurchasedDelegate(Entities.Unit unit);
        internal event UnitPurchasedDelegate UnitPurchased;
        private PurchaseButton<Entities.Unit, PurchasableAction<Entities.Unit>> UnitButton;
        internal void ResetCounts()
        {
        
        }
        internal UnitBuyingMenu(SpriteManager Sprites, Player player)
        {
            UnitButton = new PurchaseButton<Entities.Unit, PurchasableAction<Entities.Unit>>(player,
                new PurchasableAction<Entities.Unit>(Entities.Firefox.CreateFirefox(Point.Zero, Sprites)),
                Sprites)
            {
                Button = { Title = "Firefox" }
            };
            var sprite = UnitButton.Button.Sprite;
            sprite.Position = new Vector2(Sprites.ScreenSize.X - sprite.Width, 90);
            void OnPurchase(Player buyer, Entities.Entity resource)
            {
                resource.Sprite.Position = new Vector2(50*30, 150*3);
                buyer.AttackingLane.EntityGraph.Add(resource);
                UnitButton.Action.ResetResource(Entities.Firefox.CreateFirefox(Point.Zero, Sprites));
            }
            UnitButton.Action.Purchased += OnPurchase;
        }

        internal void Update(Input.InputManager input, GameTime gameTime)
        {
            UnitButton.Update(input, gameTime);
        }
        internal void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            UnitButton.Draw(spriteBatch, gameTime);
        }
    }
}
