using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using KernelPanic.Data;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Table
{
    [DataContract]
    internal sealed class Grid : IBounded
    {
        private int mRelativeX, mRelativeY;

        internal const int LaneWidthInTiles = 10;
        internal Lane.Side LaneSide { get; }
        internal Rectangle LaneRectangle { get; }

        private readonly List<Point> mCoordinateSystem = new List<Point>(); // coordinates are saved absolute/globaly
        public List<Point> CoordSystem => mCoordinateSystem;

        /// <summary>
        /// The size of a single tile in pixels.
        /// </summary>
        internal const int KachelSize = 100; // TODO
        
        private const int TilesPerSprite = 1; // per Dimension
        private const int SingleTileSizePixel = KachelSize / TilesPerSprite;

        private static int TileCountPixelSize(int tiles) => tiles * KachelSize;

        private readonly Sprite mSprite;

        public Rectangle Bounds => mSprite.Bounds;

        internal Grid(Rectangle laneBounds, SpriteManager sprites, Lane.Side laneSide)
        {
            LaneRectangle = laneBounds;
            LaneSide = laneSide;

            var tile = CreateTile(sprites);
            var mainPart = new PatternSprite(tile, 0, 0, LaneRectangle.Height, LaneWidthInTiles);

            var topPart = new PatternSprite(tile, 0, 0,
                LaneWidthInTiles,
                LaneRectangle.Width - LaneWidthInTiles);
            var bottomPart = new PatternSprite(tile, 0, 0,
                LaneWidthInTiles,
                LaneRectangle.Width - LaneWidthInTiles);
            bottomPart.Y = mainPart.Height - bottomPart.Height;

            switch (laneSide)
            {
                case Lane.Side.Left:
                    topPart.X = mainPart.Width;
                    bottomPart.X = mainPart.Width;
                    break;

                case Lane.Side.Right:
                    mainPart.X = topPart.Width;
                    break;

                default:
                    throw new InvalidEnumArgumentException(nameof(laneSide), (int)laneSide, typeof(Lane.Side));
            }

            mSprite = new CompositeSprite(TileCountPixelSize(LaneRectangle.X), TileCountPixelSize(LaneRectangle.Y))
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

        internal static ImageSprite CreateTileBorder(SpriteManager spriteManager)
        {
            var tile = spriteManager.CreateLaneBorder();
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
            var rectangleWidth = LaneRectangle.Width / TilesPerSprite;
            var rectangleHeight = LaneRectangle.Height / TilesPerSprite;

            // adding the top part
            for (var y = 0; y < laneWidth; y++)
            {
                for (var x = 0; x < rectangleWidth; x++)
                {
                    mCoordinateSystem.Add(new Point(x + LaneRectangle.X, y + LaneRectangle.Y));
                }
            }

            // adding the middle part
            for (var y = laneWidth; y < rectangleHeight - laneWidth; y++)
            {
                for (var x = 0; x < laneWidth; x++)
                {
                    mCoordinateSystem.Add(new Point(x + LaneRectangle.X, y + LaneRectangle.Y));
                }
            }

            // adding the bottom part
            for (var y = rectangleHeight - laneWidth; y < rectangleHeight; y++)
            {
                for (var x = 0; x < rectangleWidth; x++)
                {
                    mCoordinateSystem.Add(new Point(x + LaneRectangle.X, y + LaneRectangle.Y));
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
            var rectangleWidth = LaneRectangle.Width / TilesPerSprite;
            var rectangleHeight = LaneRectangle.Height / TilesPerSprite;

            // adding the top part
            for (var y = 0; y < laneWidth; y++)
            {
                for (var x = 0; x < rectangleWidth; x++)
                {
                    mCoordinateSystem.Add(new Point(x + LaneRectangle.X, y + LaneRectangle.Y));
                }
            }

            // adding the middle part
            for (var y = laneWidth; y < rectangleHeight - laneWidth; y++)
            {
                for (var x = rectangleWidth - laneWidth; x < rectangleWidth; x++)
                {
                    mCoordinateSystem.Add(new Point(x + LaneRectangle.X, y + LaneRectangle.Y));
                }
            }

            // adding the bottom part
            for (var y = rectangleHeight - laneWidth; y < rectangleHeight; y++)
            {
                for (var x = 0; x < rectangleWidth; x++)
                {
                    mCoordinateSystem.Add(new Point(x + LaneRectangle.X, y + LaneRectangle.Y));
                }
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        private void CreateCoordinateSystem()
        {
            switch (LaneSide)
            {
                case Lane.Side.Left:
                    CreateCoordinateSystemLeft();
                    break;
                case Lane.Side.Right:
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
            if (!Contains(point))
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
        /// Tests if the given <paramref name="point"/> lies in this grid.
        /// </summary>
        /// <param name="point">The point to test for.</param>
        /// <returns><c>true</c> if the point is inside, <c>false</c> otherwise.</returns>
        internal bool Contains(Vector2 point)
        {
            var cutout = new Rectangle(
                (int) mSprite.X + (LaneSide == Lane.Side.Left ? TileCountPixelSize(LaneWidthInTiles) : 0),
                TileCountPixelSize(LaneWidthInTiles),
                TileCountPixelSize(LaneRectangle.Width - LaneWidthInTiles),
                TileCountPixelSize(LaneRectangle.Height - 2 * LaneWidthInTiles));

            return Bounds.Contains(point) && !cutout.Contains(point);
        }

        /// <summary>
        /// Calculates the origin the tile identified by <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the tile.</param>
        /// <param name="origin">Where to put the origin in the rectangle, <see cref="RelativePosition.Center"/> by default.</param>
        /// <returns>The position of the origin, and the length of the square's sides.</returns>
        /// <exception cref="IndexOutOfRangeException">If <paramref name="index"/> is outside the bounds of <see cref="LaneRectangle"/>.</exception>
        internal (Vector2 Position, float Size) GetTile(TileIndex index, RelativePosition origin = RelativePosition.Center)
        {
            if (index.Column / index.SubTileCount >= LaneRectangle.Width || index.Row / index.SubTileCount >= LaneRectangle.Height)
            {
                throw new IndexOutOfRangeException($"TileIndex {index} is out of bounds {LaneRectangle.Size}");
            }

            var subTileSize = (float) KachelSize / index.SubTileCount;
            var position = mSprite.Position + index.ToPoint().ToVector2() + origin.RectangleOrigin(new Vector2(subTileSize));
            return (position, subTileSize);
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
