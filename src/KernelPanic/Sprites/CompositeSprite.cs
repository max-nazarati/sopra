using System;
using System.Collections.Generic;
using KernelPanic.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Sprites
{
    internal sealed class CompositeSprite : Sprite
    {
        public List<Sprite> Children { get; } = new List<Sprite>();

        protected override float UnscaledWidth => UnscaledSize.X;
        protected override float UnscaledHeight => UnscaledSize.Y;

        protected override Vector2 UnscaledSize
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

        public override Rectangle Bounds =>
            Children.Union(Position - Origin);

        internal override Color TintColor
        {
            get => Children.Count == 0 ? Color.White : Children[0].TintColor;
            set
            {
                foreach (var child in Children)
                {
                    child.TintColor = value;
                }
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

        protected override void CompleteClone()
        {
            base.CompleteClone();

            for (var i = 0; i < Children.Count; ++i)
            {
                Children[i] = Children[i].Clone();
            }
        }
    }
}
