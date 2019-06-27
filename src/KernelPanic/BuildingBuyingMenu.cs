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
            private readonly PurchaseButton<ImageButton, Building> mButton;
            private readonly TextSprite mCounterSprite;
            private int mCounter;

            Vector2 IPositioned.Position
            {
                get => mButton.Button.Sprite.Position;
                set
                {
                    var buttonSprite = mButton.Button.Sprite;
                    buttonSprite.Position = value;
                    mCounterSprite.X = 0/* padding between text and button */;
                    mCounterSprite.Y = value.Y + buttonSprite.Height / 2;
                }
            }
            Vector2 IPositioned.Size {
                get
                {
                    mButton.Button.Sprite.ScaleToWidth(64);
                    return mButton.Button.Sprite.Size;
                }
            }

            internal Element(PurchaseButton<ImageButton, Building> button, SpriteManager spriteManager)
            {
                mCounterSprite = spriteManager.CreateText();
                mCounterSprite.SizeChanged += sprite => sprite.SetOrigin(RelativePosition.TopLeft);
                Reset();

                // When a unit is purchased, increase its counter.
                mButton = button;
                mButton.Action.Purchased += (buyer, resource) => mCounterSprite.Text = (++mCounter).ToString();
                mButton.Button.Sprite.ScaleToWidth(64);
                mButton.Button.Sprite.SetOrigin(RelativePosition.TopLeft);
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

        private BuildingBuyingMenu(Player player, SpriteManager spriteManager, Entity selection, SoundManager sounds, params PurchaseButton<ImageButton, Building>[] buttons)
                : base(MenuPosition(Lane.Side.Left, spriteManager), player, buttons.Select(b => new Element(b, spriteManager)))
        {
        }

        internal static BuildingBuyingMenu Create(Player player, SpriteManager spriteManager, Entity selection, SoundManager sounds)
        {
            void BuildingBought(Player buyer, Building building)
            {
                if (!buyer.AttackingLane.EntityGraph.HasEntityAt(building.Sprite.Position))
                {
                    sounds.PlaySound(SoundManager.Sound.TowerPlacement);
                    buyer.AttackingLane.BuildingSpawner.Register(building.Clone());
                } else
                {
                    return;
                }
            }

            PurchaseButton<ImageButton, Building> CreateButton(Building unit, ImageSprite sprite)
            {
                var action = new PurchasableAction<Building>(unit);
                action.Purchased += BuildingBought;
                var button = new ImageButton(spriteManager, sprite, 0, 70);
                return new PurchaseButton<ImageButton, Building>(player, action, button);
            }

            var bug = CreateButton(Tower.Create(new Vector2(0, 0), 64, spriteManager, sounds), spriteManager.CreateTower());
            
            return new BuildingBuyingMenu(player, spriteManager, selection, sounds, bug);
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
