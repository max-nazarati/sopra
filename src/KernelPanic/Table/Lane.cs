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

        private HeatMap mCoordinateMap;
        private VectorField mVectorField;
        // private UnitSpawner mUnitSpawner;
        // private BuildingSpawner mBuildingSpawner;

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
            EntityGraph = new EntityGraph(LaneBoundsInPixel(laneSide), sprites);
            Target = new Base();
            mSpriteManager = sprites;
        }

        internal bool Contains(Vector2 point) => mGrid.Contains(point);

        public void Update(GameTime gameTime, InputManager inputManager)
        {
            var mouse = inputManager.TranslatedMousePosition;
            if (inputManager.KeyPressed(Keys.T))
            {
                // It seems we can't use pattern matching here because of compiler-limitations.
                var gridPoint = mGrid.GridPointFromWorldPoint(mouse);
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
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mGrid.Draw(spriteBatch, gameTime);
            EntityGraph.Draw(spriteBatch, gameTime);
        }
        
        public void DrawMinimap(SpriteBatch spriteBatch, Rectangle rectangle)
        {    
            throw new NotImplementedException();
        }
        

        public void InitCoordinateMap()
        {
            mCoordinateMap = new HeatMap(mGrid.LaneRectangle.Width, mGrid.LaneRectangle.Height);
            int xAxisReflection = 1;
            int xAxisTranslation = 0;
            if (mGrid.LaneSide == Side.Left)
            {
                xAxisReflection = -1;
                xAxisTranslation = mGrid.LaneRectangle.Width;
            }

            for (int i = Grid.LaneWidthInTiles; i < mGrid.LaneRectangle.Height - Grid.LaneWidthInTiles; i++)
            {
                for (int j = 0; j < mGrid.LaneRectangle.Width - Grid.LaneWidthInTiles; j++)
                {
                    mCoordinateMap.mMap[i, j * xAxisReflection + xAxisTranslation] = HeatMap.Blocked;
                }
            }
        }
    }
}
