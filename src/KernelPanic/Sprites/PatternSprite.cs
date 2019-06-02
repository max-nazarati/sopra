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

        public override float UnscaledWidth => Sprite.Width * Columns;
        public override float UnscaledHeight => Sprite.Height * Rows;
        
        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 offset, float rotation, float scale)
        {
            offset += Position - Origin;
            var patternWidth = scale * Sprite.Width;
            var patternHeight = scale * Sprite.Height;
            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; ++j)
                {
                    Sprite.Draw(spriteBatch,
                        gameTime,
                        offset + new Vector2(j * patternWidth, i * patternHeight),
                        rotation,
                        scale);
                }
            }
        }
    }
}