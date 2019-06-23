using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KernelPanic.Entities;
using KernelPanic.Input;
using KernelPanic.Interface;
using KernelPanic.Purchasing;
using KernelPanic.Sprites;

namespace KernelPanic
{
    internal sealed class UnitBuyingMenu : BuyingMenuOverlay<UnitBuyingMenu.Element>
    {
        internal sealed class Element : IDrawable, IUpdatable
        {
            private readonly PurchaseButton<ImageButton, Unit> mButton;
            private readonly TextSprite mCounterSprite;
            private int mCounter;

            internal Element(PurchaseButton<ImageButton, Unit> button, SpriteManager spriteManager)
            {
                // TODO: Set position of mCounterSprite.
                mCounterSprite = spriteManager.CreateText("0");

                // When a unit is purchased, increase its counter.
                mButton = button;
                mButton.Action.Purchased += (buyer, resource) => mCounterSprite.Text = (++mCounter).ToString();
            }

            internal void Reset()
            {
                mCounter = 0;
                mCounterSprite.Text = "0";
            }

            public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
            {
                mButton.Draw(spriteBatch, gameTime);
            }

            public void Update(InputManager inputManager, GameTime gameTime)
            {
                mButton.Update(inputManager, gameTime);
            }
        }

        private UnitBuyingMenu(Player player, SpriteManager spriteManager, params PurchaseButton<ImageButton, Unit>[] buttons)
            : base(player, buttons.Select(b => new Element(b, spriteManager)).ToArray())
        {
        }

        internal static UnitBuyingMenu Create(Player player, SpriteManager spriteManager)
        {
            PurchaseButton<ImageButton, Unit> CreateButton(Unit unit, AnimatedSprite sprite)
            {
                var action = new PurchasableAction<Unit>(unit);
                var button = new ImageButton(spriteManager, sprite.getSingleFrame(spriteManager), 70, 70);
                return new PurchaseButton<ImageButton, Unit>(player, action, button);
            }

            var bug = CreateButton(new Bug(spriteManager), spriteManager.CreateBug());
            var trojan = CreateButton(new Trojan(spriteManager), spriteManager.CreateTrojan());
            var firefox = CreateButton(new Firefox(spriteManager), spriteManager.CreateFirefox());
            return new UnitBuyingMenu(player, spriteManager, bug, trojan, firefox);
        }

        /// <summary>
        /// Resets the counter of each element to zero.
        /// </summary>
        internal void ResetCounts()
        {
            foreach (var element in Elements)
            {
                element.Reset();
            }
        }
    }
}
