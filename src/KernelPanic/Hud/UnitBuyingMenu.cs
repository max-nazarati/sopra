﻿using System;
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

namespace KernelPanic.Hud
{
    internal sealed class UnitBuyingMenu : BuyingMenuOverlay<UnitBuyingMenu.Element>
    {
        internal sealed class Element : IPositioned, IUpdatable, IDrawable
        {
            private int mCounter;
            internal Type UnitType { get; }
            private readonly PurchaseButton<ImageButton, Unit> mButton;
            private readonly TextSprite mCounterSprite;
            private readonly TextSprite mInfoText;

            Vector2 IPositioned.Position
            {
                get => mButton.Button.Sprite.Position;
                set
                {
                    var buttonSprite = mButton.Button.Sprite;
                    buttonSprite.Position = value;

                    if (mCounterSprite != null)
                    {
                        mCounterSprite.X = value.X - buttonSprite.Width - 8 /* padding between text and button */;
                        mCounterSprite.Y = value.Y + buttonSprite.Height / 2;
                    }

                    mInfoText.Position = value;
                    mInfoText.X -= 80;
                }
            }

            Vector2 IPositioned.Size => mButton.Button.Size;

            internal Element(int count, Type unitType, PurchaseButton<ImageButton, Unit> button, SpriteManager spriteManager)
            {
                UnitType = unitType;
                mCounter = count;

                if (!unitType.IsSubclassOf(typeof(Hero)))
                {
                    mCounterSprite = spriteManager.CreateText();
                    mCounterSprite.SizeChanged += sprite => sprite.SetOrigin(RelativePosition.CenterRight);
                    mCounterSprite.Text = count.ToString();
                }

                // When a unit is purchased, increase its counter.
                mButton = button;
                mButton.Action.Purchased += PurchasedUnit;
                mButton.Button.Sprite.SetOrigin(RelativePosition.TopRight);

                var price = button.Action.Price;
                mInfoText = spriteManager.CreateText("Preis: " + price);
                mInfoText.SetOrigin(RelativePosition.TopRight);
                mInfoText.X -= 175;
            }

            private void PurchasedUnit(Player buyer, Unit resource)
            {
                buyer.UpdateHeroCount(resource.GetType(), +1);

                // Only increment the counter if the buyer is the active player.
                if (buyer.Select(mCounterSprite, null) is TextSprite sprite)
                {
                    sprite.Text = (++mCounter).ToString();
                }
            }

            public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
            {
                mButton.Draw(spriteBatch, gameTime);
                mCounterSprite?.Draw(spriteBatch, gameTime);

                if (mButton.Button.MouseOver)
                {
                    mInfoText.Draw(spriteBatch, gameTime);
                }
            }

            public void Update(InputManager inputManager, GameTime gameTime)
            {
                mButton.PossiblyEnabled = mButton.Player.ValidHeroPurchase(UnitType);
                mButton.Update(inputManager, gameTime);
            }

            internal PurchasableAction<Unit> BuyingAction => mButton.Action;
        }

        private UnitBuyingMenu(SpriteManager spriteManager, params Element[] elements)
            : base(MenuPosition(Lane.Side.Right, spriteManager), elements)
        {
        }

        internal static UnitBuyingMenu Create(WaveManager waveManager, SpriteManager spriteManager)
        {
            var player = waveManager.Players.A;
            PurchaseButton<ImageButton, Unit> CreateButton<TUnit>() where TUnit : Unit
            {
                var unit = Unit.Create<TUnit>(spriteManager);
                // Can we have units with an other sprite type than an animated sprite?
                var image = ((AnimatedSprite)unit.Sprite).GetSingleFrame();
                var action = new PurchasableAction<Unit>(unit);
                action.Purchased += waveManager.Add;
                var button = new ImageButton(spriteManager, image, 70, 70);
                return new PurchaseButton<ImageButton, Unit>(player, action, button);
            }

            Element CreateElement<TUnit>() where TUnit : Unit
            {
                var button = CreateButton<TUnit>();
                return new Element(waveManager.CurrentUnitCount<TUnit>(player), typeof(TUnit), button, spriteManager);
            }

            return new UnitBuyingMenu(spriteManager, 
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
