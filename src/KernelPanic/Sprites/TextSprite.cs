using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class TextSprite : Sprite
    {
        private string mText;
        private SpriteFont mFont;

        private SpriteFont Font
        {
            get => mFont;
            set
            {
                mFont = value ?? throw new ArgumentNullException();
                ResetLazySize();
            }
        }

        public string Text
        {
            get => mText;
            set
            {
                mText = value ?? throw new ArgumentNullException();
                ResetLazySize();
            }
        }

        public TextSprite(SpriteFont font, string text, float x, float y) : base(x, y)
        {
            mFont = font ?? throw new ArgumentNullException(nameof(font));
            mText = text ?? throw new ArgumentNullException(nameof(text));
            ResetLazySize();
        }
        
        private Lazy<Vector2> mLazySize;
        public override float UnscaledWidth => Size.X;
        public override float UnscaledHeight => Size.Y;
        public override Vector2 UnscaledSize => mLazySize.Value;
        
        private void ResetLazySize()
        {
            mLazySize = new Lazy<Vector2>(() => Font.MeasureString(Text));
        }

        internal override void Draw(SpriteBatch spriteBatch,
            GameTime gameTime,
            Vector2 offset,
            float rotation,
            float scale)
        {
            spriteBatch.DrawString(Font,
                Text,
                Position + offset,
                Color.White,
                rotation,
                Origin,
                scale,
                SpriteEffects.None,
                1.0f);
        }
    }
}
