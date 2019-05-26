using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class Grid
    {
        private readonly float mScale;
        private readonly ContentManager mContent;
        private int mRelativeX, mRelativeY;
        private Texture2D mKacheln, mBorder;
        
        /// <summary>
        /// Left and Right lane
        /// </summary>
        public enum LaneSide
        {
            Left, Right
        }

        private readonly LaneSide mLaneSide;
        private readonly Rectangle mLaneRectangle;
        // private List<Point> mCoordinateSystem;
        private readonly int mLaneWidthInTiles;
        private readonly List<Tower> mTowerList = new List<Tower>();
        private readonly List<Vector2> mUsedGrids = new List<Vector2>();
        private const int KachelPixelSize = 200;
        private Color mBorderColor = Color.Red;


        public Grid(ContentManager content, LaneSide laneSide, Rectangle laneSizeInTilesRectangle, int laneWidthInTiles = 5)
        {
            mContent = content;
            mLaneSide = laneSide;
            mLaneRectangle = laneSizeInTilesRectangle;
            mLaneWidthInTiles = laneWidthInTiles;
            mScale = 0.5F;
        }

        /// <summary>
        /// Draws a single Tile (currently 2x2 sized)
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <param name="color"></param>
        /// TODO private void DrawTile(SpriteBatch spriteBatch, Point upperLeft, Color) {
        ///  pos func_screenpositionFromCoordinate(upperleft);
        /// new Rectangle(pos.X, pos.Y, ...., ...)
        /// }
        private void DrawTile(SpriteBatch spriteBatch, int column, int row, Color color)
        {
            spriteBatch.Draw(mKacheln,
                new Rectangle((int)(mScale * mKacheln.Width * column + 0 * mScale * mKacheln.Width),
                    (int)(mScale * mKacheln.Height * row),
                    (int)(mScale * KachelPixelSize),
                    (int)(mScale * KachelPixelSize)),
                null,
                color);
        }

        /// <summary>
        /// Draws the biggest possible rectangle of the lane (the whole vertical part of the '[' resp. ']')
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="upperLeft"></param>
        private void DrawVerticalPart(SpriteBatch spriteBatch, Point upperLeft)
        {
            for (var column = 0; column < mLaneWidthInTiles; column++)
            {
                for (var row = 0; row < mLaneRectangle.Height; row++)
                {
                    DrawTile(spriteBatch, upperLeft.X + column, upperLeft.Y + row, Color.White);
                }
            }
        }

        /// <summary>
        /// Draws the small corner parts that are left after drawing the vertical part of the lane.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="upperLeft">most top left coordinate</param>
        private void DrawAttachedPart(SpriteBatch spriteBatch, Point upperLeft)
        {
            for (var column = 0; column < mLaneRectangle.Width - mLaneWidthInTiles; column++)
            {
                for (var row = 0; row < mLaneWidthInTiles; row++)
                {
                    DrawTile(spriteBatch, upperLeft.X + column, upperLeft.Y + row, Color.White);
                }
            }
        }

        /// <summary>
        /// Calls helper functions to draw the left lane (starting at most top left point)
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="upperLeft">most top left coordinate</param>
        private void DrawLeftLane(SpriteBatch spriteBatch, Point upperLeft)
        {
            var leftPart = upperLeft;
            var topRight = new Point(upperLeft.X + mLaneWidthInTiles, upperLeft.Y);
            var bottomRight = new Point(
                upperLeft.X + mLaneWidthInTiles,
                upperLeft.Y + mLaneRectangle.Height - mLaneWidthInTiles);

            DrawVerticalPart(spriteBatch, leftPart);
            DrawAttachedPart(spriteBatch, topRight);
            DrawAttachedPart(spriteBatch, bottomRight);
        }

        /// <summary>
        /// Calls helper functions to draw the right lane (starting at most top left point)
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="upperLeft">most top left coordinate</param>
        private void DrawRightLane(SpriteBatch spriteBatch, Point upperLeft)
        {
            // position of the right part of the lane (most top left point)
            var rightPart = new Point(upperLeft.X + mLaneRectangle.Width - mLaneWidthInTiles, upperLeft.Y);
 
            // position of the top left part of the lane (most top left point)
            var topLeft = upperLeft;

            // position of the bottom left part of the lane (most top left point)
            var bottomLeft = new Point(upperLeft.X,
                upperLeft.Y + mLaneRectangle.Height - mLaneWidthInTiles);


            DrawVerticalPart(spriteBatch, rightPart);
            DrawAttachedPart(spriteBatch, topLeft);
            DrawAttachedPart(spriteBatch, bottomLeft);
        }

        /// <summary>
        /// calls the corresponding draw function for LaneSide
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void DrawLane(SpriteBatch spriteBatch)
        {
            var upperLeftPositionTuple =
                new Point(mLaneRectangle.X, mLaneRectangle.Y);
            switch (mLaneSide)
            {
                case LaneSide.Left:
                    DrawLeftLane(spriteBatch, upperLeftPositionTuple);
                    break;
                case LaneSide.Right:
                    DrawRightLane(spriteBatch, upperLeftPositionTuple);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mLaneSide));
            }
        }

        /// <summary>
        /// change the color of the selected square on doubleClick
        /// mainly for testing purpose
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
            var posX = mRelativeX / 50 * 50;
            var posY = mRelativeY / 50 * 50;
            spriteBatch.Draw(mBorder, new Rectangle(posX, posY, 50, 50), null, mBorderColor);
        }
        
        private void DrawTower(SpriteBatch spriteBatch, GameTime gameTime, Matrix viewMatrix)
        {
            if (InputManager.Default.MouseDown(InputManager.MouseButton.Left) && !mUsedGrids.Contains(new Vector2((mRelativeX / 50) * 50, (mRelativeY / 50) * 50)))
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

/*
        private void CreateCoordinateSystem()
        {
            mCoordinateSystem.Add(new Point());
            for (var i = 0; i < mLaneRectangle.X; i++)
            {
                for (var j = 0; j < mLaneRectangle.Y; j++)
                {
                    mCoordinateSystem.Add(new Point(i, j));
                }
            }
        }
*/
        /// <summary>
        /// calling the different draw function
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="viewMatrix"></param>
        /// <param name="gameTime"></param>
        internal void Draw(SpriteBatch spriteBatch, Matrix viewMatrix, GameTime gameTime)
        {
            mKacheln = mContent.Load<Texture2D>("Kachel3");
            mBorder = mContent.Load<Texture2D>("Border");
            var relativeVector = Vector2.Transform(InputManager.Default.MousePosition.ToVector2(), Matrix.Invert(viewMatrix));
            mRelativeX = (int)relativeVector.X;
            mRelativeY = (int)relativeVector.Y;

            // DrawFields(spriteBatch);
            DrawLane(spriteBatch);
            UpdateColor();
            DrawBorder(spriteBatch);
            // DrawTower(spriteBatch, gameTime, viewMatrix);
        }
    }
}