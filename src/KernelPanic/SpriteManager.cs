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
        public GraphicsDevice GraphicsDevice { get; }

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
            StandingFox,
            VectorArrow,
            Pause
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
                Texture(Image.VectorArrow, "vector_arrow"),
                Texture(Image.Pause, "pause"),
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

        internal ImageSprite CreatePause()
        {
            var texture = Lookup(Image.Pause);
            return new ImageSprite(texture);
        }

        internal TextSprite CreateText(string text = "")
        {
            return new TextSprite(Lookup(Font.Button), text);
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
                DestinationRectangle = new Rectangle(-width + (int)image.Width +2, -height + (int)image.Height, width, height)
            };
            return (
                new CompositeSprite {Children = {background, image}},
                background
            );
        }

        internal ImageSprite CreateLaneTile() => new ImageSprite(Lookup(Image.LaneTile));
        internal ImageSprite CreateLaneBorder() => new ImageSprite(Lookup(Image.LaneBorder));
        internal ImageSprite CreateTower() => new ImageSprite(Lookup(Image.Tower));
        
        internal ImageSprite CreateWifiRouter() => new ImageSprite(Lookup(Image.Tower));
        
        internal ImageSprite CreateVentilator() => new ImageSprite(Lookup(Image.Tower));
        
        internal ImageSprite CreateAntivirus() => new ImageSprite(Lookup(Image.Antivirus));
        
        internal ImageSprite CreateCDThrower() => new ImageSprite(Lookup(Image.CdThrower));
        internal ImageSprite CreateProjectile() => new ImageSprite(Lookup(Image.Projectile));
        
        internal ImageSprite CreateCursorProjectile() => new ImageSprite(Lookup(Image.Cursor));
        internal ImageSprite CreateVectorArrow()
        {
            var sprite = new ImageSprite(Lookup(Image.VectorArrow));
            sprite.ScaleToHeight(0.4f * Grid.KachelSize);
            sprite.SetOrigin(RelativePosition.CenterBottom);
            return sprite;
        }

        internal (Sprite Main, TextSprite Left, TextSprite LeftMoney, TextSprite Right, TextSprite RightMoney, TextSprite Clock) CreateScoreDisplay(Point powerIndicatorSize, Point clockSize)
        {
            var texture = Lookup(Image.ButtonBackground);
            var font = Lookup(Font.Hud);
            var pauseTexture = Lookup(Image.Pause);
            var secondLine = new Point(0, powerIndicatorSize.Y);

            var leftBackground = new ImageSprite(texture)
            {
                DestinationRectangle = new Rectangle(Point.Zero, powerIndicatorSize),
                TintColor = Color.LightBlue
            };
            var leftMoneyBackground = new ImageSprite(texture)
            {
                DestinationRectangle = new Rectangle(secondLine, powerIndicatorSize)
            };
            var rightBackground = new ImageSprite(texture)
            {
                DestinationRectangle = new Rectangle(Point.Zero, powerIndicatorSize),
                TintColor = Color.LightSalmon
            };
            var rightMoneyBackground = new ImageSprite(texture)
            {
                DestinationRectangle = new Rectangle(secondLine, powerIndicatorSize)
            };
            var clockBackground = new ImageSprite(texture)
            {
                DestinationRectangle = new Rectangle(Point.Zero, clockSize)
            };

            var leftText = AutoCenteredTextSprite(font, leftBackground);
            var leftMoneyText = AutoCenteredTextSprite(font, leftMoneyBackground);
            leftMoneyText.Y = (float)(1.5 * powerIndicatorSize.Y);
            var rightText = AutoCenteredTextSprite(font, rightBackground);
            var rightMoneyText = AutoCenteredTextSprite(font, rightMoneyBackground);
            rightMoneyText.Y = (float)(1.5 * powerIndicatorSize.Y);
            var clockText = AutoCenteredTextSprite(font, clockBackground);

            var left = new CompositeSprite
            {
                X = (ScreenSize.X - clockSize.X) / 2f,
                Y = 0,
                Children = { leftBackground, leftText, leftMoneyBackground, leftMoneyText }
            };
            left.SetOrigin(RelativePosition.TopRight);

            var right = new CompositeSprite
            {
                X = (ScreenSize.X + clockSize.X) / 2f,
                Y = 0,
                Children = { rightBackground, rightText, rightMoneyBackground, rightMoneyText }
            };
            right.SetOrigin(RelativePosition.TopLeft);

            var middle = new CompositeSprite
            {
                X = ScreenSize.X / 2f,
                Y = 0,
                Children = { clockBackground, clockText }
            };
            middle.SetOrigin(RelativePosition.CenterTop);

            var sprite = new CompositeSprite
            {
                Children = { left, right, middle }
            };
            return (sprite, leftText, leftMoneyText, rightText, rightMoneyText, clockText);
        }

        #region Troupes

        internal AnimatedSprite CreateTrojan() =>
            new AnimatedSprite(Lookup(Image.Trojan), new TimeSpan(0, 0, 0, 0, 300));
        
        internal AnimatedSprite CreateNokia() =>
            new AnimatedSprite(Lookup(Image.Nokia), new TimeSpan(0, 0, 0, 0, 300));
        
        internal AnimatedSprite CreateThunderbird() =>
            new AnimatedSprite(Lookup(Image.Thunderbird), new TimeSpan(0, 0, 0, 0, 300));
        
        internal AnimatedSprite CreateVirus() =>
            new AnimatedSprite(Lookup(Image.Virus), new TimeSpan(0, 0, 0, 0, 300));

        internal AnimatedSprite CreateBug() =>
            new AnimatedSprite(Lookup(Image.Bug), new TimeSpan(0, 0, 0, 0, 300));
        
        #endregion

        #region Heroes

        internal AnimatedSprite CreateFirefox() =>
            new AnimatedSprite(Lookup(Image.Firefox), new TimeSpan(0, 0, 0, 0, 100));

        internal AnimatedSprite CreateFirefoxJump() =>
            new AnimatedSprite(Lookup(Image.FoxJump), new TimeSpan(0, 0, 0, 0, 100));

        internal AnimatedSprite CreateSettings() =>
            new AnimatedSprite(Lookup(Image.Settings), new TimeSpan(0, 0, 0, 0, 100));

        internal AnimatedSprite CreateBluescreen() =>
            new AnimatedSprite(Lookup(Image.Bluescreen), new TimeSpan(0, 0, 0, 0, 100));
        
        #endregion

        #region Selection
        
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

        internal ImageSprite CreateSelectionBorder()
        {
            var sprite = new ImageSprite(Lookup(Image.SelectionBorder));
            sprite.SetOrigin(RelativePosition.Center);
            return sprite;
        }
        
        #endregion
        
        #region Base

        internal Sprite CreateBases(Vector2 upperLeft, Vector2 size)
        {
            var base1 = new ImageSprite(Lookup(Image.Base1)) { Y = 0 };
            var base2 = new ImageSprite(Lookup(Image.Base2)) { Y = size.Y };
            
            base1.ScaleToWidth(size.X);
            base2.ScaleToWidth(size.X);
            base1.SetOrigin(RelativePosition.TopLeft);
            base2.SetOrigin(RelativePosition.BottomLeft);
            base2.SpriteEffect = SpriteEffects.FlipHorizontally;

            return new CompositeSprite
            {
                Position = upperLeft,
                Children = {base1, base2}
            };
        }
        
        #endregion
        
        #region Indicator

        #region Tower
        
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
        
        #endregion

        #region Hero Abilities
        
        internal ImageSprite CreateJumpIndicator()
        {
            var sprite = new ImageSprite(Lookup(Image.Arrow));
            sprite.SetOrigin(RelativePosition.CenterBottom);
            return sprite;
        }

        internal ImageSprite CreateEmpIndicator(float radius)
        {
            if (Math.Abs(radius) < 0.00001)
                return null;
            var sprite = new ImageSprite(CreateCircleTexture((int) radius, Color.Blue));
            return sprite;
        }
        
        internal ImageSprite CreateHealIndicator(float radius)
        {
            if (Math.Abs(radius) < 0.00001)
                return null;
            var sprite = new ImageSprite(CreateCircleTexture((int) radius, Color.YellowGreen));
            return sprite;
        }
        
        #endregion
        
        #endregion

        internal ImageSprite CreateColoredPixel(Color color)
        {
            var texture = new Texture2D(GraphicsDevice, 1, 1);
            texture.SetData(new [] { color });
            return new ImageSprite(texture);
        }

        internal ImageSprite CreateColoredRectangle(int width, int height, Color color)
        {
            var texture = new Texture2D(GraphicsDevice, width, height);
            var data = new Color[width * height];
            for(var pixel=0; pixel<data.Count();pixel++)
            {
                data[pixel] = color;
            }
            texture.SetData(data);
            return new ImageSprite(texture);
        }
        
        internal ImageSprite CreateColoredRectangle(int width, int height, Color[] color)
        {
            if (color.Length != width * height)
            {
                throw new Exception("not enough colors to fill the rectangle");
            }
            var texture = new Texture2D(GraphicsDevice, width, height);
            var data = new Color[width * height];
            for(var pixel=0; pixel<data.Count();pixel++)
            {
                data[pixel] = color[pixel];
            }
            texture.SetData(data);
            return new ImageSprite(texture);
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
