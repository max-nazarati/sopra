using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using KernelPanic.Input;

namespace KernelPanic
{
    internal class BuyingMenuOverlay<TElement> where TElement : IDrawable, IUpdatable
    {
        internal Player Player { get; }
        internal TElement[] Elements { get; }

        protected BuyingMenuOverlay(Player player, TElement[] elements)
        {
            Player = player;
            Elements = elements;
        }

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
