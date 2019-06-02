using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class CompositeSprite : Sprite
    {
        public List<Sprite> Children { get; } = new List<Sprite>();

        internal CompositeSprite(float x, float y) : base(x, y)
        {
        }

        public override float UnscaledWidth => Children.Max(sprite => new float?(sprite.X - sprite.Origin.X + sprite.UnscaledWidth)) ?? 0.0f;
        public override float UnscaledHeight => Children.Max(sprite => new float?(sprite.Y - sprite.Origin.Y + sprite.UnscaledHeight)) ?? 0.0f;

        protected override void Draw(SpriteBatch spriteBatch,
            GameTime gameTime,
            Vector2 position,
            float rotation,
            float scale)
        {
            position -= Origin;
            foreach (var child in Children)
            {
                DrawChild(child, spriteBatch, gameTime, position, rotation, scale);
            }
        }
    }
}
