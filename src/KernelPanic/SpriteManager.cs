using System;
using System.Diagnostics;
using System.Linq;
using KernelPanic.Data;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class SpriteManager
    {
        private GraphicsDevice GraphicsDevice { get; }

        private enum Image
        {
            MenuBackground,
            ButtonBackground,
            LaneTile,
            LaneBorder,
            Tower,
            Projectile,
            Arrow,
            Trojan,
            SelectionBorder,
            Firefox,
            FoxJump,
            Base1,
            Base2,
            Skill,
            Bluescreen,
            Settings,
            Antivirus,
            CdThrower,
            Cd,
            Cursor,
            Fan,
            Mouse,
            Router,
            ShockField,
            Wires,
            Bug,
            Nokia,
            Thunderbird,
            Virus,
            StandingFox
        }

        private enum Font
        {
            Button,
            Hud
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
                Texture(Image.ButtonBackground, "button"),
                Texture(Image.LaneTile, "LaneTile"),
                Texture(Image.LaneBorder, "LaneTileBorder"),
                Texture(Image.Tower, "tower"),
                Texture(Image.Projectile, "Projectile"),
                Texture(Image.Arrow, "Arrow"),
                Texture(Image.Trojan, "troupes/trojan"),
                Texture(Image.Firefox, "heroes/firefox"),
                Texture(Image.FoxJump, "heroes/firefox_jumping"),
                Texture(Image.Antivirus, "towers/antivirus"),
                Texture(Image.Base1, "base_1"),
                Texture(Image.Base2, "base_2"),
                Texture(Image.Bluescreen, "heroes/bluescreen"),
                Texture(Image.Cd, "towers/cd"),
                Texture(Image.CdThrower, "towers/cd_thrower"),
                Texture(Image.Cursor, "towers/cursor"),
                Texture(Image.Fan, "towers/fan"),
                Texture(Image.Mouse, "towers/mouse"),
                Texture(Image.Router, "towers/router"),
                Texture(Image.Settings, "heroes/settings"),
                Texture(Image.ShockField, "towers/shock_field"),
                Texture(Image.Skill, "skill_tile"),
                Texture(Image.Wires, "towers/wires"),
                Texture(Image.Bug, "troupes/bug"),
                Texture(Image.Nokia, "troupes/nokia"),
                Texture(Image.Thunderbird, "troupes/thunderbird"),
                Texture(Image.Virus, "troupes/virus"),
                Texture(Image.StandingFox, "heroes/firefox_standing"),
                (Image.SelectionBorder, CreateSelectionBorderTexture(Color.LightBlue))
            };
            Array.Sort(mTextures);

            mFonts = new[]
            {
                SpriteFont(Font.Button, "ButtonFont"),
                SpriteFont(Font.Hud, "HUDFont")
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

        internal ImageSprite FrameToImageSprite(Color[] colors, int width, int height)
        {
            var texture = new Texture2D(GraphicsDevice, width, height);
            texture.SetData<Color>(colors);
            return new ImageSprite(texture);
        }

        internal ImageSprite CreateStandingFox()
        {
            var texture  = Lookup(Image.StandingFox);
            return new ImageSprite(texture);
        }

        internal TextSprite CreateText()
        {
            return new TextSprite(Lookup(Font.Button));
        }

        internal Sprite CreateMenuBackground()
        {
            var texture = Lookup(Image.MenuBackground);
            var sprite = new ImageSprite(texture);
            sprite.DestinationRectangle = new Rectangle(Point.Zero, ScreenSize);
            return sprite;
            /*var fullRows = ScreenSize.Y / texture.Height;
            var fullCols = ScreenSize.X / texture.Width;
            var bottomRem = ScreenSize.Y - fullRows * texture.Height;
            var rightRem = ScreenSize.X - fullCols * texture.Width;

            var fullTile = new ImageSprite(texture);
            var pattern = new PatternSprite(fullTile, fullRows, fullCols);

            var bottomTile = new ImageSprite(texture)
            {
                SourceRectangle = new Rectangle(0, 0, texture.Width, bottomRem)
            };
            var rightTile = new ImageSprite(texture)
            {
                SourceRectangle = new Rectangle(0, 0, rightRem, texture.Height)
            };
            var cornerTile = new ImageSprite(texture)
            {
                X = pattern.Width,
                Y = pattern.Height,
                SourceRectangle = new Rectangle(0, 0, rightRem, bottomRem)
            };

            return new CompositeSprite
            {
                Children =
                {
                    pattern,
                    new PatternSprite(bottomTile, 1, fullCols)
                    {
                        X = 0,
                        Y = pattern.Height
                    },
                    new PatternSprite(rightTile, fullRows, 1)
                    {
                        X = pattern.Width,
                        Y = 0
                    },
                    cornerTile
                }
            };*/
        }

        internal (Sprite, ImageSprite, TextSprite) CreateTextButton()
        {
            var background = new ImageSprite(Lookup(Image.ButtonBackground))
            {
                DestinationRectangle = new Rectangle(0, 0, 250, 70)
            };

            var titleSprite = AutoCenteredTextSprite(Lookup(Font.Button), background);

            return (
                new CompositeSprite
                {
                    Children = { background, titleSprite }
                },
                background,
                titleSprite
                
            );
        }

        internal (Sprite, ImageSprite) CreateImageButton(ImageSprite image, int width=250, int height=70)
        {
            var background = new ImageSprite(Lookup(Image.ButtonBackground))
            {
                DestinationRectangle = new Rectangle(0, 0, width, height)
            };
            return (
                new CompositeSprite {Children = {background, image}},
                background
            );
        }

        internal ImageSprite CreateLaneTile() => new ImageSprite(Lookup(Image.LaneTile));
        internal ImageSprite CreateLaneBorder() => new ImageSprite(Lookup(Image.LaneBorder));
        internal ImageSprite CreateTower() => new ImageSprite(Lookup(Image.Tower));
        internal ImageSprite CreateProjectile() => new ImageSprite(Lookup(Image.Projectile));

        internal (Sprite Main, TextSprite Left, TextSprite Right, TextSprite Clock) CreateScoreDisplay(Point powerIndicatorSize, Point clockSize)
        {
            var texture = Lookup(Image.ButtonBackground);
            var font = Lookup(Font.Hud);

            var leftBackground = new ImageSprite(texture)
            {
                DestinationRectangle = new Rectangle(Point.Zero, powerIndicatorSize),
                TintColor = Color.LightBlue
            };
            var rightBackground = new ImageSprite(texture)
            {
                DestinationRectangle = new Rectangle(Point.Zero, powerIndicatorSize),
                TintColor = Color.LightSalmon
            };
            var clockBackground = new ImageSprite(texture)
            {
                DestinationRectangle = new Rectangle(Point.Zero, clockSize)
            };

            var leftText = AutoCenteredTextSprite(font, leftBackground);
            var rightText = AutoCenteredTextSprite(font, rightBackground);
            var clockText = AutoCenteredTextSprite(font, clockBackground);

            var left = new CompositeSprite
            {
                X = (ScreenSize.X - clockSize.X) / 2f,
                Y = 0,
                Children = {leftBackground, leftText}
            };
            left.SetOrigin(RelativePosition.TopRight);

            var right = new CompositeSprite
            {
                X = (ScreenSize.X + clockSize.X) / 2f,
                Y = 0,
                Children = {rightBackground, rightText}
            };
            right.SetOrigin(RelativePosition.TopLeft);
            
            var clock = new CompositeSprite
            {
                X = ScreenSize.X / 2f,
                Y = 0,
                Children = {clockBackground, clockText}
            };
            clock.SetOrigin(RelativePosition.CenterTop);

            var sprite = new CompositeSprite
            {
                Children = {left, right, clock}
            };
            return (sprite, leftText, rightText, clockText);
        }

        internal AnimatedSprite CreateTrojan() =>
            new AnimatedSprite(Lookup(Image.Trojan), new TimeSpan(0, 0, 0, 0, 300));

        internal AnimatedSprite CreateFirefox() =>
            new AnimatedSprite(Lookup(Image.Firefox), new TimeSpan(0, 0, 0, 0, 100));

        internal AnimatedSprite CreateFirefoxJump() =>
            new AnimatedSprite(Lookup(Image.FoxJump), new TimeSpan(0, 0, 0, 0, 100));

        internal ImageSprite CreateColoredSquare(Color color)
        {
            var texture = new Texture2D(GraphicsDevice, 1, 1);
            texture.SetData(new[] {color});
            return new ImageSprite(texture);
        }

        private const int SelectionBorderThickness = 12;
        private Texture2D CreateSelectionBorderTexture(Color color)
        {
            const int line = Grid.KachelSize + 2 * SelectionBorderThickness; 
            var texture = new Texture2D(GraphicsDevice, line, line);

            var lineIdx = 0;
            var data = new Color[texture.Width * texture.Height];

            // Fill top and bottom border.
            for (var i = 0; i < SelectionBorderThickness; ++i, ++lineIdx)
            {
                for (var j = 0; j < line; ++j)
                {
                    // Multiply lineIdx with this because we skip over multiple lines.
                    data[j + line * (lineIdx + Grid.KachelSize + SelectionBorderThickness)] = color;
                    data[j + line * lineIdx] = color;
                }
            }

            // Fill rows in between.
            for (var i = 0; i < Grid.KachelSize; ++i, ++lineIdx)
            {
                // Fill the left and right border.
                for (var j = 0; j < SelectionBorderThickness; ++j)
                {
                    // Multiply only lineIdx with line, because we move inside on line.
                    data[j + line * lineIdx + Grid.KachelSize + SelectionBorderThickness] = color;
                    data[j + line * lineIdx] = color;
                }

                // Fill in between.
                for (var j = 0; j < Grid.KachelSize; ++j)
                    data[lineIdx * line + SelectionBorderThickness + j] = Color.Transparent;
            }

            texture.SetData(data);
            return texture;
        }

        internal ImageSprite CreateJumpIndicator()
        {
            var sprite = new ImageSprite(Lookup(Image.Arrow));
            sprite.ScaleToHeight(300);
            sprite.SetOrigin(RelativePosition.CenterBottom);
            return sprite;
        }
        
        internal ImageSprite CreateSelectionBorder()
        {
            var sprite = new ImageSprite(Lookup(Image.SelectionBorder));
            sprite.SetOrigin(RelativePosition.Center);
            return sprite;
        }

        private Texture2D CreateCircleTexture(int radius, Color color)
        {
            var radiusSquared = radius * radius;
            var diameter = 2 * radius;
            var data = Enumerable.Repeat(Color.Transparent, diameter * diameter).ToArray();

            // This algorithm is based on https://en.wikipedia.org/wiki/Midpoint_circle_algorithm#Variant_with_integer-based_arithmetic.
            //
            // We only have to calculate the coordinates for the first octant, the others octants are just mirror-images.

            var x = radius - 1;
            var y = 0;

            void Plot()
            {
                data[(radius + x) * diameter + radius + y] = color;
                data[(radius + x) * diameter + radius - y] = color;
                data[(radius - x) * diameter + radius - y] = color;
                data[(radius - x) * diameter + radius + y] = color;

                data[(radius + y) * diameter + radius + x] = color;
                data[(radius + y) * diameter + radius - x] = color;
                data[(radius - y) * diameter + radius - x] = color;
                data[(radius - y) * diameter + radius + x] = color;
            }

            while (true)
            {
                // Plot the current point.
                Plot();

                // We have reached 45°.
                if (x == y)
                    break;

                // Y always increases.
                ++y;
                
                // Calculate the radius error for the two cases  a) decreasing x  b) keeping x
                var error1 = Math.Abs((x - 1) * (x - 1) + y * y - radiusSquared);
                var error2 = Math.Abs(x * x + y * y - radiusSquared);

                if (error1 < error2)
                    --x;
            }

            var texture = new Texture2D(GraphicsDevice, diameter, diameter);
            texture.SetData(data);
            return texture;
        }

        internal ImageSprite CreateTowerRadiusIndicator(float radius)
        {
            if (Math.Abs(radius) < 0.00001)
                return null;

            var sprite = new ImageSprite(CreateCircleTexture((int) radius, Color.Green));
            sprite.SetOrigin(RelativePosition.Center);
            return sprite;
        }

        internal Point ScreenSize => GraphicsDevice.Viewport.Bounds.Size;

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
            var text = new TextSprite(font)
            {
                X = container.Width / 2,
                Y = container.Height / 2
            };
            text.SizeChanged += s => s.Origin = new Vector2(s.Width / 2, s.Height / 2);
            return text;
        }


    }
}
