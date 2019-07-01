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
                var buildingPosition = mGrid.GridPointFromWorldPoint(position)?.Position;
                if (buildingPosition != null)
                    clonedBuilding.Sprite.Position = (Vector2) buildingPosition;
                else
                {
                    Console.WriteLine("Error, cant register a turret here");
                    return;
                }

                mSpawnAction(clonedBuilding);
                mHeatMap.Block(Grid.CoordinatePositionFromScreen(clonedBuilding.Sprite.Position));
            }
        }
        
        private void RegisterBuilding(Building building, int x, int y)
        {
            var buildingPosition =
                mGrid.LaneSide != Lane.Side.Left
                    ? new TileIndex(Grid.LaneWidthInTiles / 2, mGrid.LaneRectangle.Width - 1, 1)
                    : new TileIndex(mGrid.LaneRectangle.Height - Grid.LaneWidthInTiles / 2, 0, 1);
            building.Sprite.Position = mGrid.GetTile(buildingPosition).Position;
            mSpawnAction(building);
        }
    }
}
