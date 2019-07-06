using System.Collections.Generic;
using System.Runtime.Serialization;
using KernelPanic.Data;
using KernelPanic.Entities;
using KernelPanic.Input;
using KernelPanic.PathPlanning;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        private readonly SpriteManager mSpriteManager;

        [JsonProperty]
        internal Base Target { get; /* required for deserialization */ private set; }

        [JsonProperty]
        private readonly Side mLaneSide;

        private HeatMap mHeatMap;
        private VectorField mVectorField;

        internal EntityGraph EntityGraph { get; private set; }
        internal UnitSpawner UnitSpawner { get; private set; }
        internal BuildingSpawner BuildingSpawner { get; private set; }
        internal Grid Grid { get; private set; }

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

        internal bool Contains(Vector2 point) => Grid.Contains(point);

        #endregion

        #region Constructors

        public Lane(Side laneSide, SpriteManager sprites) : this(sprites)
        {
            mLaneSide = laneSide;
            Target = new Base(BasePosition(laneSide));
            Initialize();
        }

        internal Lane(SpriteManager sprites)
        {
            mSpriteManager = sprites;
        }

        /// <summary>
        /// Performs the initialization common to deserialized lanes and lanes created at runtime.
        /// </summary>
        /// <param name="entities">Entities to be added to the <see cref="EntityGraph"/> and the <see cref="mHeatMap"/>.</param>
        private void Initialize(IReadOnlyCollection<Entity> entities = null)
        {
            Grid = new Grid(LaneBoundsInTiles(mLaneSide), mSpriteManager, mLaneSide);
            mHeatMap = new HeatMap(Grid.LaneRectangle.Width, Grid.LaneRectangle.Height);
            EntityGraph = new EntityGraph(LaneBoundsInPixel(mLaneSide));
            UnitSpawner = new UnitSpawner(Grid, EntityGraph.Add);
            BuildingSpawner = new BuildingSpawner(Grid, mHeatMap, EntityGraph.Add);
            
            var obstacleMatrix = new ObstacleMatrix(Grid, 1, false);
            if (entities?.Count > 0)
            {
                EntityGraph.Add(entities);
                obstacleMatrix.Raster(entities, entity => entity is Building);
            }

            foreach (var tileIndex in obstacleMatrix.Obstacles)
                mHeatMap.Block(tileIndex.ToPoint());

            UpdateHeatMap();
        }

        #endregion

        #region Update

        internal void Update(GameTime gameTime, InputManager inputManager, Owner owner)
        {
            // Maybe we want to do this only when new buildings were placed.
            UpdateHeatMap();

            var positionProvider = new PositionProvider(Grid, EntityGraph, mSpriteManager, mVectorField, Target, owner);
            EntityGraph.Update(positionProvider, gameTime, inputManager);
            UnitSpawner.Update(gameTime);
        }

        #endregion

        #region Drawing

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Grid.Draw(spriteBatch, gameTime);
            if (Grid.LaneSide == Side.Left) Visualize(spriteBatch, gameTime);
            EntityGraph.Draw(spriteBatch, gameTime);
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
                visualizer = mHeatMap.Visualize(Grid, mSpriteManager);
                visualizer.Draw(spriteBatch, gameTime);
            }

            if (DebugSettings.VisualizeVectors)
            {
                visualizer = mVectorField.Visualize(Grid, mSpriteManager);
                visualizer.Draw(spriteBatch, gameTime);
            }

            if (!DebugSettings.VisualizeHeatMap && !DebugSettings.VisualizeVectors)
                return;

            var tileVisualizer = TileVisualizer.Border(Grid, mSpriteManager);
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
            mEntitiesSerializing = new List<Entity>(EntityGraph.AllEntities);
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
