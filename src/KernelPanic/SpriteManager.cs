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
            MenuWithoutText,
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
            Cable,
            CdThrower,
            Cd,
            Cursor,
            WifiProjectile,
            FanPropeller,
            Mouse,
            Router,
            ShockField,
            Bug,
            Nokia,
            Thunderbird,
            Virus,
            StandingFox,
            VectorArrow,
            Pause,
            ScoreBackground,
            BackgroundTile1,
            BackgroundTile2,
            BackgroundTile3,
            LaneMiddle1,
            LaneMiddle2,
            LaneMiddle3,
            LaneTop1,
            LaneTop2,
            LaneTop3,
            LaneTopRight,
            LaneTopRightCorner,
            BitcoinLogo,
            Umbrella
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
                Texture(Image.MenuWithoutText, "menu"),
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
                Texture(Image.Cable, "towers/wires"),
                Texture(Image.Cd, "towers/cd"),
                Texture(Image.CdThrower, "towers/cd_thrower"),
                Texture(Image.Cursor, "towers/cursor"),
                Texture(Image.WifiProjectile, "towers/WifiProjectile"),
                Texture(Image.FanPropeller, "towers/fanPropeller"),
                Texture(Image.Mouse, "towers/mouse"),
                Texture(Image.Router, "towers/router"),
                Texture(Image.Settings, "heroes/settings"),
                Texture(Image.ShockField, "towers/shock_field"),
                Texture(Image.Skill, "skill_tile"),
                Texture(Image.Bug, "troupes/bug"),
                Texture(Image.Nokia, "troupes/nokia"),
                Texture(Image.Thunderbird, "troupes/thunderbird"),
                Texture(Image.Virus, "troupes/virus"),
                Texture(Image.StandingFox, "heroes/firefox_standing"),
                Texture(Image.VectorArrow, "vector_arrow"),
                Texture(Image.Pause, "pause"),
                Texture(Image.ScoreBackground, "score_background"),
                Texture(Image.BackgroundTile1, "tiles/background_1"),
                Texture(Image.BackgroundTile2, "tiles/background_2"),
                Texture(Image.BackgroundTile3, "tiles/background_3"),
                Texture(Image.LaneMiddle1, "tiles/lane_middle_1"),
                Texture(Image.LaneMiddle2, "tiles/lane_middle_2"),
                Texture(Image.LaneMiddle3, "tiles/lane_middle_3"),
                Texture(Image.LaneTop1, "tiles/lane_top_1"),
                Texture(Image.LaneTop2, "tiles/lane_top_2"),
                Texture(Image.LaneTop3, "tiles/lane_top_3"),
                Texture(Image.LaneTopRight, "tiles/lane_top_right"),
                Texture(Image.LaneTopRightCorner, "tiles/lane_corner_top_right"),
                Texture(Image.BitcoinLogo, "bitcoinLogo"),
                Texture(Image.Umbrella, "towers/umbrella"),
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

        internal ImageSprite CreatePause()
        {
            var texture = Lookup(Image.Pause);
            return new ImageSprite(texture) {DestinationRectangle = new Rectangle(0, 0, 40, 40)};
        }

        internal TextSprite CreateText(string text = "")
        {
            return new TextSprite(Lookup(Font.Button), text);
        }

        internal Sprite CreateMenuBackground()
        {
            var texture = Lookup(Image.MenuBackground);
            var sprite = new ImageSprite(texture) {DestinationRectangle = new Rectangle(Point.Zero, ScreenSize)};
            return sprite;
        }

        internal Sprite CreateMenuBackgroundWithoutText()
        {
            var texture = Lookup(Image.MenuWithoutText);
            var sprite = new ImageSprite(texture) { DestinationRectangle = new Rectangle(Point.Zero, ScreenSize) };
            return sprite;
        }

        internal (Sprite, ImageSprite, TextSprite) CreateTextButton(int width, int height)
        {
            var background = new ImageSprite(Lookup(Image.ButtonBackground))
            {
                DestinationRectangle = new Rectangle(0, 0, width, height)
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
                DestinationRectangle = new Rectangle(-width + (int)image.Width + (int)(width - image.Width) / 2,
                    -height + (int)image.Height + (int)(height - image.Height) / 2, width, height)
            };
            return (
                new CompositeSprite {Children = {background, image}},
                background
            );
        }

        #region Lane

        internal ImageSprite CreateLaneTile() => new ImageSprite(Lookup(Image.LaneTile));
        internal ImageSprite CreateLaneBorder() => new ImageSprite(Lookup(Image.LaneBorder));

        private CompositeSprite CreateTopLaneRow(int columns, int tileSize, bool bottom = false)
        {
            var sprite = new CompositeSprite();
            var random = new Random();
            // add tiles
            var texture1 = Lookup(Image.LaneTop1);
            var texture2 = Lookup(Image.LaneTop2);
            var texture3 = Lookup(Image.LaneTop3);
            
            for (var x = 0; x < columns; x++)
            {
                // randomly choose image for tile
                var number = random.Next(1, 3);
                var texture = number == 1 ? texture1 : number == 2 ? texture2 : texture3;
                var tile = new ImageSprite(texture);

                if (x == 0 || x == columns - 1)
                {
                    tile = new ImageSprite(Lookup(Image.LaneTopRight));
                    if (x == 0 && !bottom)
                    {
                        tile.Rotation = -(float)(Math.PI / 2);
                    }
                    if (x == columns - 1 && bottom)
                    {
                        tile.Rotation -= (float)Math.PI / 2;
                    }
                }
                // scale and position tile
                tile.ScaleToWidth(tileSize);
                tile.Position = new Vector2(x * tileSize + tileSize / 2, (float)(tileSize * 0.5));
                tile.SetOrigin(RelativePosition.Center);
                if (bottom)
                {
                    tile.Rotation += (float)Math.PI;
                }
                sprite.Children.Add(tile);
            }
            return sprite;
        }

        private CompositeSprite CreateBorderLaneRow(Lane.Side side, int columns, int laneWidth, int tileSize, bool bottom = false)
        {
            var random = new Random();
            var middle = CreateMiddleLaneRow(laneWidth, tileSize, random);
            var border = CreateTopLaneRow(columns - laneWidth + 1, tileSize, bottom);
            var corner = new ImageSprite(Lookup(Image.LaneTopRightCorner));
            corner.SetOrigin(RelativePosition.Center);
            corner.ScaleToWidth(tileSize);
            corner.Y = (float)(tileSize * 0.5);
            if (side == Lane.Side.Left)
            {
                border.X = (laneWidth - 1) * tileSize;
                corner.X = (laneWidth - 1) * tileSize + tileSize / 2;
                if (bottom)
                {
                    corner.Rotation += (float)Math.PI;
                }
            }
            else
            {
                middle.X = (laneWidth - 2) * tileSize;
                corner.X = (columns-laneWidth) * tileSize + tileSize / 2;
                corner.Rotation -= (float)(Math.PI / 2);
            }
            if (bottom)
            {
                corner.Rotation -= (float)(Math.PI / 2);
            }
            var sprite = new CompositeSprite
            {
                Children = { middle, border, corner }
            };
            return sprite;
        }

        private CompositeSprite CreateMiddleLaneRow(int columns, int tileSize, Random random)
        {
            var sprite = new CompositeSprite();

            var texture1 = Lookup(Image.LaneMiddle1);
            var texture2 = Lookup(Image.LaneMiddle2);
            var texture3 = Lookup(Image.LaneMiddle3);

            for (var x = 0; x < columns; x++)
            {
                var number = random.Next(0, 30);
                var texture = number < 25 ? texture2 : number < 28 ? texture1 : texture3;
                var tile = new ImageSprite(texture);
                number = random.Next(0, 3);
                tile.Rotation = (float)(number * Math.PI / 2);

                // replace edges
                if (x == 0 || x == columns - 1)
                {
                    var edge1 = Lookup(Image.LaneTop1);
                    var edge2 = Lookup(Image.LaneTop2);
                    var edge3 = Lookup(Image.LaneTop3);
                    texture = number == 1 ? edge1 : number == 2 ? edge2 : edge3;
                    tile = new ImageSprite(texture);
                    if (x == columns - 1)
                    {
                        tile.Rotation = (float)(Math.PI / 2);
                    }
                    else if (x == 0)
                    {
                        tile.Rotation = (float)(3 * Math.PI / 2);
                    }
                }

                // scale and position tile
                tile.ScaleToWidth(tileSize);
                tile.Position = new Vector2(x * tileSize + tileSize / 2, (float)(tileSize * 0.5));
                tile.SetOrigin(RelativePosition.Center);
                sprite.Children.Add(tile);
            }
            return sprite;
        }

        internal CompositeSprite CreateLaneTop(Lane.Side side, int laneWidth, int columns, int tileSize)
        {
            var top = CreateTopLaneRow(columns, tileSize);
            var border = CreateBorderLaneRow(side, columns, laneWidth, tileSize, true);
            border.Y = (laneWidth - 1) * tileSize;
            var sprite = new CompositeSprite
            {
                Children = { top, border }
            };
            var random = new Random();
            for (var y = 1; y < laneWidth-1; y++)
            {
                var row = CreateMiddleLaneRow(columns, tileSize, random);
                row.Y = y * tileSize;
                sprite.Children.Add(row);
            }
            

            return sprite;
        }

        internal CompositeSprite CreateLaneMiddle(int laneWidth, int rows, int tileSize)
        {
            var sprite = new CompositeSprite();
            var random = new Random();
            for (var y = 0; y < rows - 2 * laneWidth; y++)
            {
                var row = CreateMiddleLaneRow(laneWidth, tileSize, random);
                row.Y = y * tileSize;
                sprite.Children.Add(row);
            }
            return sprite;
        }

        internal CompositeSprite CreateLaneBottom(Lane.Side side, int laneWidth, int columns, int tileSize)
        {
            var bottom = CreateTopLaneRow(columns, tileSize, true);
            bottom.Y = (laneWidth - 1) * tileSize;
            var border = CreateBorderLaneRow(side, columns, laneWidth, tileSize);
            var sprite = new CompositeSprite
            {
                Children = { bottom, border }
            };
            var random = new Random();
            for (var y = 1; y < laneWidth-1; y++)
            {
                var row = CreateMiddleLaneRow(columns, tileSize, random);
                row.Y = y * tileSize;
                sprite.Children.Add(row);
            }
            return sprite;
        }

        internal CompositeSprite CreateBoardBackground(Rectangle bounds, int tileSize)
        {
            var rows = bounds.Height / 100;
            var columns = bounds.Width / 100;
            var composite = new CompositeSprite();
            var random = new Random();
            var texture1 = Lookup(Image.BackgroundTile1);
            var texture2 = Lookup(Image.BackgroundTile2);
            var texture3 = Lookup(Image.BackgroundTile3);

            // generate all tiles
            for (var x = 0; x < columns; x++)
            {
                for (var y = 0; y < rows; y++)
                {
                    // randomly choose image for tile
                    var number = random.Next(0, 30);
                    var texture = number < 15 ? texture1 : number < 26 ? texture2 : texture3;
                    var tile = new ImageSprite(texture);
                    tile.ScaleToWidth(tileSize);

                    // rotate tile randomly around its center
                    tile.Position = new Vector2(bounds.X + x * 100 + 50, bounds.Y + y * 100 + 50);
                    tile.SetOrigin(RelativePosition.Center);
                    number = random.Next(0, 3);
                    tile.Rotation = (float)(number * Math.PI / 2);

                    composite.Children.Add(tile);
                }
            }
            return composite;
        }

        #endregion

        #region Buildings

        internal ImageSprite CreateTowerLevelOne()
        {
            var sprite = new ImageSprite(CreateCircleTexture(5, Color.Gold));
            sprite.SetOrigin(RelativePosition.Center);
            return sprite;
        }
        
        internal ImageSprite CreateTowerLevelTwo()
        {
            var sprite = new ImageSprite(CreateCircleTexture(5, Color.Orange));
            sprite.SetOrigin(RelativePosition.Center);
            return sprite;
        }

        internal ImageSprite CreateWifiRouter() => new ImageSprite(Lookup(Image.Router));
        internal ImageSprite CreateVentilator() => new ImageSprite(Lookup(Image.FanPropeller));
        internal ImageSprite CreateAntivirus() => new ImageSprite(Lookup(Image.Antivirus)) {Scale = 1.5f};
        internal ImageSprite CreateCable() => new ImageSprite(Lookup(Image.Cable));
        internal ImageSprite CreateShockField() => new ImageSprite(Lookup(Image.ShockField));
        internal ImageSprite CreateCdThrower() => new ImageSprite(Lookup(Image.CdThrower));
        internal ImageSprite CreateCursorShooter() => new ImageSprite(Lookup(Image.Mouse));

        #endregion

        #region Projectiles

        internal ImageSprite CreateCdProjectile()
        {
            var sprite = new ImageSprite(Lookup(Image.Cd));
            sprite.SetOrigin(RelativePosition.Center);
            sprite.ScaleToWidth(40);
            return sprite;
        }

        internal ImageSprite CreateWifiProjectile()
        {
            var sprite = new ImageSprite(Lookup(Image.WifiProjectile))
            {
                DestinationRectangle = new Rectangle(0, 0, 40, 3)
            };
            sprite.SetOrigin(RelativePosition.TopRight);
            //sprite.ScaleToWidth(40);
            return sprite;
        }

        internal ImageSprite CreateCursorProjectile()
        {
            var sprite = new ImageSprite(Lookup(Image.Cursor));
            sprite.SetOrigin(RelativePosition.Center);
            sprite.ScaleToWidth(20);
            return sprite;
        }

        internal ImageSprite CreateUmbrellaProjectile()
        {
            var sprite = new ImageSprite(Lookup(Image.Umbrella));
            sprite.SetOrigin(RelativePosition.Center);
            sprite.ScaleToWidth(50);
            return sprite;
        }

        #endregion

        internal ImageSprite CreateVectorArrow()
        {
            var sprite = new ImageSprite(Lookup(Image.VectorArrow));
            sprite.ScaleToHeight(0.4f * Grid.KachelSize);
            sprite.SetOrigin(RelativePosition.CenterBottom);
            return sprite;
        }

        internal (Sprite Main, TextSprite Left, TextSprite LeftMoney, TextSprite LeftEP, TextSprite Right
            , TextSprite RightMoney, TextSprite RightEP, TextSprite Clock, TextSprite DefeatedWavesByHuman
            , TextSprite DefeatedWavesByComputer) CreateScoreDisplay()
        {
            const float scale = 1.8f;
            const float hudWidth = scale * 600;
            const float moneyWidth = scale * 60;
            const float hudHeight = scale * 44;
            const float padding = scale * 5;
            const float topPadding = scale * 8;
            var font = Lookup(Font.Hud);
            
            var leftBoarder = (int)((float)ScreenSize.X / 2 - hudWidth / 2);

            var leftMoneyText = new TextSprite(font, "00000 $")
            {
                Position = new Vector2(padding * 25, topPadding)
            };
            
            var leftEpText = new TextSprite(font, "000 EP")
            {
                Position = new Vector2(moneyWidth * 2 + padding*15, topPadding)
            };
            
            var leftText = new TextSprite(font, "000%")
            {
                Position = new Vector2(hudWidth / 2 - 10 * padding, topPadding),
                TintColor = Color.DarkBlue
            };
            
            var rightMoneyText = new TextSprite(font, "00000$")
            {
                Position = new Vector2(hudWidth - padding * 26, topPadding)
            };
            rightMoneyText.SetOrigin(RelativePosition.TopRight);
            
            var rightEpText = new TextSprite(font, "000 EP")
            {
                Position = new Vector2(hudWidth - 2 * moneyWidth - padding*12, topPadding)
            };
            rightEpText.SetOrigin(RelativePosition.TopRight);
            
            var rightText = new TextSprite(font, "000%")
            {
                Position = new Vector2(hudWidth / 2 + 10 * padding, topPadding),
                TintColor = Color.DarkRed
            };
            
            rightText.SetOrigin(RelativePosition.TopRight);
            
            var clockText = new TextSprite(font, "00:00:00")
            {
                Position = new Vector2(hudWidth / 2, hudHeight / 2 + topPadding)
            };
            clockText.SetOrigin(RelativePosition.CenterTop);
            
            var defeatedWavesByHumanText = new TextSprite(font, "00:00:00")
            {
                Position = new Vector2(75, topPadding)
            };
            defeatedWavesByHumanText.SetOrigin(RelativePosition.CenterTop);
            
            var defeatedWavesByComputerText = new TextSprite(font, "00:00:00")
            {
                Position = new Vector2(hudWidth-150, topPadding)
            };
            defeatedWavesByComputerText.SetOrigin(RelativePosition.CenterTop);

            var hudSprite = new ImageSprite(Lookup(Image.ScoreBackground));
            hudSprite.DestinationRectangle = new Rectangle(hudSprite.Position.ToPoint()
                , new Point((int)hudWidth,(int)hudHeight));
            var bitcoinSpriteLeft = new ImageSprite(Lookup(Image.BitcoinLogo))
            {
                Position = new Vector2(padding * 25 + leftMoneyText.Width/1.7f + 4, 12)
            };
            bitcoinSpriteLeft.ScaleToHeight(23);
            
            var bitcoinSpriteRight = new ImageSprite(Lookup(Image.BitcoinLogo))
            {
                Position = new Vector2(hudWidth - padding * 24 - rightMoneyText.Width/1.7f, 12)
            };
            bitcoinSpriteRight.ScaleToHeight(23);
            
            var sprite = new CompositeSprite
            {
                Children = { hudSprite, leftText, leftMoneyText, leftEpText, rightText, rightMoneyText, rightEpText
                    , clockText, defeatedWavesByHumanText, defeatedWavesByComputerText, bitcoinSpriteLeft, bitcoinSpriteRight },
                Position = new Vector2(leftBoarder, 0)
            };
            return (sprite, leftText, leftMoneyText, leftEpText, rightText, rightMoneyText, rightEpText, clockText
                , defeatedWavesByHumanText, defeatedWavesByComputerText);
        }

        #region Troupes

        internal AnimatedSprite CreateTrojan() =>
            new AnimatedSprite(Lookup(Image.Trojan), TimeSpan.FromMilliseconds(300));
        
        internal AnimatedSprite CreateNokia() =>
            new AnimatedSprite(Lookup(Image.Nokia), TimeSpan.FromMilliseconds(300));
        
        internal AnimatedSprite CreateThunderbird() =>
            new AnimatedSprite(Lookup(Image.Thunderbird), TimeSpan.FromMilliseconds(300));
        
        internal AnimatedSprite CreateVirus() =>
            new AnimatedSprite(Lookup(Image.Virus), TimeSpan.FromMilliseconds(300));

        internal AnimatedSprite CreateBug() =>
            new AnimatedSprite(Lookup(Image.Bug), TimeSpan.FromMilliseconds(300));
        
        #endregion

        #region Heroes

        internal AnimatedSprite CreateFirefox() =>
            new AnimatedSprite(Lookup(Image.Firefox), TimeSpan.FromMilliseconds(100));

        internal AnimatedSprite CreateSettings() =>
            new AnimatedSprite(Lookup(Image.Settings), TimeSpan.FromMilliseconds(100));

        internal AnimatedSprite CreateBluescreen() =>
            new AnimatedSprite(Lookup(Image.Bluescreen), TimeSpan.FromMilliseconds(100));
        
        #endregion

        #region Borders

        /// <summary>
        /// Creates a texture of the given <paramref name="size"/> which is transparent save for the a border with the
        /// given <paramref name="thickness"/>. Note that the border is inside <paramref name="size"/>.
        /// </summary>
        /// <param name="size">The size of the resulting texture.</param>
        /// <param name="thickness">The thickness of the border.</param>
        /// <param name="color">The color of the border.</param>
        /// <returns>A transparent texture with a border.</returns>
        private Texture2D CreateBorderTexture(Point size, int thickness, Color color)
        {
            var data = new Color[size.X * size.Y];

            // Fill top and bottom border.
            for (var row = 0; row < thickness; ++row)
            {
                for (var col = 0; col < size.X; ++col)
                {
                    data[col + size.X * row] = color;
                    data[col + size.X * (size.Y - row - 2)] = color;
                }
            }

            // Fill rows in between.
            for (var row = thickness; row < size.Y - thickness - 1; ++row)
            {
                // Fill the left and right border.
                for (var col = 0; col < thickness; ++col)
                {
                    data[col + size.X * row] = color;
                    data[size.X - 1 - col + size.X * row] = color;
                }

                // Fill in between.
                for (var col = thickness; col < size.X - thickness; ++col)
                    data[col + size.X * row] = Color.Transparent;
            }

            var texture = new Texture2D(GraphicsDevice, size.X, size.Y);
            texture.SetData(data);
            return texture;
        }

        private Texture2D CreateSelectionBorderTexture(Color color)
        {
            const int thickness = 12;
            var size = new Point(Grid.KachelSize + 2 * thickness);
            return CreateBorderTexture(size, thickness, color);
        }

        internal ImageSprite CreateSelectionBorder()
        {
            var sprite = new ImageSprite(Lookup(Image.SelectionBorder));
            sprite.SetOrigin(RelativePosition.Center);
            return sprite;
        }

        internal ImageSprite CreateHitBoxBorder(Point size)
        {
            return new ImageSprite(CreateBorderTexture(size, 1, Color.Red));
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
                if (x <= y)
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

        internal ImageSprite CreateEmpIndicatorRange(int abilityRange)
        {
            if (Math.Abs(abilityRange) < 0.00001)
                return null;
            var sprite = new ImageSprite(CreateCircleTexture(abilityRange, Color.Blue));
            sprite.SetOrigin(RelativePosition.Center);
            return sprite;
        }
        
        public ImageSprite CreateEmpIndicatorTarget(int gridRadius = Grid.KachelSize / 2)
        {
            if (Math.Abs(gridRadius) < 0.00001)
                return null;
            var sprite = new ImageSprite(CreateCircleTexture(gridRadius, Color.Blue));
            sprite.SetOrigin(RelativePosition.Center);
            return sprite;
        }
        
        public ImageSprite CreateTroupeMarker(int gridRadius = Grid.KachelSize / 2)
        {
            if (Math.Abs(gridRadius) < 0.00001)
                return null;
            var sprite = new ImageSprite(CreateCircleTexture(gridRadius, Color.LightGreen));
            sprite.SetOrigin(RelativePosition.Center);
            return sprite;
        }
        
        internal ImageSprite CreateHealIndicator(float radius)
        {
            if (Math.Abs(radius) < 0.00001)
                return null;
            var sprite = new ImageSprite(CreateCircleTexture((int) radius, Color.YellowGreen));
            sprite.SetOrigin(RelativePosition.Center);
            return sprite;
        }
        
        internal ImageSprite CreateEmp()
        {
            var sprite = new ImageSprite(CreateCircleTexture(Grid.KachelSize / 2, Color.Pink));
            sprite.SetOrigin(RelativePosition.Center);
            return sprite;
        }
        
        #endregion
        
        #endregion

        internal ImageSprite CreateColoredRectangle(int width, int height, Color[] color)
        {
            if (color.Length != width * height)
            {
                throw new Exception("not enough colors to fill the rectangle");
            }

            var texture = new Texture2D(GraphicsDevice, width, height);
            texture.SetData(color);
            return new ImageSprite(texture);
        }

        internal ImageSprite CreateEmptyTexture(int width, int height)
        {
            return new ImageSprite(new Texture2D(GraphicsDevice, width, height));
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
