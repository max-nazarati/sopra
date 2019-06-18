using System.Runtime.Serialization;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Sprites
{
    [DataContract]
    internal sealed class AnimatedSprite : Sprite
    {
        private const int DefaultFrameSize = 64;
        private readonly Texture2D mTexture;
        private int mFrameCount;
        private SpriteEffects mEffect;
        private int mRow;

        // specifies row of the sprite sheet
        internal enum Movement
        {
            Standing,
            Left,
            Right,
            Up,
            Down
        }
        internal Movement mMovement;

        /// <summary>
        /// The duration for which each frame is displayed.
        /// </summary>
        private readonly TimeSpan mFrameDuration;

        internal Color TintColor { get; set; } = Color.White;

        protected override float UnscaledWidth => DefaultFrameSize;
        protected override float UnscaledHeight => DefaultFrameSize;

        public AnimatedSprite(Texture2D texture, int x, int y, TimeSpan frameDuration) : base(x, y)
        {
            mTexture = texture;
            mFrameCount = 1;
            mFrameDuration = frameDuration;
            mMovement = Movement.Standing;
            mEffect = SpriteEffects.None;
        }

        protected override void Draw(SpriteBatch spriteBatch,
            GameTime gameTime,
            Vector2 position,
            float rotation,
            float scale)
        {
            switch (mMovement)
            {
                case Movement.Standing:
                    mFrameCount = 1;
                    mEffect = SpriteEffects.None;
                    mRow = 0;
                    break;
                case Movement.Left:
                    mFrameCount = 8;
                    mEffect = SpriteEffects.None;
                    mRow = 1;
                    break;
                case Movement.Right:
                    mFrameCount = 8;
                    mEffect = SpriteEffects.FlipHorizontally;
                    mRow = 1;
                    break;
                case Movement.Up:
                    mFrameCount = 1;
                    mEffect = SpriteEffects.None;
                    mRow = 2;
                    break;
                case Movement.Down:
                    mFrameCount = 1;
                    mEffect = SpriteEffects.None;
                    mRow = 3;
                    break;
            }
            var textureIndex = gameTime.TotalGameTime.Ticks / mFrameDuration.Ticks % mFrameCount;
            var sourceRect =
                new Rectangle((int)textureIndex * DefaultFrameSize, mRow * DefaultFrameSize, DefaultFrameSize, DefaultFrameSize);

            spriteBatch.Draw(mTexture,
                position,
                sourceRect,
                TintColor,
                rotation,
                Origin,
                scale,
                mEffect,
                1f);
        }
    }
}