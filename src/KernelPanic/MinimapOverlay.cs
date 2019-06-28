using System;
using System.ComponentModel;
using System.Net;
using KernelPanic.Entities;
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
        private int mRadius;

        #region Colors
        
        private readonly Color mColorBackground = Color.DimGray;
        private readonly Color mColorPlayerA = Color.Lime;
        private readonly Color mColorPlayerB = Color.Red;
        private readonly Color mColorLaneA = Color.SlateGray;
        private readonly Color mColorLaneB = Color.LightGray;
        private readonly Color mColorSelected = Color.Coral;
        
        #endregion
        
        private readonly Color[] mData;
        private readonly Color[] mInitializedData;

        #endregion

        #region Konstruktor
        
        internal MinimapOverlay(Player player1, Player player2, SpriteManager spriteManager, float relativeSize = 0.3f)
        {
            var screenSizeX = spriteManager.ScreenSize.X;
            var screenSizeY = spriteManager.ScreenSize.Y;
            Console.WriteLine(screenSizeX);
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
            
            InitializeScale(); // mRadius is init here
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
            return (int)( ( (int)( point.Y / mScale) * mSize) + (point.X / mScale));
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
            
            mRadius = (int)(Grid.KachelSize / (mScale * 2)); // InitializeScale before mRadius
            Console.WriteLine("Kachelsize is: " + Grid.KachelSize + " and mScale is: " + mScale);
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
            var drawSelected = false;
            var selectedIndex = 0;
            
            var entities = mPlayerB.AttackingLane.EntityGraph.QuadTree;
            foreach (var entity in entities)
            {
                var index = CalculateMapIndexPosition(entity.Sprite.Position);
                var color = mColorBackground;
                
                if (entity.Selected)
                {
                    // dont draw it yet
                    // TODO needs to be adapted if we can select multiple entities at a given point
                    drawSelected = true;
                    selectedIndex = index;
                    continue;
                }
                else if (entity is Unit)
                {
                    color = mColorPlayerA;
                }
                else if (entity is Tower)
                {
                    color = mColorPlayerB;
                }
                SetPixelSquare(index, color);

                // draw the selected one on top
                if (drawSelected)
                {
                    SetPixelSquare(selectedIndex, mColorSelected);
                }
            }
        }

        private void SetPixelSquare(int dataIndex, Color color, int radius = -1)
        {
            // dEfAuLt PaRaMeTeR vAlUe FoR 'rAdIuS' mUsT bE a CoMpIlE-tImE cOnStAnT
            if (radius == -1)
            {
                radius = mRadius;
            }
            
            for (int i = -radius; i < radius; i++)
            {
                for (int j = -radius; j < radius; j++)
                {
                    var pos = dataIndex + i + mSize * j;
                    var clampedPos = Math.Max(Math.Min(pos, mSize * mSize), 0);
                    mData[clampedPos] = color;
                    // mData[dataIndex + i + mSize * j] = color;
                }
            }
        }

        private void UpdateTexture()
        {
            mSprite = mSpriteManager.CreateColoredRectangle(mSize, mSize, mData);
            mSprite.Position = mPosition;
        }

        #region Debug
        
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
        
        #endregion
        
        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mSprite.Draw(spriteBatch, gameTime);
        }

        #endregion
    }
}