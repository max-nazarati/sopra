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
            Sprite.SetOrigin(RelativePosition.Center);
        }

        public bool Selected { get; set; }

        internal Rectangle Bounds => new Rectangle((int)Sprite.X-25, 
            (int)Sprite.Y-25, (int)Sprite.Width-4, (int)Sprite.Height);

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
