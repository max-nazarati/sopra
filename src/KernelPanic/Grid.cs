using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    [DataContract]
    internal sealed class Grid
    {
        /// <summary>
        /// Left and Right lane
        /// </summary>
        public enum LaneSide
        {
            /// <summary>
            /// Describes the lane which is shaped like an opening bracket ›[‹.
            /// </summary>
            Left,

            /// <summary>
            /// Describes the lane which is shaped like an opening bracket ›]‹.
            /// </summary>
            Right
        }

        private int mRelativeX, mRelativeY;

        private readonly LaneSide mLaneSide;
        private readonly Rectangle mLaneRectangle;

        private readonly List<Point> mCoordinateSystem = new List<Point>(); // coordinates are saved absolute/globaly
        public List<Point> CoordSystem => mCoordinateSystem;

        /// <summary>
        /// The size of a single tile in pixels.
        /// </summary>
        internal const int KachelSize = 100; // TODO
        
        private const int TilesPerSprite = 1; // per Dimension
        private const int SingleTileSizePixel = KachelSize / TilesPerSprite;
        private const int LaneWidthInTiles = 10;

        private static int TileCountPixelSize(int tiles) => tiles * KachelSize;

        private readonly Sprite mSprite;

        internal Grid(Rectangle laneBounds, SpriteManager sprites, LaneSide laneSide)
        {
            mLaneRectangle = laneBounds;
            mLaneSide = laneSide;

            var tile = CreateTile(sprites);
            var mainPart = new PatternSprite(tile, 0, 0, mLaneRectangle.Height, LaneWidthInTiles);

            var topPart = new PatternSprite(tile, 0, 0,
                LaneWidthInTiles,
                mLaneRectangle.Width - LaneWidthInTiles);
            var bottomPart = new PatternSprite(tile, 0, 0,
                LaneWidthInTiles,
                mLaneRectangle.Width - LaneWidthInTiles);
            bottomPart.Y = mainPart.Height - bottomPart.Height;

            switch (laneSide)
            {
                case LaneSide.Left:
                    topPart.X = mainPart.Width;
                    bottomPart.X = mainPart.Width;
                    break;

                case LaneSide.Right:
                    mainPart.X = topPart.Width;
                    mLaneRectangle.X = 32;
                    break;

                default:
                    throw new InvalidEnumArgumentException(nameof(laneSide), (int)laneSide, typeof(LaneSide));
            }

            mSprite = new CompositeSprite(TileCountPixelSize(mLaneRectangle.X), TileCountPixelSize(mLaneRectangle.Y))
            {
                Children = {mainPart, bottomPart, topPart}
            };

            CreateCoordinateSystem();
        }

        internal static ImageSprite CreateTile(SpriteManager spriteManager)
        {
            var tile = spriteManager.CreateLaneTile();
            tile.ScaleToWidth(KachelSize);
            return tile;
        }

        /* TODO implement when needed
        private static bool CoordinateInGrid(Point coordinate)
        {
            coordinate = CoordinatePositionFromScreen(coordinate);
            foreach (var rectangle in sExistingGrids)
            {
                if (rectangle.X <= coordinate.X && coordinate.X <= rectangle.X + rectangle.Width && 
                    rectangle.Y <= coordinate.Y && coordinate.Y <= rectangle.Y + rectangle.Height)
                {
                    return true;
                }
            }

            return false;
        }
        */

        /* TODO implement when needed
        /// <summary>
        /// calculates the position in the Grid for a given MousePosition on the screen
        /// </summary>
        /// <returns></returns>
        private static Point CoordinatePositionFromScreen(Point screenCoordinate)
        {
            var posX = mRelativeX / SingleTileSizePixel * SingleTileSizePixel;
            var posY = mRelativeY / SingleTileSizePixel * SingleTileSizePixel;
            return new Point(screenCoordinate.X * 100, screenCoordinate.Y * 100);
        }
        */

        /// <summary>
        /// calculates the position on the screen for a grid coordinate point,
        /// so things can be drawn correctly
        /// </summary>
        /// <param name="upperLeft"></param>
        /// <returns></returns>
        public static Point ScreenPositionFromCoordinate(Point upperLeft)
        {
            var xPositionGlobal = upperLeft.X; // this is now saved in the grid + mLaneRectangle.X;
            var yPositionGlobal = upperLeft.Y; // this is now saved in the grid + mLaneRectangle.Y;
            var x = (SingleTileSizePixel * TilesPerSprite * xPositionGlobal);
            var y = (SingleTileSizePixel * TilesPerSprite * yPositionGlobal);
            var result = new Point(x, y);
            return result;

        }


        /// <summary>
        /// 
        /// </summary>
        /// In this example:
        ///  * lane width = 3
        ///  * height = 11
        ///  * width = 6
        /// [ ][ ][ ][ ][ ][ ]    \
        /// [ ][ ][ ][ ][ ][ ]     > 1st for loop: top part
        /// [ ][ ][ ][ ][ ][ ]    /
        /// [ ][ ][ ]            \
        /// [ ][ ][ ]             \
        /// [ ][ ][ ]              > 2nd for loop: middle part
        /// [ ][ ][ ]             /
        /// [ ][ ][ ]            /
        /// [ ][ ][ ][ ][ ][ ]    \
        /// [ ][ ][ ][ ][ ][ ]     > 3rd for loop: bottom part
        /// [ ][ ][ ][ ][ ][ ]    /
        private void CreateCoordinateSystemLeft()
        {
            // calculate new Values depending on the Size of the sprite
            var laneWidth = LaneWidthInTiles / TilesPerSprite;
            var rectangleWidth = mLaneRectangle.Width / TilesPerSprite;
            var rectangleHeight = mLaneRectangle.Height / TilesPerSprite;

            // adding the top part
            for (var y = 0; y < laneWidth; y++)
            {
                for (var x = 0; x < rectangleWidth; x++)
                {
                    mCoordinateSystem.Add(new Point(x + mLaneRectangle.X, y + mLaneRectangle.Y));
                }
            }

            // adding the middle part
            for (var y = laneWidth; y < rectangleHeight - laneWidth; y++)
            {
                for (var x = 0; x < laneWidth; x++)
                {
                    mCoordinateSystem.Add(new Point(x + mLaneRectangle.X, y + mLaneRectangle.Y));
                }
            }

            // adding the bottom part
            for (var y = rectangleHeight - laneWidth; y < rectangleHeight; y++)
            {
                for (var x = 0; x < rectangleWidth; x++)
                {
                    mCoordinateSystem.Add(new Point(x + mLaneRectangle.X, y + mLaneRectangle.Y));
                }
            }
        }

        /// <summary>
        /// TODO 1st and 3rd loop can be put in one function and be called by left and right (4times ~same code)
        /// </summary>
        /// In this example:
        ///  * lane width = 3
        ///  * height = 11
        ///  * width = 6
        /// [ ][ ][ ][ ][ ][ ]    \
        /// [ ][ ][ ][ ][ ][ ]     > 1st for loop: top part
        /// [ ][ ][ ][ ][ ][ ]    /
        ///          [ ][ ][ ]   \
        ///          [ ][ ][ ]    \
        ///          [ ][ ][ ]     > 2nd for loop: middle part
        ///          [ ][ ][ ]    /
        ///          [ ][ ][ ]   /
        /// [ ][ ][ ][ ][ ][ ]    \
        /// [ ][ ][ ][ ][ ][ ]     > 3rd for loop: bottom part
        /// [ ][ ][ ][ ][ ][ ]    /
        private void CreateCoordinateSystemRight()
        {
            // calculate new Values depending on the Size of the sprite
            var laneWidth = LaneWidthInTiles / TilesPerSprite;
            var rectangleWidth = mLaneRectangle.Width / TilesPerSprite;
            var rectangleHeight = mLaneRectangle.Height / TilesPerSprite;

            // adding the top part
            for (var y = 0; y < laneWidth; y++)
            {
                for (var x = 0; x < rectangleWidth; x++)
                {
                    mCoordinateSystem.Add(new Point(x + mLaneRectangle.X, y + mLaneRectangle.Y));
                }
            }

            // adding the middle part
            for (var y = laneWidth; y < rectangleHeight - laneWidth; y++)
            {
                for (var x = rectangleWidth - laneWidth; x < rectangleWidth; x++)
                {
                    mCoordinateSystem.Add(new Point(x + mLaneRectangle.X, y + mLaneRectangle.Y));
                }
            }

            // adding the bottom part
            for (var y = rectangleHeight - laneWidth; y < rectangleHeight; y++)
            {
                for (var x = 0; x < rectangleWidth; x++)
                {
                    mCoordinateSystem.Add(new Point(x + mLaneRectangle.X, y + mLaneRectangle.Y));
                }
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        private void CreateCoordinateSystem()
        {
            switch (mLaneSide)
            {
                case LaneSide.Left:
                    CreateCoordinateSystemLeft();
                    break;
                case LaneSide.Right:
                    CreateCoordinateSystemRight();
                    break;
            }
        }

        /// <summary>
        /// Tests if the given point lies on this lane and if successful returns information about the hit tile.
        /// </summary>
        /// <param name="point">The point to test for.</param>
        /// <param name="subTileCount">The number of sub-tiles to segment </param>
        /// <param name="origin">If a tile is hit return its position according to this.</param>
        /// <returns>
        /// <c>null</c> if <paramref name="point"/> does not lie on this lane, otherwise the position of the hit
        /// tile and the tiles size.
        /// </returns>
        internal (Vector2 Position, float Size)? GridPointFromWorldPoint(
            Vector2 point,
            int subTileCount = 1,
            RelativePosition origin = RelativePosition.Center)
        {
            // TODO: We just convert float to int and Vector2 to Point,
            //       does this make a discernible difference to doing the exact calculations?
            var full = new Rectangle(mSprite.Position.ToPoint(), mSprite.Size.ToPoint());
            var cutout = new Rectangle(
                (int) mSprite.X + (mLaneSide == LaneSide.Left ? TileCountPixelSize(LaneWidthInTiles) : 0),
                TileCountPixelSize(LaneWidthInTiles),
                TileCountPixelSize(mLaneRectangle.Width - LaneWidthInTiles),
                TileCountPixelSize(mLaneRectangle.Height - 2 * LaneWidthInTiles));

            if (!full.Contains(point.ToPoint()) || cutout.Contains(point.ToPoint()))
                return null;

            var subTileSize = (float) KachelSize / subTileCount;
            void Calculate(ref float val)
            {
                var fullDiv = (int) (val / KachelSize);
                var fullRem = val % KachelSize;

                var subTileDiv = (int) (fullRem / subTileSize);
                
                // If there is no remainder to be put into the next sub-tile, use one less full sub-tile.
                if (Math.Abs(subTileDiv * subTileSize - fullRem) < 0.0001)
                    --subTileDiv;
                
                val = fullDiv * KachelSize + subTileDiv * subTileSize;
            }

            point -= mSprite.Position;
            Calculate(ref point.X);
            Calculate(ref point.Y);
            point += mSprite.Position + origin.RectangleOrigin(new Vector2(subTileSize));

            return (point, (float) KachelSize / subTileCount);
        }

        /// <summary>
        /// calling the different draw function
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        internal void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mSprite.Draw(spriteBatch, gameTime);
        }
    }
}
