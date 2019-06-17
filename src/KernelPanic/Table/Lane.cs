using System;
using System.Runtime.Serialization;
using KernelPanic.Data;
using KernelPanic.Entities;
using KernelPanic.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic.Table
{
    [DataContract]
    internal sealed class Lane
    {
        /// <summary>
        /// Left and Right lane
        /// </summary>
        public enum Side
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

        [DataMember]
        internal EntityGraph EntityGraph { get; set; }
        internal Base Target { get; }

        [DataMember(Name = "Grid")]
        private readonly Grid mGrid;
        private readonly SpriteManager mSpriteManager;
        private static bool VISUAL_DEBUG;
        private SoundManager mSounds;

        private Side mLaneSide;
        private int mWidth = 16;
        private int mHeight = 42;
        private int mLaneWidth = 10;
        private HeatMap mCoordinateMap;
        private VectorField mVectorField;
        // private UnitSpawner mUnitSpawner;
        // private BuildingSpawner mBuildingSpawner;

        private AStar mAStar;

        private static Rectangle LaneBoundsInTiles(Side laneSide) =>
            new Rectangle(laneSide == Side.Left ? 0 : 32, 0, 16, 42);

        private static Rectangle LaneBoundsInPixel(Side laneSide)
        {
            var bounds = LaneBoundsInTiles(laneSide);
            bounds.X *= Grid.KachelSize;
            bounds.Y *= Grid.KachelSize;
            bounds.Width *= Grid.KachelSize;
            bounds.Height *= Grid.KachelSize;
            return bounds;
        }

        internal static Rectangle LeftBounds => LaneBoundsInPixel(Side.Left);
        internal static Rectangle RightBounds => LaneBoundsInPixel(Side.Right);

        public Lane(Side laneSide, SpriteManager sprites, SoundManager sounds)
        {
            mSounds = sounds;
            mGrid = new Grid(LaneBoundsInTiles(laneSide), sprites, laneSide);
            EntityGraph = new EntityGraph(LaneBoundsInPixel(laneSide), new ObstacleMatrix(mGrid), sprites);
            Target = new Base();
            mSpriteManager = sprites;
            
            if (VISUAL_DEBUG)
            {
                InitAStar(sprites);
            }
        }

        internal bool Contains(Vector2 point) => mGrid.Contains(point);

        public void Update(GameTime gameTime, InputManager inputManager)
        {
            var mouse = inputManager.TranslatedMousePosition;
            if (inputManager.KeyPressed(Keys.T))
            {
                // It seems we can't use pattern matching here because of compiler-limitations.
                var gridPoint = mGrid.GridPointFromWorldPoint(mouse, 2);
                if (gridPoint != null)
                {
                    var (position, size) = gridPoint.Value;
                    if (!EntityGraph.HasEntityAt(position))
                    {
                        EntityGraph.Add(Tower.Create(position, size, mSpriteManager, mSounds));
                        mSounds.PlaySound(SoundManager.Sound.TowerPlacement);
                    }
                }
            }


            var positionProvider = new PositionProvider(mGrid, EntityGraph, mSpriteManager);
            EntityGraph.Update(positionProvider, gameTime, inputManager);
            if (VISUAL_DEBUG)
            {
                mAStar.Update(inputManager);
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mGrid.Draw(spriteBatch, gameTime);
            EntityGraph.Draw(spriteBatch, gameTime);
            if (VISUAL_DEBUG)
            {
               mAStar.Draw(spriteBatch, gameTime); // visual demo
            }
               
        }
        
        public void DrawMinimap(SpriteBatch spriteBatch, Rectangle rectangle)
        {    
            throw new NotImplementedException();
        }
        

        public void InitCoordinateMap()
        {
            mCoordinateMap = new HeatMap(mWidth, mHeight);
            int xAxisReflection = 1;
            int xAxisTranslation = 0;
            if (mLaneSide == Side.Left)
            {
                xAxisReflection = -1;
                xAxisTranslation = mWidth;
            }

            for (int i = mLaneWidth; i < mHeight - mLaneWidth; i++)
            {
                for (int j = 0; j < mWidth - mLaneWidth; j++)
                {
                    mCoordinateMap.mMap[i, j * xAxisReflection + xAxisTranslation] = HeatMap.Blocked;
                }
            }
        }
        
        // -------------------------- A STAR DEMO ----------------------------------------------------------------------
        private void InitAStar(SpriteManager sprite, int obstacleEnv=2)
        {
            // set start and target
            Point start = new Point(0, 0);
            Point target = new Point(0, 15);
            
            mAStar = new AStar(mGrid.CoordSystem, start, target, sprite);
            mAStar.ChangeObstacleEnvironment(obstacleEnv);
       
            mAStar.CalculatePath();
        }
        // ------------------------------------------------------------------------------------------------------------
    }
}
