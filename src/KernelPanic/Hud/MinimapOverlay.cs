using System;
using KernelPanic.Camera;
using KernelPanic.Data;
using KernelPanic.Entities;
using KernelPanic.Players;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Hud
{
    internal sealed class MinimapOverlay
    {
        #region Member Variables
        
        private Sprite mSprite;
        private readonly PlayerIndexed<Player> mPlayers;
        private readonly SpriteManager mSpriteManager;
        private readonly float mRelativeSize; // how much of the screen should be the minimap [0, 1]
        private int mSize;
        private readonly ICamera mCamera;
        private readonly Vector2 mPosition;
        private bool mSizeShouldChange;
        private float mScale;
        private int mRadius;

        #region Colors
        
        private readonly Color mColorBackground = Color.DimGray;
        private readonly Color mColorPlayerA = Color.Lime;
        private readonly Color mColorPlayerB = Color.Red;
        private readonly Color mColorLaneA = Color.SlateGray;
        private readonly Color mColorLaneB = Color.SlateGray;
        private readonly Color mColorSelected = Color.Coral;
        private readonly Color mScreenBorder = Color.White;
        
        #endregion
        
        private readonly Color[] mData;
        private readonly Color[] mInitializedData;

        #endregion

        #region Konstruktor
        
        
        internal MinimapOverlay(PlayerIndexed<Player> players, SpriteManager spriteManager, ICamera camera, float relativeSize = 0.3f)
        {
            var screenSizeX = spriteManager.ScreenSize.X;
            var screenSizeY = spriteManager.ScreenSize.Y;
            Console.WriteLine(screenSizeX);
            mRelativeSize = relativeSize;
            mSize = (int)(Math.Min(screenSizeX, screenSizeY) * mRelativeSize);
            mPosition = new Vector2(screenSizeX - mSize, screenSizeY - mSize);
            mPlayers = players;
            mSpriteManager = spriteManager;
            mCamera = camera;

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
        /// since there are e.g. 15x15 world pixel for the same minimap pixel
        /// CalculateMapIndexPosition( (0, 1) ) and CalculateMapIndexPosition( (0, 2) ) will return the same MiniM.
        /// Data Index
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
            var laneRight = mPlayers.B.DefendingLane.Grid;
            var pointBottomRight = new TileIndex(laneRight.LaneRectangle.Size - new Point(1), 1);
            var bottomRight = laneRight.GetTile(pointBottomRight, RelativePosition.BottomRight).Position;

            mScale = Math.Max(bottomRight.X, bottomRight.Y) / mSize;
            
            Console.WriteLine("There will be " + mScale + " x " + mScale + " Pixel represented by 1 minimap Pixel");
            // Console.WriteLine("There are a total of " + mSize * mSize + " Pixel.");
            
            mRadius = (int)(Grid.KachelSize / (mScale * 2)); // InitializeScale before mRadius
            Console.WriteLine("KachelSize is: " + Grid.KachelSize + " and mScale is: " + mScale);
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

        public void Update()
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
            
            SetEntityColor(mPlayers.A.DefendingLane);
            SetEntityColor(mPlayers.B.DefendingLane);
            SetScreenColor();
        }

        private void SetScreenColor()
        {
            DrawCameraRectangle();
        }

        private void DrawCameraRectangle()
        {
            DrawRectangle(CameraRectangle());
        }

        /// <summary>
        /// Calculates the Camera view Rectangle, translated in MiniMap coordinates.
        /// </summary>
        /// <returns></returns>
        private Rectangle CameraRectangle()
        {
            Rectangle rect;
            rect.X = (int)-(mCamera.Transformation.M41 / (mScale * mCamera.Transformation.M11));
            rect.Y = (int)-(mCamera.Transformation.M42 / (mScale * mCamera.Transformation.M11));

            rect.Width = (int)((mCamera.ViewportSize.X / mScale) / mCamera.Transformation.M11);
            rect.Height = (int)((mCamera.ViewportSize.Y / mScale) / mCamera.Transformation.M11);
            if (rect.X < 0) rect.X = 0;
            if (rect.Y < 0) rect.Y = 0;
            if (rect.X + rect.Width >= mSize) rect.X = mSize - rect.Width - 1;
            if (rect.Y + rect.Height >= mSize) rect.Y = mSize - rect.Height - 1;
            return rect;
        }

        private void DrawRectangle(Rectangle rect)
        {
            var topLeft = new Vector2(rect.X, rect.Y);
            var topRight = new Vector2(rect.X+rect.Width, rect.Y);
            var bottomLeft = new Vector2(rect.X, rect.Y+rect.Height);
            var bottomRight = new Vector2(rect.X+rect.Width, rect.Y+rect.Height);
            DrawLine(topLeft, topRight);
            DrawLine(topLeft, bottomLeft);
            DrawLine(bottomLeft, bottomRight);
            DrawLine(topRight, bottomRight);
        }

        private void DrawLine(Vector2 start, Vector2 end)
        {
            // for horizontal lines
            for (var i = start.X; i <= end.X; i++)
            {
                mData[(int)(i + mSize * start.Y)] = mScreenBorder;
            }
            
            // for vertical lines
            for (var i = start.Y; i <= end.Y; i++)
            {
                mData[(int)(start.X + i * mSize)] = mScreenBorder;
            }
        }

        private Color LaneColor(int i)
        {
            var point = CalculateWorldPosition(i);
            // Console.WriteLine(point);
            if (mPlayers.A.DefendingLane.Contains(new Vector2(point.X, point.Y)))
            {
                return mColorLaneA;
            }
            if (mPlayers.B.DefendingLane.Contains(new Vector2(point.X, point.Y)))
            {
                return mColorLaneB;
            }

            return mColorBackground;
        }
        
        private void SetEntityColor(Lane lane)
        {
            // at first we only save the position of the selected entity, so we can draw it on top.
            // TODO needs to be adapted if we can select multiple entities at a given point
            var drawSelected = false;
            var selectedIndex = 0;

            foreach (var entity in lane.EntityGraph.AllEntities)
            {
                var index = CalculateMapIndexPosition(entity.Sprite.Position);
                var color = mColorBackground;

                if (entity.Selected)
                {
                    // dont draw it yet, but save the position and remember to draw it
                    drawSelected = true;
                    selectedIndex = index;
                }
                else
                {
                    switch (entity)
                    {
                        case Unit _:
                            color = mColorPlayerA;
                            break;
                        case Building _:
                            color = mColorPlayerB;
                            break;
                    }

                    SetPixelSquare(index, color);
                }
            }

            // draw the selected one on top
            if (drawSelected)
            {
                SetPixelSquare(selectedIndex, mColorSelected);
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
        
        
        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mSprite.Draw(spriteBatch, gameTime);
        }

        #endregion
    }
}