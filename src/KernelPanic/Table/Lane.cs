﻿using System;
using System.Collections.Generic;
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

        private HeatMap mHeatMap;
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
            InitHeatMap();
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
                        mHeatMap.Set(Grid.CoordinatePositionFromScreen(position), HeatMap.Blocked);
                        UpdateHeatMap();
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
            switch (mGrid.LaneSide)
            {
                case Side.Left:
                    basePoints.Add(new Point(mGrid.LaneRectangle.Width - 1, mGrid.LaneRectangle.Height - 1));
                    basePoints.Add(new Point(mGrid.LaneRectangle.Width - 1, mGrid.LaneRectangle.Height - 2));
                    basePoints.Add(new Point(mGrid.LaneRectangle.Width - 2, mGrid.LaneRectangle.Height - 1));
                    basePoints.Add(new Point(mGrid.LaneRectangle.Width - 2, mGrid.LaneRectangle.Height - 2));
                    break;
                case Side.Right:
                    basePoints.Add(new Point(0, 0));
                    basePoints.Add(new Point(0, 1));
                    basePoints.Add(new Point(1, 0));
                    basePoints.Add(new Point(1, 1));
                    break;
            }
            BreadthFirstSearch bfs = new BreadthFirstSearch(mHeatMap, basePoints);
            bfs.UpdateHeatMap();
            bfs.UpdateVectorField();
            mHeatMap = bfs.HeatMap;
            mVectorField = bfs.VectorField;
        }
    }
}
