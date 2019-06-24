using System;
using System.ComponentModel;
using System.Net;
using KernelPanic.Input;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class MinimapOverlay
    {
        #region Member Variables
        
        private Sprite mSprite;
        private readonly Player mPlayerA;
        private readonly Player mPlayerB;
        private readonly SpriteManager mSpriteManager;
        private readonly float mRelativeSize; // how much of the screen should be the minimap [0, 1]
        private int mSize;
        private readonly Vector2 mPosition;
        private bool mSizeShouldChange;
        private float mScale;

        #region Colors
        
        private readonly Color mColorBackground = Color.DimGray;
        private readonly Color mColorPlayerA = Color.Lime;
        private readonly Color mColorPlayerB = Color.Red;
        private readonly Color mColorLaneA = Color.SlateGray;
        private readonly Color mColorLaneB = Color.LightGray;
        
        #endregion
        
        private readonly Color[] mData;
        private readonly Color[] mInitializedData;
        
        #endregion

        #region Konstruktor
        
        internal MinimapOverlay(Player player1, Player player2, SpriteManager spriteManager, float relativeSize = 0.3f)
        {
            var screenSizeX = spriteManager.ScreenSize.X;
            var screenSizeY = spriteManager.ScreenSize.Y;
            mRelativeSize = relativeSize;
            mSize = (int)(Math.Min(screenSizeX, screenSizeY) * mRelativeSize);
            mPosition = new Vector2(screenSizeX - mSize, screenSizeY - mSize);
            mPlayerA = player1;
            mPlayerB = player2;
            mSpriteManager = spriteManager;

            // filling the minimap with background color
            mData = new Color[mSize * mSize];
            mInitializedData = new Color[mSize * mSize];
            SetBackground();
            UpdateTexture();
            
            InitializeScale();
            InitializeLaneData();
        }
        
        #endregion

        #region Translate Function

        /// <summary>
        /// returns the index for data, which needs to be set for a world position
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        int CalculateMapIndexPosition(Vector2 point)
        {
            return (int)( ((point.Y * (mSize)) / mScale) + (point.X / mScale) );
        }
        
        /// <summary>
        /// calculates the world position of a minimap color index
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private Point CalculateWorldPosition(int i)
        {
            var x = (i % mSize) * mScale;
            var y = (i / (float)mSize) * mScale;
            return new Point((int)x, (int)y);
        }
        
        #endregion
        
        #region Initialize
        
        private void SetBackground()
        {
            for (var i = 0; i < mSize * mSize; i++)
            {
                mData[i] = mColorBackground;
            }
        }
        
        private void InitializeScale()
        {
            // we should not assume that minX = 0 and minY = 0 although it will probably be
            var laneRectangleA = mPlayerA.DefendingLane.GridRectangle();
            var minX = laneRectangleA.X;
            var minY = laneRectangleA.Y;
            
            var laneRectangleB = mPlayerB.DefendingLane.GridRectangle();
            var maxX = laneRectangleB.X + laneRectangleB.Width;
            var maxY = laneRectangleB.Y + laneRectangleB.Height;
            
            var bottomRight = Grid.ScreenPositionFromCoordinate(new Point(maxX-minX, maxY-minY));
            
            mScale = Math.Max(bottomRight.X, bottomRight.Y) / (float)mSize;
            Console.WriteLine("There will be " + mScale + " x " + mScale + " Pixel represented by 1 minimap Pixel");
            // Console.WriteLine("There are a total of " + mSize * mSize + " Pixel.");
        }

        private void InitializeLaneData()
        {
            for (int i = 0; i < mData.Length; i++)
            {
                mInitializedData[i] = LaneColor(i);
            }
        }
        
        #endregion

        #region Update

        public void Update(InputManager inputManager, GameTime gameTime)
        {
            UpdateSize();
            UpdateData();
            UpdateTexture();
        }

        private void UpdateSize()
        {
            if (!mSizeShouldChange) { return; }
            var screenSizeX = mSpriteManager.ScreenSize.X;
            var screenSizeY = mSpriteManager.ScreenSize.Y;
            mSize = (int)(Math.Min(screenSizeX, screenSizeY) * mRelativeSize);
        }

        private void UpdateData()
        {
            for (int i = 0; i < mData.Length; i++)
            {
                // mData[i] = CalculateColor(i);
                mData[i] = mInitializedData[i];
            }
            
            SetEntityColor();
        }

        private Color LaneColor(int i)
        {
            Point point = CalculateWorldPosition(i);
            // Console.WriteLine(point);
            if (mPlayerA.DefendingLane.Contains(new Vector2(point.X, point.Y)))
            {
                return mColorLaneA;
            }
            if (mPlayerB.DefendingLane.Contains(new Vector2(point.X, point.Y)))
            {
                return mColorLaneB;
            }

            return mColorBackground;
        }

        private void SetEntityColor()
        {
            var entities = mPlayerB.AttackingLane.EntityGraph.QuadTree.GetObjects();
            foreach (var entity in entities)
            {
                mData[CalculateMapIndexPosition(entity.Sprite.Position)] = mColorPlayerA;
                
                mData[CalculateMapIndexPosition(entity.Sprite.Position) + 1] = mColorPlayerA;
                mData[CalculateMapIndexPosition(entity.Sprite.Position) + 2] = mColorPlayerA;
                mData[CalculateMapIndexPosition(entity.Sprite.Position) + 3] = mColorPlayerA;
                mData[CalculateMapIndexPosition(entity.Sprite.Position) - 1] = mColorPlayerA;
                mData[CalculateMapIndexPosition(entity.Sprite.Position) - 2] = mColorPlayerA;
                mData[CalculateMapIndexPosition(entity.Sprite.Position) - 3] = mColorPlayerA;
                mData[CalculateMapIndexPosition(entity.Sprite.Position) +     mSize] = mColorPlayerA;
                mData[CalculateMapIndexPosition(entity.Sprite.Position) + 2 * mSize] = mColorPlayerA;
                mData[CalculateMapIndexPosition(entity.Sprite.Position) + 3 * mSize] = mColorPlayerA;
                mData[CalculateMapIndexPosition(entity.Sprite.Position) -     mSize] = mColorPlayerA;
                mData[CalculateMapIndexPosition(entity.Sprite.Position) - 2 * mSize] = mColorPlayerA;
                mData[CalculateMapIndexPosition(entity.Sprite.Position) - 3 * mSize] = mColorPlayerA;
                
                Console.WriteLine(CalculateMapIndexPosition(entity.Sprite.Position));
            }
        }
        
        private void DebugInformation()
        {
            var sizePerLane = mSize / 2;
            var laneGridA = mPlayerA.DefendingLane.GridRectangle();
            var laneGridB = mPlayerB.DefendingLane.GridRectangle();

            var horizontalPlaceNeeded = laneGridA.Width;

            var topLeftPointA = Grid.ScreenPositionFromCoordinate(new Point(laneGridA.X, laneGridA.Y));
            var topLeftPointB = Grid.ScreenPositionFromCoordinate(new Point(laneGridB.X, laneGridB.Y));
            Console.WriteLine("bottom right: " + topLeftPointA);
            Console.WriteLine("top left: " + topLeftPointB);
            
            var rectangleSizeA = new Point(laneGridA.Width, laneGridA.Height);
            var rectangleSizeB = new Point(laneGridB.Width, laneGridB.Height);
            Console.WriteLine("Rectangle Size A: " + rectangleSizeA);
            Console.WriteLine("Rectangle Size B: " + rectangleSizeB);

            var bottomRight = Grid.ScreenPositionFromCoordinate(new Point(laneGridB.X + laneGridB.Width, laneGridB.Y + laneGridB.Height));
            Console.WriteLine("bottomRight: " + bottomRight);
        }

        private void UpdateTexture()
        {
            mSprite = mSpriteManager.CreateColoredRectangle(mSize, mSize, mData);
            mSprite.Position = mPosition;
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mSprite.Draw(spriteBatch, gameTime);
        }

        #endregion
    }
}