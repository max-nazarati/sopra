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
        internal enum Direction
        {
            Standing,
            Left,
            Right,
            /*Up,
            Down*/
        }

        internal Direction MovementDirection { get; set; }

        /// <summary>
        /// The duration for which each frame is displayed.
        /// </summary>
        private readonly TimeSpan mFrameDuration;

        /*internal*/ private Color TintColor { get; /*set;*/ } = Color.White;

        protected override float UnscaledWidth => DefaultFrameSize;
        protected override float UnscaledHeight => DefaultFrameSize;

        internal ImageSprite getSingleFrame(SpriteManager spriteManager)
        {
            var rect = new Rectangle(0, 0, DefaultFrameSize, DefaultFrameSize);
            var texcolor = new Color[DefaultFrameSize * DefaultFrameSize];
            mTexture.GetData<Color>(0, rect,  texcolor, 0, DefaultFrameSize * DefaultFrameSize);
            return spriteManager.FrameToImageSprite(texcolor, rect.Width, rect.Height);
        }

        public AnimatedSprite(Texture2D texture, TimeSpan frameDuration)
        {
            mTexture = texture;
            mFrameCount = 1;
            mFrameDuration = frameDuration;
            MovementDirection = Direction.Standing;
            mEffect = SpriteEffects.None;
        }

        protected override void Draw(SpriteBatch spriteBatch,
            GameTime gameTime,
            Vector2 position,
            float rotation,
            float scale)
        {
            switch (MovementDirection)
            {
                case Direction.Standing:
                    mFrameCount = 1;
                    mEffect = SpriteEffects.None;
                    mRow = 0;
                    break;
                case Direction.Left:
                    mFrameCount = 8;
                    mEffect = SpriteEffects.None;
                    mRow = 1;
                    break;
                case Direction.Right:
                    mFrameCount = 8;
                    mEffect = SpriteEffects.FlipHorizontally;
                    mRow = 1;
                    break;
                /*case Direction.Up:
                    mFrameCount = 1;
                    mEffect = SpriteEffects.None;
                    mRow = 2;
                    break;
                case Direction.Down:
                    mFrameCount = 1;
                    mEffect = SpriteEffects.None;
                    mRow = 3;
                    break;*/
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