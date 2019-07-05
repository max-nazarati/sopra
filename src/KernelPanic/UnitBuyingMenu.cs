using System;
using System.Collections.Generic;
using KernelPanic.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KernelPanic.Entities;
using KernelPanic.Entities.Units;
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

            Vector2 IPositioned.Size => mButton.Button.Size;

            internal Element(Type unitType, PurchaseButton<ImageButton, Unit> button, SpriteManager spriteManager)
            {
                UnitType = unitType;
                mCounter = 0;
                mCounterSprite = spriteManager.CreateText("0");
                mCounterSprite.SizeChanged += sprite => sprite.SetOrigin(RelativePosition.CenterRight);

                // When a unit is purchased, increase its counter.
                mButton = button;
                mButton.Action.Purchased += PurchasedUnit;
                mButton.Button.Sprite.SetOrigin(RelativePosition.TopRight);
            }

            private void PurchasedUnit(IPlayerDistinction buyer, Unit resource)
            {
                // Only increment the counter if the buyer is the active player.
                if (buyer.Select(mCounterSprite, null) is TextSprite sprite)
                {
                    sprite.Text = (++mCounter).ToString();
                }
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
            PurchaseButton<ImageButton, Unit> CreateButton<TUnit>() where TUnit : Unit
            {
                var unit = Unit.Create<TUnit>(spriteManager);
                // Can we have units with an other sprite type than an animated sprite?
                var image = ((AnimatedSprite)unit.Sprite).GetSingleFrame();
                var action = new PurchasableAction<Unit>(unit);
                action.Purchased += waveManager.Add;
                var button = new ImageButton(spriteManager, image, 70, 70);
                return new PurchaseButton<ImageButton, Unit>(waveManager.Players.A, action, button);
            }

            Element CreateElement<TUnit>() where TUnit : Unit
            {
                var button = CreateButton<TUnit>();
                return new Element(typeof(TUnit), button, spriteManager);
            }

            return new UnitBuyingMenu(waveManager.Players.A, spriteManager, 
                CreateElement<Bug>(),
                CreateElement<Virus>(),
                CreateElement<Trojan>(),
                CreateElement<Thunderbird>(),
                CreateElement<Nokia>(),
                CreateElement<Firefox>(),
                CreateElement<Settings>(),
                CreateElement<Bluescreen>());
        }

        internal Dictionary<Type, PurchasableAction<Unit>> BuyingActions
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
