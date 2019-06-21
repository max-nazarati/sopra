﻿using System;
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
            /// Describes the lane which is shaped like an opening bracket ›]‹.
            /// </summary>
            Right
        }

        private readonly SpriteManager mSpriteManager;
        private readonly SoundManager mSounds;


        [JsonProperty]
        internal Base Target { get; }

        [JsonProperty]
        private Side mLaneSide;

        private Grid mGrid;
        private HeatMap mHeatMap;
        private VectorField mVectorField;

        internal EntityGraph EntityGraph { get; private set; }

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
            mLaneSide = laneSide;
            mGrid = new Grid(LaneBoundsInTiles(laneSide), sprites, laneSide);
            EntityGraph = new EntityGraph(LaneBoundsInPixel(laneSide), sprites);
            switch (mGrid.LaneSide)
            {
                case Side.Left:
                    Target = new Base(new Vector2(mGrid.LaneRectangle.Width - 2, mGrid.LaneRectangle.Height - 2));
                    break;
                case Side.Right:
                    Target = new Base(new Vector2(1, 1));
                    break;
            }
            mSpriteManager = sprites;
            InitHeatMap();
            UpdateHeatMap();
        }

        internal Lane(SpriteManager sprites, SoundManager sounds)
        {
            Target = new Base();
            mSpriteManager = sprites;
            mSounds = sounds;
        }

        internal bool Contains(Vector2 point) => mGrid.Contains(point);

        internal void Update(GameTime gameTime, InputManager inputManager, Owner owner)
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
                        mHeatMap.Set(Grid.CoordinatePositionFromScreen(position), HeatMap.Blocked);
                        UpdateHeatMap();
                        mSounds.PlaySound(SoundManager.Sound.TowerPlacement);
                    }
                }
            }

            var positionProvider = new PositionProvider(mGrid, EntityGraph, mSpriteManager, mVectorField, Target, owner);
            EntityGraph.Update(positionProvider, gameTime, inputManager);
        }

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
        

        public void InitHeatMap()
        {
            mHeatMap = new HeatMap(mGrid.LaneRectangle.Width, mGrid.LaneRectangle.Height);

            switch (mGrid.LaneSide)
            {
                case Side.Left:
                    for (int y = Grid.LaneWidthInTiles; y < mGrid.LaneRectangle.Height - Grid.LaneWidthInTiles; y++)
                    {
                        for (int x = Grid.LaneWidthInTiles; x < mGrid.LaneRectangle.Width; x++)
                        {
                            mHeatMap.mMap[y, x] = HeatMap.Blocked;
                        }
                    }
                    break;
                case Side.Right:
                    for (int y = Grid.LaneWidthInTiles; y < mGrid.LaneRectangle.Height - Grid.LaneWidthInTiles; y++)
                    {
                        for (int x = 0; x < mGrid.LaneRectangle.Width - Grid.LaneWidthInTiles; x++)
                        {
                            mHeatMap.mMap[y, x] = HeatMap.Blocked;
                        }
                    }
                    break;
            }
        }

        public void UpdateHeatMap()
        {
            List<Point> basePoints = new List<Point>();
            foreach (var basePoint in Target.GetHitBox())
                {
                basePoints.Add(new Point((int)basePoint.X, (int)basePoint.Y));
                }
            
            BreadthFirstSearch bfs = new BreadthFirstSearch(mHeatMap, basePoints);
            bfs.UpdateVectorField();
            mHeatMap = bfs.HeatMap;
            mVectorField = bfs.VectorField;
        }

        private void Visualize(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!DebugSettings.VisualizeHeatMap)
                return;

            var visualizer = mHeatMap.CreateVisualization(mGrid, mSpriteManager, false);
            visualizer.Draw(spriteBatch, gameTime);
        }

        #region Serialization

        /// <summary>
        /// Stores/receives the entities during serialization. Don't use it outside of this!
        /// </summary>
        [JsonProperty]
        internal List<Entity> mEntitiesSerializing;

        [OnSerializing]
        private void BeforeSerialization(StreamingContext context)
        {
            mEntitiesSerializing = new List<Entity>(EntityGraph);
        }

        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            mGrid = new Grid(LaneBoundsInTiles(mLaneSide), mSpriteManager, mLaneSide);
            EntityGraph = new EntityGraph(LaneBoundsInPixel(mLaneSide), mSpriteManager);
            foreach (var entity in mEntitiesSerializing)
            {
                EntityGraph.Add(entity);
            }
            
            InitHeatMap();

            var obstacles = new ObstacleMatrix(mGrid);
            obstacles.Rasterize(EntityGraph, mGrid.Bounds, entity => entity is Building);
            foreach (var obstacle in obstacles.Obstacles)
            {
                mHeatMap.Set(obstacle.ToPoint(), HeatMap.Blocked);
            }
            UpdateHeatMap();
        }

        #endregion
    }
}
