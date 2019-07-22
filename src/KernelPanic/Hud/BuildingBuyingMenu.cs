using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KernelPanic.Interface;
using KernelPanic.Entities;
using KernelPanic.Sprites;
using KernelPanic.Input;
using KernelPanic.Data;
using KernelPanic.Entities.Buildings;
using KernelPanic.Table;
using Microsoft.Xna.Framework.Input;
using KernelPanic.Players;

namespace KernelPanic.Hud
{
    internal sealed class BuildingBuyingMenu : BuyingMenuOverlay<BuildingBuyingMenu.Element>
    {
        private Button mSelectedButton;
        private readonly BuildingBuyer mBuildingBuyer;

        internal sealed class Element : IPositioned, IUpdatable, IDrawable
        {
            private readonly TextSprite mInfoText;
            private readonly Player mBuyer;
            internal Button Button { get; }
            internal Building Building { get; }

            Vector2 IPositioned.Position
            {
                get => Button.Sprite.Position;
                set
                {
                    Button.Sprite.Position = value;
                    mInfoText.Position = value;
                    mInfoText.X += 80;
                }
            }

            Vector2 IPositioned.Size => Button.Size;

            internal Element(Building building, SpriteManager spriteManager, Player buyer)
            {
                mBuyer = buyer;
                Building = building;
                const int buttonSize = 70;
                const int iconSize = 64;
                var sprite = building.Sprite.Clone();
                sprite.SetOrigin(RelativePosition.TopLeft);
                sprite.ScaleToWidth(iconSize); // for some reason cable has a size of 100x100
                Button = new ImageButton(spriteManager, (ImageSprite)sprite, buttonSize, buttonSize);
                mInfoText = (TextSprite)building.mInfoText.Clone();
                mInfoText.SetOrigin(RelativePosition.TopLeft);
                mInfoText.Position = sprite.Position;
                mInfoText.X += buttonSize + 5;
            }

            public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
            {
                Button.Draw(spriteBatch, gameTime);
                if (Button.MouseOver)
                {
                    mInfoText.Draw(spriteBatch, gameTime);
                }
            }

            public void Update(InputManager inputManager, GameTime gameTime)
            {
                Button.Enabled = mBuyer.Bitcoins >= Building.Price;
                Button.Update(inputManager, gameTime);
            }
        }

        private BuildingBuyingMenu(BuildingBuyer buildingBuyer, SpriteManager spriteManager, params Element[] elements)
            : base(MenuPosition(Lane.Side.Left, spriteManager), elements)
        {
            mBuildingBuyer = buildingBuyer;
            foreach (var element in Elements)
            {
                element.Button.Clicked += (button, input) =>
                {
                    if (Deselect(button))
                        return;

                    button.ViewPressed = true;
                    mSelectedButton = button;
                    mBuildingBuyer.Building = element.Building;
                };
            }
        }

        internal override void Update(InputManager inputManager, GameTime gameTime)
        {
            base.Update(inputManager, gameTime);
            if (mSelectedButton != null && inputManager.KeyPressed(Keys.Escape))
                Deselect(mSelectedButton);
        }

        private bool Deselect(IDrawable button)
        {
            if (mSelectedButton != null)
                mSelectedButton.ViewPressed = false;

            if (mSelectedButton != button)
                return false;

            mSelectedButton = null;
            mBuildingBuyer.Building = null;
            return true;
        }

        internal static BuildingBuyingMenu Create(BuildingBuyer buildingBuyer,
            SpriteManager spriteManager, Player buyer)
        {
            Element CreateElement<TBuilding>() where TBuilding : Building =>
                new Element(Building.Create<TBuilding>(spriteManager), spriteManager, buyer);

            return new BuildingBuyingMenu(buildingBuyer, spriteManager,
                CreateElement<CursorShooter>(),
                CreateElement<WifiRouter>(),
                CreateElement<CdThrower>(),
                CreateElement<Antivirus>(),
                CreateElement<Ventilator>(),
                CreateElement<ShockField>(),
                CreateElement<Cable>());
        }
    }
}
