using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KernelPanic.Interface;
using KernelPanic.Entities;
using KernelPanic.Sprites;
using KernelPanic.Input;
using KernelPanic.Data;
using KernelPanic.Entities.Buildings;
using KernelPanic.Table;

namespace KernelPanic.Hud
{
    internal sealed class BuildingBuyingMenu : BuyingMenuOverlay<BuildingBuyingMenu.Element>
    {
        internal sealed class Element : IPositioned, IUpdatable, IDrawable
        {
            internal Button Button { get; }
            internal Building Building { get; }

            Vector2 IPositioned.Position
            {
                get => Button.Sprite.Position;
                set => Button.Sprite.Position = value;
            }

            Vector2 IPositioned.Size => Button.Size;

            internal Element(Building building, SpriteManager spriteManager)
            {
                Building = building;
                var sprite = building.Sprite.Clone();
                sprite.SetOrigin(RelativePosition.TopLeft);
                Button = new ImageButton(spriteManager, (ImageSprite)sprite, 70, 70);
            }

            public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
            {
                Button.Draw(spriteBatch, gameTime);
            }

            public void Update(InputManager inputManager, GameTime gameTime)
            {
                Button.Update(inputManager, gameTime);
            }
        }

        private BuildingBuyingMenu(BuildingBuyer buildingBuyer, SpriteManager spriteManager, params Element[] elements)
            : base(MenuPosition(Lane.Side.Left, spriteManager), elements)
        {
            Button selectedButton = null;

            foreach (var element in Elements)
            {
                element.Button.Clicked += (button, input) =>
                {
                    if (selectedButton != null)
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        // ReSharper does not understand the check above.
                        selectedButton.ViewPressed = false;
                    }

                    if (button == selectedButton)
                    {
                        buildingBuyer.SetBuilding(null);
                        selectedButton = null;
                        return;
                    }

                    button.ViewPressed = true;
                    selectedButton = button;
                    buildingBuyer.SetBuilding(element.Building);
                };
            }
        }

        internal static BuildingBuyingMenu Create(BuildingBuyer buildingBuyer,
            SpriteManager spriteManager,
            SoundManager sounds)
        {
            Element CreateElement<TBuilding>() where TBuilding : Building =>
                new Element(Building.Create<TBuilding>(spriteManager, sounds), spriteManager);

            return new BuildingBuyingMenu(buildingBuyer, spriteManager,
                CreateElement<CursorShooter>(),
                CreateElement<WifiRouter>(),
                CreateElement<CdThrower>(),
                CreateElement<Antivirus>(),
                CreateElement<Ventilator>());
        }
    }
}
