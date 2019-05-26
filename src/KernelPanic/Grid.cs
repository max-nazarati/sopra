using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    internal sealed class Grid
    {
        /// <summary>
        /// Left and Right lane
        /// </summary>
        public enum LaneSide
        {
            Left, Right
        }

        private readonly ContentManager mContent;
        private int mRelativeX, mRelativeY;
        private Texture2D mKacheln, mBorder;

        private readonly LaneSide mLaneSide;
        private readonly Rectangle mLaneRectangle;
        private readonly int mLaneWidthInTiles;

        private readonly List<Point> mCoordinateSystem = new List<Point>();  // coordinates are saved absolute/globaly 

        private const int KachelPixelSize = 100;  // TODO
        private const int TilesPerSprite = 1;  // per Dimension
        private const int SingleTileSizePixel = KachelPixelSize / TilesPerSprite;

        private readonly List<Tower> mTowerList = new List<Tower>();
        private readonly List<Vector2> mUsedGrids = new List<Vector2>();

        private Color mBorderColor = Color.Red;


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
        /// TODO
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void DrawGrid(SpriteBatch spriteBatch)
        {
            foreach (var point in mCoordinateSystem)
            {
                DrawTile(spriteBatch, point);
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="point"></param>
        private void DrawTile(SpriteBatch spriteBatch, Point point)
        {
            var pos = ScreenPositionFromCoordinate(point);
            spriteBatch.Draw(mKacheln, new Rectangle(pos.X, pos.Y, (SingleTileSizePixel * TilesPerSprite), (SingleTileSizePixel * TilesPerSprite)), Color.White);
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

        /// <summary>
        /// Draws the tile marking overlay
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void DrawBorder(SpriteBatch spriteBatch)
        {
            var posX = mRelativeX / SingleTileSizePixel * SingleTileSizePixel;
            var posY = mRelativeY / SingleTileSizePixel * SingleTileSizePixel;
            spriteBatch.Draw(mBorder, new Rectangle(posX, posY, SingleTileSizePixel, SingleTileSizePixel), null, mBorderColor);
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
            mKacheln = mContent.Load<Texture2D>("LaneTile");
            mBorder = mContent.Load<Texture2D>("Border");
            var relativeVector = Vector2.Transform(InputManager.Default.MousePosition.ToVector2(), Matrix.Invert(viewMatrix));
            mRelativeX = (int)relativeVector.X;
            mRelativeY = (int)relativeVector.Y;

            // DrawFields(spriteBatch);
            DrawGrid(spriteBatch);
            UpdateColor();
            DrawBorder(spriteBatch);
            DrawTower(spriteBatch, gameTime, viewMatrix);
            /*
            foreach (var point in mCoordinateSystem)
            {
                Console.WriteLine(point);
            }
            */
        }
    }
}