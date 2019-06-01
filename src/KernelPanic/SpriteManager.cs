using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    public sealed class SpriteManager
    {
        private enum Image
        {
            MenuBackground
        }

        private enum Font
        {
            Button
        }

        private readonly (Image image, Texture2D texture)[] mTextures;
        private readonly (Font font, SpriteFont spriteFont)[] mFonts;

        public SpriteManager(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            ContentManager = contentManager;
            GraphicsDevice = graphicsDevice;

            mTextures = new[]
            {
                (Image.MenuBackground, contentManager.Load<Texture2D>("Base"))
            };
            Array.Sort(mTextures);

            mFonts = new[]
            {
                (Font.Button, contentManager.Load<SpriteFont>("buttonFont"))
            };
            Array.Sort(mFonts);
        }

        private Texture2D Lookup(Image image)
        {
            var (realImage, texture) = mTextures[(int) image];
            Trace.Assert(realImage == image, "Texture array not ordered correctly");
            return texture;
        }

        private SpriteFont Lookup(Font font)
        {
            var (realFont, texture) = mFonts[(int) font];
            Trace.Assert(realFont == font, "Texture array not ordered correctly");
            return texture;
        }
        
        public ContentManager ContentManager { get; private set; }
        public GraphicsDevice GraphicsDevice { get; private set; }
        public Texture2D LoadImage(string textureName)
        {
            return ContentManager.Load<Texture2D>(textureName);
        }
        public SpriteFont LoadFont(string fontName)
        {
            return ContentManager.Load<SpriteFont>(fontName);
        }

        public Sprite CreateMenuBackground(Point screenSize)
        {
            var texture = Lookup(Image.MenuBackground);
            var fullRows = screenSize.Y / texture.Height;
            var fullCols = screenSize.X / texture.Width;
            var bottomRem = screenSize.Y - fullRows * texture.Height;
            var rightRem = screenSize.X - fullCols * texture.Width;
            
            var fullTile = new ImageSprite(texture, 0, 0);
            var pattern = new PatternSprite(fullTile, 0, 0, fullRows, fullCols);

            var bottomTile = new ImageSprite(texture, 0, 0)
            {
                SourceRectangle = new Rectangle(0, 0, texture.Width, bottomRem)
            };
            var rightTile = new ImageSprite(texture, 0, 0)
            {
                SourceRectangle = new Rectangle(0, 0, rightRem, texture.Height)
            };
            var cornerTile = new ImageSprite(texture, pattern.Width, pattern.Height)
            {
                SourceRectangle = new Rectangle(0, 0, rightRem, bottomRem)
            };

            return new CompositeSprite(0, 0)
            {
                Children =
                {
                    pattern,
                    new PatternSprite(bottomTile, 0, pattern.Height, 1, fullCols),
                    new PatternSprite(rightTile, pattern.Width, 0, fullRows, 1),
                    cornerTile
                }
            };
        }
    }
}
