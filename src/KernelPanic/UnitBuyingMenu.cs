using System;
using System.Collections.Generic;
using System.Reflection;
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
    internal sealed class UnitBuyingMenu : BuyingMenuOverlay<UnitBuyingMenu.Element, Unit>
    {
        internal sealed class Element : IPositioned, IUpdatable, IDrawable
        {
            internal Type UnitType { get; }
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

            internal Element(Type unitType, PurchaseButton<ImageButton, Unit> button, SpriteManager spriteManager)
            {
                UnitType = unitType;
                mCounter = 0;
                mCounterSprite = spriteManager.CreateText("0");
                mCounterSprite.SizeChanged += sprite => sprite.SetOrigin(RelativePosition.CenterRight);

                // When a unit is purchased, increase its counter.
                mButton = button;
                mButton.Action.Purchased += (buyer, resource) => mCounterSprite.Text = (++mCounter).ToString();
                mButton.Button.Sprite.SetOrigin(RelativePosition.TopRight);
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

            internal PurchasableAction<Unit> BuyingAction => mButton.Action;
        }

        private UnitBuyingMenu(Player player, SpriteManager spriteManager, params Element[] elements)
            : base(MenuPosition(Lane.Side.Right, spriteManager), player, elements)
        {
        }

        internal static UnitBuyingMenu Create(WaveManager waveManager, SpriteManager spriteManager)
        {
            Element CreateElement<T>()
            {
                const BindingFlags bindingFlags =
                    BindingFlags.Instance | BindingFlags.CreateInstance | BindingFlags.NonPublic |
                    BindingFlags.DeclaredOnly;
                var unit = (Unit)Activator.CreateInstance(typeof(T), bindingFlags, null, new object[] {spriteManager}, null);
                var button = CreateButton(unit, (AnimatedSprite) unit.Sprite);
                return new Element(typeof(T), button, spriteManager);
            }

            PurchaseButton<ImageButton, Unit> CreateButton(Unit unit, AnimatedSprite sprite)
            {
                var action = new PurchasableAction<Unit>(unit);
                action.Purchased += waveManager.Add;
                var button = new ImageButton(spriteManager, sprite.getSingleFrame(spriteManager), 70, 70);
                return new PurchaseButton<ImageButton, Unit>(waveManager.Players.A, action, button);
            }

            var bug = CreateElement<Bug>();
            var trojan = CreateElement<Trojan>();
            var firefox = CreateElement<Firefox>();
            return new UnitBuyingMenu(waveManager.Players.A, spriteManager, bug, trojan, firefox);
        }

        internal override Dictionary<Type, PurchasableAction<Unit>> BuyingActions
        {
            get
            {
                var dict = new Dictionary<Type, PurchasableAction<Unit>>(Elements.Length);
                foreach (var element in Elements)
                {
                    dict[element.UnitType] = element.BuyingAction;
                }
                return dict;
            }
        }
    }
}
