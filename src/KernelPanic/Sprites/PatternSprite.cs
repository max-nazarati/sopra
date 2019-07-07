using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Sprites
{
    internal sealed class PatternSprite: Sprite
    {
        private int Rows { get; set; }
        private int Columns { get; set; }

        private Sprite Sprite { get; set; }

        internal PatternSprite(Sprite sprite, int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Sprite = sprite;
        }

        protected override float UnscaledWidth => Sprite.Width * Columns;
        protected override float UnscaledHeight => Sprite.Height * Rows;

        internal override Color TintColor
        {
            get => Sprite.TintColor;
            set => Sprite.TintColor = value;
        }

        protected override void Draw(SpriteBatch spriteBatch,
            GameTime gameTime,
            Vector2 position,
            float rotation,
            float scale)
        {
            position -= Origin;
            var patternWidth = scale * Sprite.Width;
            var patternHeight = scale * Sprite.Height;
            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; ++j)
                {
                    DrawChild(Sprite,
                        spriteBatch,
                        gameTime,
                        position + new Vector2(j * patternWidth, i * patternHeight),
                        rotation,
                        scale);
                }
            }
        }

        protected override void CompleteClone()
        {
            base.CompleteClone();
            Sprite = Sprite.Clone();
        }
    }
}