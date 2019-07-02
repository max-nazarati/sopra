using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KernelPanic.Data;
using KernelPanic.Entities;
using KernelPanic.Input;
using KernelPanic.PathPlanning;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KernelPanic.Table
{
    internal sealed class Lane
    {
        /// <summary>
        /// Left and Right lane
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Side
        {
            /// <summary>
            /// Describes the lane which is shaped like an opening bracket ›[‹.
            /// </summary>
            Left,

            /// <summary>
            /// Describes the lane which is shaped like an closing bracket ›]‹.
            /// </summary>
            Right
        }

        #region Properties

        internal readonly SpriteManager mSpriteManager; // TODO write getonly property
        internal readonly SoundManager mSounds; // TODO write getonly property

        [JsonProperty]
        internal Base Target { get; /* required for deserialization */ private set; }

        [JsonProperty]
        private readonly Side mLaneSide;

        private Grid mGrid;
        private HeatMap mHeatMap;
        private VectorField mVectorField;

        internal EntityGraph EntityGraph { get; private set; }
        internal UnitSpawner UnitSpawner { get; private set; }
        internal BuildingSpawner BuildingSpawner { get; private set; }

        internal Rectangle GridRectangle() => mGrid.LaneRectangle;
        internal int LaneWidthInTiles() => Grid.LaneWidthInTiles;
        
        // private BuildingSpawner mBuildingSpawner;

        #endregion

        #region Size, position and bound functions

        private static Rectangle LaneBoundsInTiles(Side laneSide) =>
            new Rectangle(laneSide == Side.Left ? 0 : 30, 0, 18, 42);

        private static Rectangle LaneBoundsInPixel(Side laneSide)
        {
            var bounds = LaneBoundsInTiles(laneSide);
            bounds.X *= Grid.KachelSize;
            bounds.Y *= Grid.KachelSize;
            bounds.Width *= Grid.KachelSize;
            bounds.Height *= Grid.KachelSize;
            return bounds;
        }

        private static Point BasePosition(Side laneSide)
        {
            if (laneSide == Side.Right)
                return new Point(1);

            var position = LaneBoundsInTiles(laneSide).Size;
            position.X -= 2;
            position.Y -= 2;
            return position;
        }

        internal static Rectangle LeftBounds => LaneBoundsInPixel(Side.Left);
        internal static Rectangle RightBounds => LaneBoundsInPixel(Side.Right);

        internal bool Contains(Vector2 point) => mGrid.Contains(point);

        #endregion

        #region Constructors

        public Lane(Side laneSide, SpriteManager sprites, SoundManager sounds) : this(sprites, sounds)
        {
            mLaneSide = laneSide;
            Target = new Base(BasePosition(laneSide));
            Initialize();
        }

        internal Lane(SpriteManager sprites, SoundManager sounds)
        {
            mSpriteManager = sprites;
            mSounds = sounds;
        }

        /// <summary>
        /// Performs the initialization common to deserialized lanes and lanes created at runtime.
        /// </summary>
        /// <param name="entities">Entities to be added to the <see cref="EntityGraph"/> and the <see cref="mHeatMap"/>.</param>
        private void Initialize(IReadOnlyCollection<Entity> entities = null)
        {
            mGrid = new Grid(LaneBoundsInTiles(mLaneSide), mSpriteManager, mLaneSide);
            mHeatMap = new HeatMap(mGrid.LaneRectangle.Width, mGrid.LaneRectangle.Height);
            var obstacleMatrix = new ObstacleMatrix(mGrid, 1, false);
            EntityGraph = new EntityGraph(LaneBoundsInPixel(mLaneSide), mSpriteManager);
            UnitSpawner = new UnitSpawner(mGrid, EntityGraph.Add);
            BuildingSpawner = new BuildingSpawner(mGrid, mHeatMap, EntityGraph.Add);
            
            if (entities?.Count > 0)
            {
                EntityGraph.Add(entities);
                obstacleMatrix.Rasterize(entities, mGrid.Bounds, entity => entity is Building);
            }

            foreach (var tileIndex in obstacleMatrix.Obstacles)
                mHeatMap.Block(tileIndex.ToPoint());

            UpdateHeatMap();
        }

        #endregion

        #region Update

        internal void Update(GameTime gameTime, InputManager inputManager, Owner owner)
        {
            // It seems we can't use pattern matching here because of compiler-limitations.
            var gridPoint = mGrid.GridPointFromWorldPoint(inputManager.TranslatedMousePosition);
            if (gridPoint != null && inputManager.KeyPressed(Keys.T))
            {
                var (position, size) = gridPoint.Value;
                if (!EntityGraph.HasEntityAt(position))
                {
                    mSounds.PlaySound(SoundManager.Sound.TowerPlacement);
                    EntityGraph.Add(Tower.CreateTower(position, size, mSpriteManager, mSounds
                        , StrategicTower.Towers.CursorShooter));
                    mHeatMap.Block(Grid.CoordinatePositionFromScreen(position));
                    UpdateHeatMap();
                }
            }
            
            if (gridPoint != null && inputManager.KeyPressed(Keys.R))
            {
                var (position, size) = gridPoint.Value;
                if (!EntityGraph.HasEntityAt(position))
                {
                    mSounds.PlaySound(SoundManager.Sound.TowerPlacement);
                    EntityGraph.Add(Tower.CreateTower(position, size, mSpriteManager, mSounds
                        , StrategicTower.Towers.WifiRouter));
                    mHeatMap.Block(Grid.CoordinatePositionFromScreen(position));
                    UpdateHeatMap();
                }
            }
            
            if (gridPoint != null && inputManager.KeyPressed(Keys.Z))
            {
                var (position, size) = gridPoint.Value;
                if (!EntityGraph.HasEntityAt(position))
                {
                    mSounds.PlaySound(SoundManager.Sound.TowerPlacement);
                    EntityGraph.Add(Tower.CreateTower(position, size, mSpriteManager, mSounds
                        , StrategicTower.Towers.CdThrower));
                    mHeatMap.Block(Grid.CoordinatePositionFromScreen(position));
                    UpdateHeatMap();
                }
            }

            UpdateHeatMap();

            var positionProvider = new PositionProvider(mGrid, EntityGraph, mSpriteManager, mVectorField, Target, owner);
            EntityGraph.Update(positionProvider, gameTime, inputManager);
            UnitSpawner.Update(gameTime);
        }

        #endregion

        #region Drawing

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mGrid.Draw(spriteBatch, gameTime);
            if (mGrid.LaneSide == Side.Left) Visualize(spriteBatch, gameTime);
            EntityGraph.Draw(spriteBatch, gameTime);
        }
        
        public void DrawMinimap(SpriteBatch spriteBatch, Rectangle rectangle)
        {    
            throw new NotImplementedException();
        }

        #endregion

        #region Heat Map Handling

        private void UpdateHeatMap()
        {
            BreadthFirstSearch.UpdateHeatMap(mHeatMap, Target.HitBox);
            mVectorField = new VectorField(mHeatMap);
        }

        private void Visualize(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Visualizer visualizer;

            if (DebugSettings.VisualizeHeatMap)
            {
                visualizer = mHeatMap.Visualize(mGrid, mSpriteManager);
                visualizer.Draw(spriteBatch, gameTime);
            }

            if (DebugSettings.VisualizeVectors)
            {
                visualizer = mVectorField.Visualize(mGrid, mSpriteManager);
                visualizer.Draw(spriteBatch, gameTime);
            }

            if (!DebugSettings.VisualizeHeatMap && !DebugSettings.VisualizeVectors)
                return;

            var tileVisualizer = TileVisualizer.Border(mGrid, mSpriteManager);
            tileVisualizer.Append(Target.HitBox, Color.Blue);
            tileVisualizer.Draw(spriteBatch, gameTime);
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Stores/receives the entities during serialization. Don't use it outside of this!
        /// </summary>
        [JsonProperty]
        internal List<Entity> mEntitiesSerializing;

        [OnSerializing]
        private void BeforeSerialization(StreamingContext context)
        {
            // Store the current entities.
            mEntitiesSerializing = new List<Entity>(EntityGraph);
        }

        [OnSerialized]
        private void AfterSerialization(StreamingContext context)
        {
            // Reset this secondary storage.
            mEntitiesSerializing = null;
        }

        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            Initialize(mEntitiesSerializing);
            mEntitiesSerializing = null;
        }

        #endregion
    }
}
