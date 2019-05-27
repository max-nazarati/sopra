using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    public sealed class PatternSprite: Sprite
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        
        public Sprite Sprite { get; set; }

        public PatternSprite(Sprite sprite, float x, float y, int rows, int columns) : base(x, y)
        {
            Rows = rows;
            Columns = columns;
            Sprite = sprite;
        }

        public override float UnscaledWidth => Sprite.UnscaledWidth * Columns;
        public override float UnscaledHeight => Sprite.UnscaledHeight * Rows;
        
        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 offset, float rotation, float scale)
        {
            var patternWidth = scale * Sprite.UnscaledWidth;
            var patternHeight = scale * Sprite.UnscaledHeight;
            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; ++j)
                {
                    Sprite.Draw(spriteBatch,
                        gameTime,
                        Position + offset + new Vector2(j * patternWidth, i * patternHeight),
                        rotation,
                        scale);
                }
            }
        }
    }
}