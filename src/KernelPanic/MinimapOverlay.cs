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
        private readonly float RelativeMinimapSize; // how much of the screen should be the minimap [0, 1]
        private int MinimapSize;
        internal MinimapOverlay(SpriteManager spriteManager, float relativeMinimapSize = 0.3f)
        {
            RelativeMinimapSize = relativeMinimapSize;
            var screenSizeX = spriteManager.ScreenSize.X;
            var screenSizeY = spriteManager.ScreenSize.Y;
            MinimapSize = (int)(Math.Min(screenSizeX, screenSizeY) * RelativeMinimapSize);
            
            mSprite = spriteManager.CreateColoredRectangle(MinimapSize, MinimapSize, Color.DimGray);
            mSprite.Position = new Vector2(0, screenSizeY - MinimapSize);
            mSpriteManager = spriteManager;
            
        }
        
        public void Update(InputManager inputManager, GameTime gameTime)
        {
            // throw new System.NotImplementedException();
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mSprite.Draw(spriteBatch, gameTime);
        }
    }
}