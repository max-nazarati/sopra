using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using KernelPanic.Input;
using KernelPanic.Table;

namespace KernelPanic
{
    internal abstract class BuyingMenuOverlay<TElement>
        where TElement : class, IPositioned, IUpdatable, IDrawable
    {
        internal TElement[] Elements { get; }

        protected BuyingMenuOverlay(Vector2 startPosition, IEnumerable<TElement> elements)
        {
            Elements = elements.ToArray();

            int i = 1;
            foreach (var element in Elements)
            {
                element.Position = startPosition;
                // calculate spacing between buttons
                // add bigger space after first five units to seperate heroes from troupes
                startPosition.Y += (i == 5 && element is UnitBuyingMenu.Element) ? element.Size.Y + 20 : element.Size.Y;
                i++;
            }
        }

        protected static Vector2 MenuPosition(Lane.Side side, SpriteManager spriteManager) =>
            new Vector2(
                side == Lane.Side.Left
                    ? InputManager.ScreenBorderDistance
                    : spriteManager.ScreenSize.X - InputManager.ScreenBorderDistance,
                InputManager.ScreenBorderDistance);

        internal void Update(InputManager inputManager, GameTime gameTime)
        {
            foreach (var element in Elements)
            {
                element.Update(inputManager, gameTime);
            }
        }

        internal void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var element in Elements)
            {
                element.Draw(spriteBatch, gameTime);
            }
        }
    }
}
