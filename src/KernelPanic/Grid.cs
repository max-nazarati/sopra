using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class Grid
    {
        /// <summary>
        /// Left and Right lane
        /// </summary>
        public enum LaneSide
        {
            Left,
            Right
        }

        private int mRelativeX, mRelativeY;

        private readonly LaneSide mLaneSide;
        private readonly Rectangle mLaneRectangle;

        private readonly List<Point> mCoordinateSystem = new List<Point>(); // coordinates are saved absolute/globaly 

        /// <summary>
        /// The size of a single tile in pixels.
        /// </summary>
        internal const int KachelSize = 100; // TODO
        
        private const int TilesPerSprite = 1; // per Dimension
        private const int SingleTileSizePixel = KachelSize / TilesPerSprite;
        private const int LaneWidthInTiles = 10;

        private readonly Sprite mSprite;

        internal Grid(SpriteManager sprites, LaneSide laneSide)
        {
            mLaneRectangle = new Rectangle(0, 0, 16, 42);
            mLaneSide = laneSide;

            var tile = sprites.CreateLaneTile();
            tile.ScaleToWidth(KachelSize);
            var mainPart = new PatternSprite(tile, 0, 0, mLaneRectangle.Height, LaneWidthInTiles);
            
            float xOffset;
            RelativePosition upperOrigin;
            switch (laneSide)
            {
                case LaneSide.Left:
                    xOffset = mainPart.Width;
                    upperOrigin = RelativePosition.TopLeft;
                    break;

                case LaneSide.Right:
                    mLaneRectangle.X = 32;
                    xOffset = 0;
                    upperOrigin = RelativePosition.TopRight;
                    break;

                default:
                    throw new InvalidEnumArgumentException(nameof(laneSide), (int)laneSide, typeof(LaneSide));
            }
            
            var topPart = new PatternSprite(tile,
                xOffset,
                0,
                LaneWidthInTiles,
                mLaneRectangle.Width - LaneWidthInTiles);
            var bottomPart = new PatternSprite(tile,
                xOffset,
                mainPart.Height,
                LaneWidthInTiles,
                mLaneRectangle.Width - LaneWidthInTiles);
            
            topPart.SetOrigin(upperOrigin);
            bottomPart.SetOrigin(upperOrigin.MirrorVertical());

            mSprite = new CompositeSprite(mLaneRectangle.X * KachelSize, mLaneRectangle.Y * KachelSize)
            {
                Children = {mainPart, bottomPart, topPart}
            };

            CreateCoordinateSystem();
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
        private static Point ScreenPositionFromCoordinate(Point upperLeft)
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
