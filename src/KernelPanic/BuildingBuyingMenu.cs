using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KernelPanic.Purchasing;
using KernelPanic.Interface;
using KernelPanic.Entities;
using KernelPanic.Sprites;
using KernelPanic.Input;
using KernelPanic.Data;
using KernelPanic.Players;
using KernelPanic.Table;

namespace KernelPanic
{
    internal sealed class BuildingBuyingMenu : BuyingMenuOverlay<BuildingBuyingMenu.Element, Building>
    {
        internal sealed class Element : IPositioned, IUpdatable, IDrawable
        {
            private readonly ImageButton mButton;
            private readonly TextSprite mCounterSprite;
            private int mCounter;

            Vector2 IPositioned.Position
            {
                get => mButton.Sprite.Position;
                set
                {
                    var buttonSprite = mButton.Sprite;
                    buttonSprite.Position = value;
                    mCounterSprite.X = buttonSprite.Position.X + 72 /* padding between text and button */;
                    mCounterSprite.Y = value.Y + buttonSprite.Height / 2;
                }
            }

            Vector2 IPositioned.Size => mButton.Size;

            internal Element(ImageButton button, SpriteManager spriteManager)
            {
                mCounterSprite = spriteManager.CreateText();
                mCounterSprite.SizeChanged += sprite => sprite.SetOrigin(RelativePosition.CenterLeft);
                Reset();

                // When a unit is purchased, increase its counter.
                mButton = button;
                mButton.Clicked += (btn, input) =>
                {
                    if (true)
                    {
                        mCounterSprite.Text = (++mCounter).ToString();
                    }
                };
                //mButton.Button.Sprite.ScaleToWidth(64);
                mButton.Sprite.SetOrigin(RelativePosition.TopLeft);
            }

            private void Reset()
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

        private BuildingBuyingMenu(Player player, SpriteManager spriteManager, params ImageButton[] buttons)
                : base(MenuPosition(Lane.Side.Left, spriteManager), player, buttons.Select(b => new Element(b, spriteManager)))
        {
        }

        internal static BuildingBuyingMenu Create(Player player,
            BuildingBuyer buildingBuyer,
            SpriteManager spriteManager,
            SoundManager sounds)
        {
            void BuildingBought(Player buyer, Building building)
            {
                sounds.PlaySound(SoundManager.Sound.TowerPlacement);
            }

            ImageButton CreateButton(Building building, ImageSprite sprite)
            {
                var button = new ImageButton(spriteManager, sprite, 70, 70);
                button.Clicked += (btn, input) =>
                {
                    var action = new PurchasableAction<Building>(building);
                    action.Purchased += BuildingBought;
                    buildingBuyer.Building = building;
                    buildingBuyer.BuyAction = action;
                };
                return button;
            } 

            var cursorSprite = spriteManager.CreateCursorShooter();
            cursorSprite.ScaleToWidth(64);
            var cursorShooter = CreateButton(new CursorShooter(spriteManager, sounds),  cursorSprite);

            var wifiSprite = spriteManager.CreateWifiRouter();
            wifiSprite.ScaleToWidth(64);
            var router = CreateButton(new WifiRouter(spriteManager, sounds), wifiSprite);

            var cdSprite = spriteManager.CreateCdThrower();
            cdSprite.ScaleToWidth(64);
            var cdThrower = CreateButton(new CdThrower(spriteManager, sounds), cdSprite);
            
            var antivirusSprite = spriteManager.CreateAntivirus();
            antivirusSprite.ScaleToWidth(64);
            var antivirus = CreateButton(new Antivirus(spriteManager, sounds), antivirusSprite);

            var ventilatorSprite = spriteManager.CreateVentilator();
            cdSprite.ScaleToWidth(64);
            var ventilator = CreateButton(new Ventilator(spriteManager, sounds), ventilatorSprite);
            
            return new BuildingBuyingMenu(player, spriteManager, 
                cursorShooter, router, cdThrower , antivirus, ventilator);
        }

        internal override Dictionary<Type, PurchasableAction<Building>> BuyingActions =>
            null; // throw new NotImplementedException();
    }
}
