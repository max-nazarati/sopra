using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KernelPanic.Purchasing;
using KernelPanic.Interface;
using KernelPanic.Entities;
using KernelPanic.Sprites;
using KernelPanic.Input;
using KernelPanic.Data;
using KernelPanic.Table;

namespace KernelPanic
{
    class BuildingBuyingMenu : BuyingMenuOverlay<BuildingBuyingMenu.Element>
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
            Vector2 IPositioned.Size {
                get
                {
                    return mButton.Sprite.Size;
                }
            }

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

        private BuildingBuyingMenu(Player player, SpriteManager spriteManager, params ImageButton[] buttons)
                : base(MenuPosition(Lane.Side.Left, spriteManager), player, buttons.Select(b => new Element(b, spriteManager)))
        {
        }

        internal static BuildingBuyingMenu Create(Player player, SpriteManager spriteManager, Entity selection, SoundManager sounds)
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
                    BuildingBuyer.Building = building;
                    var action = new PurchasableAction<Building>(BuildingBuyer.Building);
                    action.Purchased += BuildingBought;
                    BuildingBuyer.BoughtAction = action;
                };
                return button;
            } 

            var cursorSprite = spriteManager.CreateCursorShooter();
            cursorSprite.ScaleToWidth(64);
            var cursorShooter = CreateButton(Tower.CreateTower(new Vector2(0, 0), 64
                , spriteManager, sounds, StrategicTower.Towers.CursorShooter), cursorSprite);

            var wifiSprite = spriteManager.CreateWifiRouter();
            wifiSprite.ScaleToWidth(64);
            var router = CreateButton(Tower.CreateTower(new Vector2(0, 0), 64, spriteManager, sounds,
                StrategicTower.Towers.WifiRouter), wifiSprite);

            var cdSpite = spriteManager.CreateCdThrower();
            var cdThrower = CreateButton(Tower.CreateTower(Vector2.Zero, 64, spriteManager, sounds,
                StrategicTower.Towers.CdThrower), cdSpite);
            
            return new BuildingBuyingMenu(player, spriteManager, cursorShooter, router, cdThrower);
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
