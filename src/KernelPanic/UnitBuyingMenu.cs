using System.Linq;
using KernelPanic.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KernelPanic.Entities;
using KernelPanic.Input;
using KernelPanic.Interface;
using KernelPanic.Players;
using KernelPanic.Purchasing;
using KernelPanic.Sprites;
using KernelPanic.Table;
using KernelPanic.Waves;

namespace KernelPanic
{
    internal sealed class UnitBuyingMenu : BuyingMenuOverlay<UnitBuyingMenu.Element>
    {
        internal sealed class Element : IPositioned, IUpdatable, IDrawable
        {
            private readonly PurchaseButton<ImageButton, Unit> mButton;
            private readonly TextSprite mCounterSprite;
            private int mCounter;

            Vector2 IPositioned.Position
            {
                get => mButton.Button.Sprite.Position;
                set
                {
                    var buttonSprite = mButton.Button.Sprite;
                    buttonSprite.Position = value;
                    mCounterSprite.X = value.X - buttonSprite.Width - 8 /* padding between text and button */;
                    mCounterSprite.Y = value.Y + buttonSprite.Height / 2;
                }
            }

            Vector2 IPositioned.Size => mButton.Button.Sprite.Size;

            internal Element(PurchaseButton<ImageButton, Unit> button, SpriteManager spriteManager)
            {
                mCounterSprite = spriteManager.CreateText();
                mCounterSprite.SizeChanged += sprite => sprite.SetOrigin(RelativePosition.CenterRight);
                Reset();

                // When a unit is purchased, increase its counter.
                mButton = button;
                mButton.Action.Purchased += (buyer, resource) => mCounterSprite.Text = (++mCounter).ToString();
                mButton.Button.Sprite.SetOrigin(RelativePosition.TopRight);
            }

            internal void Reset()
            {
                mCounter = 0;
                mCounterSprite.Text = "0";
            }

            public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
            {
                mButton.Draw(spriteBatch, gameTime);
                mCounterSprite.Draw(spriteBatch, gameTime);
            }

            public void Update(InputManager inputManager, GameTime gameTime)
            {
                mButton.Update(inputManager, gameTime);
            }
        }

        private UnitBuyingMenu(Player player, SpriteManager spriteManager, params PurchaseButton<ImageButton, Unit>[] buttons)
            : base(MenuPosition(Lane.Side.Right, spriteManager), player, buttons.Select(b => new Element(b, spriteManager)))
        {
        }

        internal static UnitBuyingMenu Create(WaveManager waveManager, SpriteManager spriteManager)
        {
            PurchaseButton<ImageButton, Unit> CreateButton(Unit unit, AnimatedSprite sprite)
            {
                var action = new PurchasableAction<Unit>(unit);
                action.Purchased += waveManager.Add;
                var button = new ImageButton(spriteManager, sprite.getSingleFrame(spriteManager), 70, 70);
                return new PurchaseButton<ImageButton, Unit>(waveManager.Players.A, action, button);
            }

            var bug = CreateButton(new Bug(spriteManager), spriteManager.CreateBug());
            var trojan = CreateButton(new Trojan(spriteManager), spriteManager.CreateTrojan());
            var firefox = CreateButton(new Firefox(spriteManager), spriteManager.CreateFirefox());
            return new UnitBuyingMenu(waveManager.Players.A, spriteManager, bug, trojan, firefox);
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
