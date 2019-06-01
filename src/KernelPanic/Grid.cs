using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    public sealed class Grid
    {
        // ------------------------------------------------------------------------------------------------------------
        private AStar mAStar;
        private Texture2D mKacheln;

        private void InitAStar()
        {
            mAStar = new AStar(mCoordinateSystem, mCoordinateSystem[0], mCoordinateSystem[mCoordinateSystem.Count]);
        }

        private void NewStart(Point start)
        {
            mAStar.SetStart(start);
        }

        private void NewTarget(Point target)
        {
            mAStar.SetTarget(target);
        }

        private void DrawPath(SpriteBatch spriteBatch)
        {
            List < Point > path = mAStar.FindPath();
            foreach (var position in path)
            {
                DrawTile(spriteBatch, position);
            }
        }
        private void DrawTile(SpriteBatch spriteBatch, Point point){
            mKacheln = mContent.Load<Texture2D>("LaneTile");
            var pos = ScreenPositionFromCoordinate(point);
            spriteBatch.Draw(mKacheln, new Rectangle(pos.X, pos.Y, (SingleTileSizePixel * TilesPerSprite), (SingleTileSizePixel * TilesPerSprite)), Color.Red);}
        // ------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Left and Right lane
        /// </summary>
        public enum LaneSide
        {
            Left,
            Right
        }

        private readonly ContentManager mContent;
        private int mRelativeX, mRelativeY;

        private readonly LaneSide mLaneSide;
        private readonly Rectangle mLaneRectangle;
        private readonly int mLaneWidthInTiles;

        private readonly List<Point> mCoordinateSystem = new List<Point>(); // coordinates are saved absolute/globaly 

        private const int KachelPixelSize = 100; // TODO
        private const int TilesPerSprite = 1; // per Dimension
        private const int SingleTileSizePixel = KachelPixelSize / TilesPerSprite;

        private readonly List<Tower> mTowerList = new List<Tower>();
        private readonly List<Vector2> mUsedGrids = new List<Vector2>();

        private Color mBorderColor = Color.Red;

        private Sprite mSprite;

        // static readonly List<Rectangle> sExistingGrids = new List<Rectangle>();

        public Grid(ContentManager content, LaneSide laneSide, Rectangle laneRectangle, int laneWidthInTiles = 10)
        {
            // TODO add assertions so the lane cant be created with weird numbers. 
            mContent = content;
            mLaneSide = laneSide;
            mLaneRectangle = laneRectangle;
            mLaneWidthInTiles = laneWidthInTiles;
            CreateCoordinateSystem();
            // sExistingGrids.Add(laneRectangle);

            var tile = content.Load<Texture2D>("LaneTile");
            var kachelSprite = new ImageSprite(tile, 0, 0) {Scale = (float) KachelPixelSize / tile.Width};
            var mainPart = new PatternSprite(kachelSprite, 0, 0, laneRectangle.Height, laneWidthInTiles);
            var topPart = new PatternSprite(kachelSprite,
                0,
                0,
                laneWidthInTiles,
                laneRectangle.Width - mLaneWidthInTiles);
            var bottomPart = new PatternSprite(kachelSprite,
                0,
                0,
                laneWidthInTiles,
                laneRectangle.Width - mLaneWidthInTiles);

            switch (laneSide)
            {
                case LaneSide.Right:
                    topPart.X = -topPart.Width;
                    bottomPart.X = -bottomPart.Width;
                    bottomPart.Y = mainPart.Height - bottomPart.Height;
                    break;

                case LaneSide.Left:
                    topPart.X = mainPart.Width;
                    bottomPart.X = mainPart.Width;
                    bottomPart.Y = mainPart.Height - bottomPart.Height;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(laneSide));
            }

            mSprite = new CompositeSprite(laneRectangle.X * KachelPixelSize, laneRectangle.Y * KachelPixelSize)
            {
                Children = {mainPart, bottomPart, topPart}
            };
        }

        /*
        ~Grid()
        {
            // delete the class level information about the grid
            sExistingGrids.Remove(mLaneRectangle);
        }
        */

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
            var laneWidth = mLaneWidthInTiles / TilesPerSprite;
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
            var laneWidth = mLaneWidthInTiles / TilesPerSprite;
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
        /// change the color of the selected square on doubleClick mainly for testing purpose
        /// </summary>
        private void UpdateColor()
        {
            if (!InputManager.Default.DoubleClick)
                return;
            
            mBorderColor = mBorderColor == Color.White ? Color.Red : Color.White;
        }

        
        private void DrawTower(SpriteBatch spriteBatch, GameTime gameTime, Matrix viewMatrix)
        {
            if (InputManager.Default.KeyDown(Keys.T) && !mUsedGrids.Contains(new Vector2((mRelativeX / 50) * 50, (mRelativeY / 50) * 50)))
            {
                mTowerList.Add(new Tower(mContent, (mRelativeX / 50) * 50, (mRelativeY / 50) * 50));
                mUsedGrids.Add(new Vector2((mRelativeX / 50) * 50, (mRelativeY / 50) * 50));
                SoundManager.Instance.PlaySound("placement");
            }


            foreach (var tower in mTowerList)
            {
                tower.Update(gameTime, viewMatrix);
                tower.Draw(spriteBatch);
            }
        }


        /// <summary>
        /// calling the different draw function
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="viewMatrix"></param>
        /// <param name="gameTime"></param>
        internal void Draw(SpriteBatch spriteBatch, Matrix viewMatrix, GameTime gameTime)
        {
            mSprite.Draw(spriteBatch, gameTime);
            DrawPath(spriteBatch); // debug AStar
            /*
            mKacheln = mContent.Load<Texture2D>("LaneTile");
            mBorder = mContent.Load<Texture2D>("Border");
            var relativeVector = Vector2.Transform(InputManager.Default.MousePosition.ToVector2(), Matrix.Invert(viewMatrix));
            mRelativeX = (int)relativeVector.X;
            mRelativeY = (int)relativeVector.Y;

            // DrawFields(spriteBatch);
            DrawGrid(spriteBatch);
            UpdateColor();
            DrawBorder(spriteBatch);
            DrawTower(spriteBatch, gameTime, viewMatrix);*/
            /*
            foreach (var point in mCoordinateSystem)
            {
                Console.WriteLine(point);
            }
            */
        }
    }
}