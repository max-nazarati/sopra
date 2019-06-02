using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Sprites
{
    internal class AnimatedSprite : Sprite
    {
        private int mTextureIndex = 0;
        private Rectangle mRectangle;
        private CooldownComponent mTimer;
        private Texture2D mImage;
        private int mFrames;
        private const int mPixel = 64;
        private Rectangle mDestination;

        public override float UnscaledWidth => throw new NotImplementedException();

        public override float UnscaledHeight => throw new NotImplementedException();

        public AnimatedSprite(Texture2D image, int x, int y, int width, int height) : base(x, y)
        {
            mImage = image;
            mRectangle = new Rectangle(0, 0, mPixel, mPixel);
            mFrames = image.Width / mPixel;
            mTimer = new CooldownComponent(new TimeSpan(0, 0, 0, 0, 1000/mFrames));
            mTimer.CooledDown += CooledDownDelegate;
            mDestination = new Rectangle(x, y, width, height);
        }

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mRectangle.X = mPixel * mTextureIndex;
            spriteBatch.Draw(mImage, mDestination, mRectangle, Color.White);
        }

        public void CooledDownDelegate(CooldownComponent source)
        {
            mTextureIndex = (mTextureIndex + 1) % mFrames;
            source.Reset();
        }

        public void Update(GameTime time)
        {
            mTimer.Update(time);
        }

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 offset, float rotation, float scale)
        {
            throw new NotImplementedException();
        }
    }
}
