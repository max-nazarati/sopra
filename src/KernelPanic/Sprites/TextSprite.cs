using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Sprites
{
    internal sealed class TextSprite : Sprite
    {
        internal delegate void SizeChangedDelegate(TextSprite sprite);

        /// <summary>
        /// This event is invoked when the size of this sprite changes because <see cref="Font"/> or
        /// <see cref="Text"/> were changed.
        ///
        /// This event can be used to reposition this sprite or other sprites based on the new size.
        /// </summary>
        internal event SizeChangedDelegate SizeChanged;
        
        private string mText;
        private SpriteFont mFont;

        private SpriteFont Font
        {
            get => mFont;
            set
            {
                if (mFont == value)
                    return;
                mFont = value ?? throw new ArgumentNullException();
                ResetLazySize();
            }
        }

        public string Text
        {
            get => mText;
            set
            {
                if (mText == value)
                    return;
                mText = value ?? throw new ArgumentNullException();
                ResetLazySize();
            }
        }

        public Color TextColor { /*internal*/ private get; set; } = Color.Black;

        public TextSprite(SpriteFont font, string text, float x, float y) : base(x, y)
        {
            mFont = font ?? throw new ArgumentNullException(nameof(font));
            mText = text ?? throw new ArgumentNullException(nameof(text));
            ResetLazySize();
        }
        
        private Lazy<Vector2> mLazySize;
        protected override float UnscaledWidth => Size.X;
        protected override float UnscaledHeight => Size.Y;
        protected override Vector2 UnscaledSize => mLazySize.Value;

        private void ResetLazySize()
        {
            mLazySize = new Lazy<Vector2>(() => Font.MeasureString(Text));
            SizeChanged?.Invoke(this);
        }

        protected override void Draw(SpriteBatch spriteBatch,
            GameTime gameTime,
            Vector2 position,
            float rotation,
            float scale)
        {
            spriteBatch.DrawString(Font,
                Text,
                position,
                TextColor,
                rotation,
                Origin,
                scale,
                SpriteEffects.None,
                1.0f);
        }
    }
}
