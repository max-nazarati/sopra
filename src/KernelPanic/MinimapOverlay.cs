using System;
using System.Net;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class MinimapOverlay
    {
        private SpriteManager mSpriteManager;
        private Sprite mSprite;
        private readonly float mRelativeSize; // how much of the screen should be the minimap [0, 1]
        private int mSize;
        private readonly Vector2 mPosition;
        private bool mSizeShouldChange;
        private readonly Color mBackground = Color.DimGray;
        private Color[] mData;
        
        
        internal MinimapOverlay(SpriteManager spriteManager, float relativeSize = 0.3f)
        {
            var screenSizeX = spriteManager.ScreenSize.X;
            var screenSizeY = spriteManager.ScreenSize.Y;
            mRelativeSize = relativeSize;
            mSize = (int)(Math.Min(screenSizeX, screenSizeY) * mRelativeSize);
            mPosition = new Vector2(screenSizeX - mSize, screenSizeY - mSize);
            mSpriteManager = spriteManager;

            // filling the minimap with background color
            mData = new Color[mSize * mSize];
            for (int i = 0; i < mSize * mSize; i++)
            {
                mData[i] = mBackground;
            }
            UpdateTexture();
        }
        
        public void Update(InputManager inputManager, GameTime gameTime)
        {
            UpdateSize();
            UpdateData();
            UpdateTexture();
        }

        private void UpdateSize()
        {
            if (!mSizeShouldChange) { return; }
            var screenSizeX = mSpriteManager.ScreenSize.X;
            var screenSizeY = mSpriteManager.ScreenSize.Y;
            mSize = (int)(Math.Min(screenSizeX, screenSizeY) * mRelativeSize);
        }
        
        private void UpdateData()
        {
            /*
            for (int i = 0; i < mData.Length; i++)
            {
                mData[i] = Color.DimGray;
            }
            */
        }

        private void UpdateTexture()
        {
            mSprite = mSpriteManager.CreateColoredRectangle(mSize, mSize, mData);
            mSprite.Position = mPosition;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mSprite.Draw(spriteBatch, gameTime);
        }
    }
}