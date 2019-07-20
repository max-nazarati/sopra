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
            BitcoinLogo
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

        internal PatternSprite CreateBoardBackground(Rectangle bounds, int tileSize)
        {
            var rows = bounds.Height / 100;
            var columns = bounds.Width / 100;
            var tile = new ImageSprite(Lookup(Image.BackgroundTile1));
            tile.ScaleToWidth(tileSize);
            var sprite = new PatternSprite(tile, rows, columns)
            {
                Position = new Vector2(bounds.X, bounds.Y)
            };
            return sprite;
        }

        #endregion

        #region Buildings

        internal ImageSprite CreateWifiRouter() => new ImageSprite(Lookup(Image.Router));
        internal ImageSprite CreateVentilator() => new ImageSprite(Lookup(Image.FanPropeller));
        internal ImageSprite CreateAntivirus() => new ImageSprite(Lookup(Image.Antivirus));
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
            sprite.SetOrigin(RelativePosition.Center);
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

        #endregion

        internal ImageSprite CreateVectorArrow()
        {
            var sprite = new ImageSprite(Lookup(Image.VectorArrow));
            sprite.ScaleToHeight(0.4f * Grid.KachelSize);
            sprite.SetOrigin(RelativePosition.CenterBottom);
            return sprite;
        }

        internal (Sprite Main, TextSprite Left, TextSprite LeftMoney, TextSprite LeftEP, TextSprite Right
            , TextSprite RightMoney, TextSprite RightEP, TextSprite Clock) CreateScoreDisplay()
        {
            const float scale = 1.8f;
            const float hudWidth = scale * 318;
            const float moneyWidth = scale * 60;
            const float hudHeight = scale * 44;
            const float padding = scale * 5;
            const float topPadding = scale * 8;
            var font = Lookup(Font.Hud);
            var leftBoarder = (int)((float)ScreenSize.X / 2 - hudWidth / 2);

            var leftMoneyText = new TextSprite(font, "00000 $")
            {
                Position = new Vector2(padding, topPadding)
            };
            var leftEpText = new TextSprite(font, "000 EP")
            {
                Position = new Vector2(moneyWidth + padding, topPadding)
            };
            var leftText = new TextSprite(font, "000%")
            {
                Position = new Vector2(2 * moneyWidth + padding, topPadding),
                TintColor = Color.DarkBlue
            };
            var rightMoneyText = new TextSprite(font, "00000$")
            {
                Position = new Vector2(hudWidth - padding, topPadding)
            };
            rightMoneyText.SetOrigin(RelativePosition.TopRight);
            var rightEpText = new TextSprite(font, "000 EP")
            {
                Position = new Vector2(hudWidth - moneyWidth - padding, topPadding),
                
            };
            rightEpText.SetOrigin(RelativePosition.TopRight);
            var rightText = new TextSprite(font, "000%")
            {
                Position = new Vector2(hudWidth - 2 * moneyWidth - padding, topPadding),
                TintColor = Color.DarkRed
            };
            rightText.SetOrigin(RelativePosition.TopRight);
            var clockText = new TextSprite(font, "00:00:00")
            {
                Position = new Vector2(hudWidth / 2, hudHeight / 2 + topPadding)
            };
            clockText.SetOrigin(RelativePosition.CenterTop);

            var hudSprite = new ImageSprite(Lookup(Image.ScoreBackground));
            hudSprite.ScaleToWidth(hudWidth);
            var bitcoinSpriteLeft = new ImageSprite(Lookup(Image.BitcoinLogo))
            {
                Position = new Vector2(50, 12)
            };
            bitcoinSpriteLeft.ScaleToHeight(23);
            
            var bitcoinSpriteRight = new ImageSprite(Lookup(Image.BitcoinLogo))
            {
                Position = new Vector2(hudWidth-40, 12)
            };
            bitcoinSpriteRight.ScaleToHeight(23);
            
            var sprite = new CompositeSprite
            {
                Children = { hudSprite, leftText, leftMoneyText, leftEpText, rightText, rightMoneyText, rightEpText
                    , clockText, bitcoinSpriteLeft, bitcoinSpriteRight },
                Position = new Vector2(leftBoarder, 0)
            };
            return (sprite, leftText, leftMoneyText, leftEpText, rightText, rightMoneyText, rightEpText, clockText);
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
