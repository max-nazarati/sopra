using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TowerPlacementTest
{
    internal sealed class Grid
    {
        private readonly float mScale;
        private readonly ContentManager mContent;
        private int mRelativeX, mRelativeY;
        private Texture2D mKacheln, mBorder;
        /*
        private readonly int mRows, mColumns;
        
        
        private readonly bool mLeft;
        

        internal Grid(ContentManager content, int rows, int columns, bool left)
        {
            this.mContent = content;
            this.mRows = rows;
            this.mColumns = columns;
            this.mLeft = left;
            mScale = 0.5F;
        }

        private void DrawFields(SpriteBatch spriteBatch)
        {
            for (var i = 0; i < mColumns; i++)
            {
                for (var j = 0; j < mRows; j++)
                {
                    if (mLeft)
                    {
                        // draws left lane
                        spriteBatch.Draw(mKacheln,
                            new Rectangle((int)(mScale * mKacheln.Width * i),
                                (int)(mScale * mKacheln.Height * j),
                                (int)(mScale * 200),
                                (int)(mScale * 200)),
                            null,
                            Color.AliceBlue);
                        // draws left corners
                        if (j < mColumns || j > mRows - mColumns - 1)
                        {
                            spriteBatch.Draw(mKacheln,
                                new Rectangle((int)(mScale * mKacheln.Width * i + (mColumns * mScale * mKacheln.Width)),
                                    (int)(mScale * mKacheln.Height * j),
                                    (int)(mScale * 200),
                                    (int)(mScale * 200)),
                                null,
                                Color.AliceBlue);
                        }
                    }
                    else
                    {
                        // draws right lane
                        spriteBatch.Draw(mKacheln,
                            new Rectangle((int)(mScale * mKacheln.Width * i) + (int)(4000 * mScale),
                                (int)(mScale * mKacheln.Height * j),
                                (int)(mScale * 200),
                                (int)(mScale * 200)),
                            null,
                            Color.AliceBlue);
                        // draws right corners
                        if (j < mColumns || j > mRows - mColumns - 1)
                        {
                            spriteBatch.Draw(mKacheln,
                                new Rectangle(
                                    (int)(mScale * mKacheln.Width * i + 4000 * mScale -
                                           (mColumns * mScale * mKacheln.Width)),
                                    (int)(mScale * mKacheln.Height * j),
                                    (int)(mScale * 200),
                                    (int)(mScale * 200)),
                                null,
                                Color.AliceBlue);
                        }
                    }
                }
            }
        }
        */

        /// <summary>
        /// Left and Right lane
        /// </summary>
        public enum LaneSide
        {
            Left, Right
        }

        private readonly LaneSide mLaneSide;
        private readonly Rectangle mLaneSizeInTilesRectangle;
        private readonly int mLaneWidthInTiles;
        private readonly int mKachelPixelSize = 200;
        private readonly List<Tower> mTowerList = new List<Tower>();
        private readonly List<Vector2> mUsedGrids = new List<Vector2>();
        private Color mBorderColor = Color.Red;


        public Grid(ContentManager content, LaneSide laneSide, Rectangle laneSizeInTilesRectangle, int laneWidthInTiles = 5)
        {
            mContent = content;
            mLaneSide = laneSide;
            mLaneSizeInTilesRectangle = laneSizeInTilesRectangle;
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
        private void DrawTile(SpriteBatch spriteBatch, int column, int row, Color color)
        {
            spriteBatch.Draw(mKacheln,
                new Rectangle((int)(mScale * mKacheln.Width * column + (0 * mScale * mKacheln.Width)),
                    (int)(mScale * mKacheln.Height * row),
                    (int)(mScale * mKachelPixelSize),
                    (int)(mScale * mKachelPixelSize)),
                null,
                color);
        }

        /// <summary>
        /// Draws the biggest possible rectangle of the lane (the whole vertical part of the '[' resp. ']')
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="upperLeftPositionTuple"></param>
        private void DrawVerticalPart(SpriteBatch spriteBatch, Tuple<int, int> upperLeftPositionTuple)
        {
            for (var column = 0; column < mLaneWidthInTiles; column++)
            {
                for (var row = 0; row < mLaneSizeInTilesRectangle.Height; row++)
                {
                    DrawTile(spriteBatch, upperLeftPositionTuple.Item1 + column, upperLeftPositionTuple.Item2 + row, Color.Green);
                }
            }
        }

        /// <summary>
        /// Draws the small corner parts that are left after drawing the vertical part of the lane.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="upperLeftPositionTuple">most top left coordinate</param>
        private void DrawAttachedPart(SpriteBatch spriteBatch, Tuple<int, int> upperLeftPositionTuple)
        {
            for (var column = 0; column < mLaneSizeInTilesRectangle.Width - mLaneWidthInTiles; column++)
            {
                for (var row = 0; row < mLaneWidthInTiles; row++)
                {
                    DrawTile(spriteBatch, upperLeftPositionTuple.Item1 + column, upperLeftPositionTuple.Item2 + row, Color.Black);
                }
            }
        }

        /// <summary>
        /// Calls helper functions to draw the left lane (starting at most top left point)
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="upperLeftPositionTuple">most top left coordinate</param>
        private void DrawLeftLane(SpriteBatch spriteBatch, Tuple<int, int> upperLeftPositionTuple)
        {
            var leftPart = upperLeftPositionTuple;

            var topRight = new Tuple<int, int>(upperLeftPositionTuple.Item1 + mLaneWidthInTiles,
                upperLeftPositionTuple.Item2);

            var bottomRight = new Tuple<int, int>(upperLeftPositionTuple.Item1 + mLaneWidthInTiles,
                upperLeftPositionTuple.Item2 + mLaneSizeInTilesRectangle.Height - mLaneWidthInTiles);

            DrawVerticalPart(spriteBatch, leftPart);
            DrawAttachedPart(spriteBatch, topRight);
            DrawAttachedPart(spriteBatch, bottomRight);
        }

        /// <summary>
        /// Calls helper functions to draw the right lane (starting at most top left point)
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="upperLeftPositionTuple">most top left coordinate</param>
        private void DrawRightLane(SpriteBatch spriteBatch, Tuple<int, int> upperLeftPositionTuple)
        {
            // position of the right part of the lane (most top left point)
            var rightPart = new Tuple<int, int>(upperLeftPositionTuple.Item1 + mLaneWidthInTiles,
                upperLeftPositionTuple.Item2);

            // position of the top left part of the lane (most top left point)
            var topLeft = upperLeftPositionTuple;

            // position of the bottom left part of the lane (most top left point)
            var bottomLeft = new Tuple<int, int>(upperLeftPositionTuple.Item1,
                upperLeftPositionTuple.Item2 + mLaneSizeInTilesRectangle.Height - mLaneWidthInTiles);


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
                new Tuple<int, int>(mLaneSizeInTilesRectangle.X, mLaneSizeInTilesRectangle.Y);
            switch (mLaneSide)
            {
                case LaneSide.Left:
                    DrawLeftLane(spriteBatch, upperLeftPositionTuple);
                    break;
                case LaneSide.Right:
                    DrawRightLane(spriteBatch, upperLeftPositionTuple);
                    break;
            }
        }

        /// <summary>
        /// change the color of the selected square on doubleClick
        /// mainly for testing purpose
        /// </summary>
        private void UpdateColor()
        {
            if (!InputManager.Default.DoubleClick()) return;
            mBorderColor = mBorderColor == Color.Green ? Color.Red : Color.Green;
        }

        /// <summary>
        /// Draws the tile marking overlay
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void DrawBorder(SpriteBatch spriteBatch)
        {
            var posX = (int)((int)((mRelativeX) / 50) * 50);
            var posY = (int)((int)((mRelativeY) / 50) * 50);
            spriteBatch.Draw(mBorder, new Rectangle(posX, posY, 50, 50), null, Color.Black);
        }

        private void DrawTower(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (InputManager.Default.MouseDown(InputManager.MouseButton.Left) && !mUsedGrids.Contains(new Vector2((mRelativeX / 50) * 50, (mRelativeY / 50) * 50)))
            {
                mTowerList.Add(new Tower(mContent, (mRelativeX / 50) * 50, (mRelativeY / 50) * 50));
                mUsedGrids.Add(new Vector2((mRelativeX / 50) * 50, (mRelativeY / 50) * 50));
                SoundManager.Instance.PlaySound("placement");
            }


            foreach (var tower in mTowerList)
            {
                tower.Update(gameTime);
                tower.Draw(spriteBatch);
            }

        }

        /// <summary>
        /// calling the different draw function
        /// </summary>
        /// <param name="spriteBatch"></param>
        internal void Draw(SpriteBatch spriteBatch, Matrix viewMatrix, GameTime gameTime)
        {
            mKacheln = mContent.Load<Texture2D>("Kachel");
            mBorder = mContent.Load<Texture2D>("KachelDark");
            var relativeVector = Vector2.Transform(new Vector2(InputManager.Default.MousePositionX, InputManager.Default.MousePositionY),
                Matrix.Invert(viewMatrix));
            mRelativeX = (int)relativeVector.X;
            mRelativeY = (int)relativeVector.Y;

            // DrawFields(spriteBatch);
            DrawLane(spriteBatch);
            UpdateColor();
            DrawBorder(spriteBatch);
            DrawTower(spriteBatch, gameTime);
        }


    }
}