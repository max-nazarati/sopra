using System;
using System.Runtime.Serialization;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Entities
{
    [DataContract]
    [KnownType(typeof(Unit))]
    [KnownType(typeof(Building))]

    internal abstract class Entity : IPriced, IBounded
    {
        internal Sprite Sprite { get; private set; }

        protected Entity(int price, Sprite sprite)
        {
            Price = price;
            Sprite = sprite;
            Sprite.SetOrigin(RelativePosition.Center);
        }

        public bool Selected { get; set; }

        public Rectangle Bounds => Sprite.Bounds;

        internal virtual void Update(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            if (inputManager.MousePressed(InputManager.MouseButton.Left))
            {
                Selected = Bounds.Contains(inputManager.TranslatedMousePosition);
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
