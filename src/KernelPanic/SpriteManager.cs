using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class SpriteManager
    {
        private enum Image
        {
            MenuBackground,
            ButtonBackground,
            LaneTile,
            Tower,
            Projectile
        }

        private enum Font
        {
            Button
        }

        private readonly (Image image, Texture2D texture)[] mTextures;
        private readonly (Font font, SpriteFont spriteFont)[] mFonts;

        internal SpriteManager(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;

            (Image, Texture2D) Texture(Image image, string name) => (image, contentManager.Load<Texture2D>(name));
            (Font, SpriteFont) SpriteFont(Font font, string name) => (font, contentManager.Load<SpriteFont>(name));

            mTextures = new[]
            {
                Texture(Image.MenuBackground, "Base"),
                Texture(Image.ButtonBackground, "Papier"),
                Texture(Image.LaneTile, "LaneTile"),
                Texture(Image.Tower, "tower"),
                Texture(Image.Projectile, "Projectile")
            };
            Array.Sort(mTextures);

            mFonts = new[]
            {
                SpriteFont(Font.Button, "buttonFont")
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

        internal GraphicsDevice GraphicsDevice { get; }

        internal Sprite CreateMenuBackground(Point screenSize)
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

        internal (Sprite, TextSprite) CreateButton()
        {
            var texture = Lookup(Image.ButtonBackground);
            var background = new ImageSprite(texture, 0, 0)
            {
                DestinationRectangle = new Rectangle(0, 0, 250, 70)
            };

            var titleSprite = new TextSprite(
                Lookup(Font.Button),
                "",
                background.Width / 2,
                background.Height / 2);
            titleSprite.SizeChanged += sprite => sprite.Origin = new Vector2(sprite.Width / 2, sprite.Height / 2);

            return (
                new CompositeSprite(0, 0)
                {
                    Children = {background, titleSprite}
                },
                titleSprite
            );
        }

        internal ImageSprite CreateLaneTile() => new ImageSprite(Lookup(Image.LaneTile), 0, 0);
        internal ImageSprite CreateTower() => new ImageSprite(Lookup(Image.Tower), 0, 0) {Scale = 0.8f};
        internal ImageSprite CreateProjectile() => new ImageSprite(Lookup(Image.Projectile), 0, 0);
    }
}
