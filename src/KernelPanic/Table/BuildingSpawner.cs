﻿using System;
using Microsoft.Xna.Framework;
using KernelPanic.Entities;
using KernelPanic.Data;

namespace KernelPanic.Table
{
    internal sealed class BuildingSpawner
    {
        private readonly Action<Building> mSpawnAction;
        private readonly Grid mGrid;
        private readonly HeatMap mHeatMap;

        public BuildingSpawner(Grid grid, HeatMap heatMap, Action<Building> spawnAction)
        {
            mGrid = grid;
            mSpawnAction = spawnAction;
            mHeatMap = heatMap;
        }

        internal void Register(Building building, Point tile)
        {
            building.Sprite.Position = mGrid.GetTile(new TileIndex(tile, 1)).Position;
            mHeatMap.Block(tile);
            mSpawnAction(building);
        }
    }
}
