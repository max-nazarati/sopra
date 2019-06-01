using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Sprites
{
    internal class AnimatedSprite : Sprite
    {
        private List<Texture2D> mTextures;
        private int mTextureIndex = 0;
        private Rectangle mRectangle;
        private CooldownComponent mTimer;

        public override float UnscaledWidth => throw new NotImplementedException();

        public override float UnscaledHeight => throw new NotImplementedException();

        public AnimatedSprite(List<Texture2D> Textures, int x, int y, int width, int height) : base(x, y)
        {
            mTextures = Textures;
            mTimer = new CooldownComponent(new TimeSpan(0, 0, 0, 0, 250));
            mTimer.CooledDown += CooledDownDelegate;
            mRectangle = new Rectangle(x, y, width, height);
        }

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(mTextures[mTextureIndex], mRectangle, Color.White);
        }

        public void CooledDownDelegate(CooldownComponent source)
        {
            mTextureIndex = (mTextureIndex + 1) % mTextures.Count;
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
