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

            foreach (var element in Elements)
            {
                element.Position = startPosition;
                startPosition.Y += element.Size.Y;
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
