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

        protected override float UnscaledWidth => Sprite.Width * Columns;
        public override float UnscaledHeight => Sprite.Height * Rows;
        
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
    }
}