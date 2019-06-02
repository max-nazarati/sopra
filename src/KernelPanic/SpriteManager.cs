﻿using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class SpriteManager
    {
        private static SpriteManager sSprites;
        public ContentManager ContentManager { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }

        internal static SpriteManager Default => sSprites ?? (sSprites = new SpriteManager());

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
            Button,
            HUD
        }

        private SpriteManager()
        {
        }

        private readonly (Image image, Texture2D texture)[] mTextures;
        private readonly (Font font, SpriteFont spriteFont)[] mFonts;

        internal SpriteManager(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            ContentManager = contentManager;
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
                SpriteFont(Font.Button, "buttonFont"),
                SpriteFont(Font.HUD, "HUDFont")
            };
            Array.Sort(mFonts);
        }

        private Texture2D Lookup(Image image)
        {
            var (realImage, texture) = mTextures[(int) image];
            Trace.Assert(realImage == image, $"{nameof(mTextures)} not ordered correctly");
            return texture;
        }

        private SpriteFont Lookup(Font font)
        {
            var (realFont, texture) = mFonts[(int) font];
            Trace.Assert(realFont == font, $"{nameof(mFonts)} not ordered correctly");
            return texture;
        }

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
            var background = new ImageSprite(Lookup(Image.ButtonBackground), 0, 0)
            {
                DestinationRectangle = new Rectangle(0, 0, 250, 70)
            };

            var titleSprite = AutoCenteredTextSprite(Lookup(Font.Button), background);

            return (
                new CompositeSprite(0, 0)
                {
                    Children = {background, titleSprite}
                },
                titleSprite
            );
        }

        internal ImageSprite CreateLaneTile() => new ImageSprite(Lookup(Image.LaneTile), 0, 0);
        internal ImageSprite CreateTower() => new ImageSprite(Lookup(Image.Tower), 0, 0);
        internal ImageSprite CreateProjectile() => new ImageSprite(Lookup(Image.Projectile), 0, 0);

        internal (Sprite Main, TextSprite Left, TextSprite Right, TextSprite Clock) CreateScoreDisplay(Point screenSize, Point powerIndicatorSize, Point clockSize)
        {
            var texture = Lookup(Image.ButtonBackground);
            var font = Lookup(Font.HUD);

            var leftBackground = new ImageSprite(texture, 0, 0)
            {
                DestinationRectangle = new Rectangle(Point.Zero, powerIndicatorSize),
                TintColor = Color.LightBlue
            };
            var rightBackground = new ImageSprite(texture, 0, 0)
            {
                DestinationRectangle = new Rectangle(Point.Zero, powerIndicatorSize),
                TintColor = Color.Black
            };
            var clockBackground = new ImageSprite(texture, 0, 0)
            {
                DestinationRectangle = new Rectangle(Point.Zero, clockSize)
            };

            var leftText = AutoCenteredTextSprite(font, leftBackground);
            var rightText = AutoCenteredTextSprite(font, rightBackground);
            var clockText = AutoCenteredTextSprite(font, clockBackground);

            var left = new CompositeSprite((screenSize.X - powerIndicatorSize.X) / 2.0f - clockSize.X, 0)
            {
                Children = {leftBackground, leftText}
            };
            var right = new CompositeSprite((screenSize.X - clockSize.X) / 2.0f, 0)
            {
                Children = {rightBackground, rightText}
            };
            var clock = new CompositeSprite((screenSize.X - clockSize.X) / 2.0f, 0)
            {
                Children = {clockBackground, clockText}
            };

            var sprite = new CompositeSprite(0, 0)
            {
                Children = {left, right, clock}
            };
            return (sprite, leftText, rightText, clockText);
        }

        /// <summary>
        /// <para>
        /// Creates a <see cref="TextSprite"/> which automatically re-centers itself in <paramref name="container"/>
        /// when its text or font change.
        /// </para>
        /// <para>
        /// When <paramref name="container"/> changes its size this construct moves out of sync.
        /// </para>
        /// </summary>
        /// <param name="font">The font to use for the text sprite.</param>
        /// <param name="container">The container in which to center the new sprite.</param>
        /// <returns>A new <see cref="TextSprite"/></returns>
        private static TextSprite AutoCenteredTextSprite(SpriteFont font, Sprite container)
        {
            var text = new TextSprite(font, "", container.Width / 2, container.Height / 2);
            text.SizeChanged += s => s.Origin = new Vector2(s.Width / 2, s.Height / 2);
            return text;
        }
    }
}
