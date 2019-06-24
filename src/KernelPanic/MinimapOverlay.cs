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
        private readonly Sprite mSprite;
        private readonly float RelativeSize; // how much of the screen should be the minimap [0, 1]
        private int Size;
        private bool mSizeShouldChange;
        private Color[] mMinimapData;
        
        
        internal MinimapOverlay(SpriteManager spriteManager, float relativeSize = 0.3f)
        {
            var screenSizeX = spriteManager.ScreenSize.X;
            var screenSizeY = spriteManager.ScreenSize.Y;
            RelativeSize = relativeSize;
            Size = (int)(Math.Min(screenSizeX, screenSizeY) * RelativeSize);
            
            mSprite = spriteManager.CreateColoredRectangle(Size, Size, Color.DimGray);
            mSprite.Position = new Vector2(screenSizeX - Size, screenSizeY - Size);
            mSpriteManager = spriteManager;

            mMinimapData = new Color[Size * Size];
            for (int i = 0; i < Size * Size; i++)
            {
                mMinimapData[i] = Color.DimGray;
            }
        }
        
        public void Update(InputManager inputManager, GameTime gameTime)
        {
            UpdateSize();
            UpdateData();
        }

        private void UpdateSize()
        {
            if (!mSizeShouldChange) { return; }
            var screenSizeX = mSpriteManager.ScreenSize.X;
            var screenSizeY = mSpriteManager.ScreenSize.Y;
            Size = (int)(Math.Min(screenSizeX, screenSizeY) * RelativeSize);
        }
        
        private void UpdateData()
        {
            // mData
        }

        private void UpdateTexture()
        {
            
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mSprite.Draw(spriteBatch, gameTime);
        }
    }
}