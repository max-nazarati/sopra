using System;
using Microsoft.Xna.Framework;
using KernelPanic.Entities;
using KernelPanic.Data;

namespace KernelPanic.Table
{
    class BuildingSpawner
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

        internal void Register(Building unit, Vector2 position)
        {
            if (mGrid.Contains(position))
            {
                var clonedBuilding = unit;
                clonedBuilding.Sprite.Position = (Vector2)mGrid.GridPointFromWorldPoint(position)?.Position;
                mSpawnAction(clonedBuilding);
                mHeatMap.Block(Grid.CoordinatePositionFromScreen(clonedBuilding.Sprite.Position));
            }
        }
    }
}
