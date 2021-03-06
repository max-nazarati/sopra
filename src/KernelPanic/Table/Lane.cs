﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Entities.Units;
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

        internal Point[] SpawnPoints => Base.SpawnPoints(LaneSize, mLaneSide);
        internal Point[] TargetPoints => Base.TargetPoints(LaneSize, mLaneSide);

        [JsonProperty]
        private readonly Side mLaneSide;

        internal EntityGraph EntityGraph { get; private set; }
        internal UnitSpawner UnitSpawner { get; private set; }
        internal BuildingSpawner BuildingSpawner { get; private set; }
        internal Grid Grid { get; private set; }

        private TroupePathData mTroupeData;

        #endregion

        #region Size, position and bound functions

        private static Point LaneSize => new Point(18, 42);

        private static Rectangle LaneBoundsInTiles(Side laneSide) =>
            new Rectangle(laneSide == Side.Left ? 0 : 30, 0, LaneSize.X, LaneSize.Y);

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

        internal bool Contains(Vector2 point) => Grid.Contains(point);

        private List<LaneBorder> LaneBorders
        {
            get
            {
                var targetSide = mLaneSide == Side.Left ? Side.Right : Side.Left;
                var borders = new List<LaneBorder>(8);
                borders.AddRange(LaneBorder.Borders(Grid.Bounds, Grid.KachelSize, true, targetSide));
                borders.AddRange(LaneBorder.Borders(Grid.PixelCutout, Grid.KachelSize, false));
                return borders;
            }
        }

        #endregion

        #region Constructors

        public Lane(Side laneSide, SpriteManager sprites) : this(sprites)
        {
            mLaneSide = laneSide;
            Target = new Base();
            Initialize();
        }

        internal Lane(SpriteManager sprites)
        {
            mSpriteManager = sprites;
        }

        /// <summary>
        /// Performs the initialization common to deserialized lanes and lanes created at runtime.
        /// </summary>
        private void Initialize()
        {
            Grid = new Grid(LaneBoundsInTiles(mLaneSide), mSpriteManager, mLaneSide);
            EntityGraph = new EntityGraph(LaneBorders);

            var incompleteBuildings = mEntitiesSerializing?
                    .OfType<Building>()
                    .Where(building => building.State == BuildingState.Inactive);
            BuildingSpawner = new BuildingSpawner(Grid, EntityGraph.Add, incompleteBuildings);
            UnitSpawner = new UnitSpawner(Grid, EntityGraph, mQueuedUnitsSerializing);

            if (mEntitiesSerializing?.Count > 0)
            {
                EntityGraph.Add(mEntitiesSerializing);
            }

            var collidingBuildings = mEntitiesSerializing?
                    .Select(entity => entity is Building b && !(b is ShockField) ? b : null)
                    .Where(building => building != null);
            mTroupeData = new TroupePathData(this, collidingBuildings);
            mTroupeData.Update();
        }

        #endregion

        #region Update

        internal sealed class UpdateData
        {
            public UpdateData(GameTime gameTime, InputManager inputManager, Owner owner, int waveIndex)
            {
                GameTime = gameTime;
                InputManager = inputManager;
                Owner = owner;
                WaveIndex = waveIndex;
            }

            private GameTime GameTime { get; }
            private InputManager InputManager { get; }
            private Owner Owner { get; }
            private int WaveIndex { get; }

            internal void Update(Lane lane)
            {
                lane.Update(GameTime, InputManager, Owner, WaveIndex);
            }
        }

        private void Update(GameTime gameTime, InputManager inputManager, Owner owner, int waveIndex)
        {
            mTroupeData.BuildingMatrix.ResetUpdatedStatus();
            
            // Block new buildings in the obstacle matrix.
            foreach (var position in BuildingSpawner.NewBuildings())
            {
                mTroupeData.BuildingMatrix[position] = true;
            }

            // Unblock sold buildings in the obstacle matrix.
            IEnumerable<Building> buildings = EntityGraph.Entities<Building>();
            foreach (var building in buildings.Where(building => building.WantsRemoval))
            {
                Vector2 position = building.Sprite.Position;
                TileIndex tileIndex = Grid.TileFromWorldPoint(position).GetValueOrDefault();
                mTroupeData.BuildingMatrix[tileIndex.ToPoint()] = false;
            }

            mTroupeData.Update();

            var positionProvider = new PositionProvider(Target, owner, Grid, EntityGraph, mTroupeData, mSpriteManager);
            if (waveIndex > 0)
                EntityGraph.Update(positionProvider, gameTime, inputManager);
            UnitSpawner.Update(gameTime);
            BuildingSpawner.Update(positionProvider);
        }

        #endregion

        #region Drawing

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Grid.Draw(spriteBatch, gameTime);
            mTroupeData.Visualize(mSpriteManager, spriteBatch, gameTime);
            EntityGraph.Draw(spriteBatch, gameTime);
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Stores/receives the entities during serialization. Don't use it outside of this!
        /// </summary>
        [JsonProperty]
        internal List<Entity> mEntitiesSerializing;
        
        /// <summary>
        /// Stores/receives the queued units during serialization. Don't use it outside of this!
        /// </summary>
        [JsonProperty]
        internal List<Troupe> mQueuedUnitsSerializing;

        [OnSerializing]
        private void BeforeSerialization(StreamingContext context)
        {
            // Store the current entities.
            mEntitiesSerializing = EntityGraph.AllEntities.ToList();
            mQueuedUnitsSerializing = UnitSpawner.QueuedUnits.ToList();
        }

        [OnSerialized]
        private void AfterSerialization(StreamingContext context)
        {
            // Reset this secondary storage.
            mEntitiesSerializing = null;
            mQueuedUnitsSerializing = null;
        }

        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            Initialize();
            mEntitiesSerializing = null;
            mQueuedUnitsSerializing = null;
        }

        #endregion
    }
}
