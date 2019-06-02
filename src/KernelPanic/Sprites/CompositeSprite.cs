using System.Collections.Generic;
using System.Linq;
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

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Draw(spriteBatch, gameTime, Vector2.Zero, 0.0f, 1.0f);
        }

        internal override void Draw(SpriteBatch spriteBatch,
            GameTime gameTime,
            Vector2 offset,
            float rotation,
            float scale)
        {
            offset += Position - Origin;
            rotation += Rotation;
            scale *= Scale;
            foreach (var child in Children)
            {
                child.Draw(spriteBatch, gameTime, offset, rotation, scale);
            }
        }
    }
}
