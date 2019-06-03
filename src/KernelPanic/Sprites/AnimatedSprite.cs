﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Sprites
{
    internal sealed class AnimatedSprite : Sprite
    {
        private const int DefaultFrameSize = 64;
        private readonly Texture2D mTexture;
        private readonly int mFrameCount;

        /// <summary>
        /// The duration for which each frame is displayed.
        /// </summary>
        private readonly TimeSpan FrameDuration;

        internal Color TintColor { get; set; } = Color.White;

        public override float UnscaledWidth => DefaultFrameSize;
        public override float UnscaledHeight => DefaultFrameSize;

        public AnimatedSprite(Texture2D texture, int x, int y, TimeSpan frameDuration) : base(x, y)
        {
            mTexture = texture;
            mFrameCount = texture.Width / DefaultFrameSize;
            FrameDuration = frameDuration;
        }

        protected override void Draw(SpriteBatch spriteBatch,
            GameTime gameTime,
            Vector2 position,
            float rotation,
            float scale)
        {
            var textureIndex = gameTime.TotalGameTime.Ticks / FrameDuration.Ticks % mFrameCount;
            var sourceRect =
                new Rectangle((int)textureIndex * DefaultFrameSize, 0, DefaultFrameSize, DefaultFrameSize);

            spriteBatch.Draw(mTexture,
                position,
                sourceRect,
                TintColor,
                rotation,
                Origin,
                scale,
                SpriteEffects.None,
                1f);
        }
    }
}