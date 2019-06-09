using System;
using System.Collections.Generic;
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

        protected override float UnscaledWidth => UnscaledSize.X;
        public override float UnscaledHeight => UnscaledSize.Y;

        public override Vector2 UnscaledSize
        {
            get
            {
                float minX = 0, minY = 0, maxX = 0, maxY = 0;

                foreach (var child in Children)
                {
                    minX = Math.Min(minX, child.X - child.Origin.X);
                    minY = Math.Min(minY, child.Y - child.Origin.Y);
                    maxX = Math.Max(maxX, child.X - child.Origin.X + child.Width);
                    maxY = Math.Max(maxY, child.Y - child.Origin.Y + child.Height);
                }
                
                return new Vector2(maxX - minX, maxY - minY);
            }
        }

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
