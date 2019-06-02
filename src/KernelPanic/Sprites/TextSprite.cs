﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
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

        public Color TextColor { get; set; } = Color.Black;

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
            SizeChanged?.Invoke(this);
        }

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Draw(spriteBatch, gameTime, Vector2.Zero, 0, 0);
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
                TextColor,
                rotation,
                Origin,
                scale,
                SpriteEffects.None,
                1.0f);
        }
    }
}
