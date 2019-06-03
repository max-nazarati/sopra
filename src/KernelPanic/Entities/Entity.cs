using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal abstract class Entity : IPriced
    {
        internal Sprite Sprite { get; }

        protected Entity(int price, Sprite sprite)
        {
            Price = price;
            Sprite = sprite;
        }

        public bool Selected { get; set; }

        internal virtual void Update(PositionProvider positionProvider, GameTime gameTime, Matrix invertedViewMatrix)
        {
            var input = InputManager.Default;
            if (input.MousePressed(InputManager.MouseButton.Left))
            {
                Selected = Sprite.Contains(Vector2.Transform(input.MousePosition.ToVector2(), invertedViewMatrix));
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, gameTime);
        }

        public int Price { get; }
        public Currency Currency => Currency.Bitcoin;
    }
}
