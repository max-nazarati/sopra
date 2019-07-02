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

        internal void Register(Building building, Vector2 position)
        {
            if (mGrid.TileFromWorldPoint(position) is TileIndex tile)
            {
                building.Sprite.Position = mGrid.GetTile(tile).Position;
                mHeatMap.Block(tile.ToPoint());
                mSpawnAction(building);
                return;
            }

            Console.WriteLine("Requested an out-of bounds placement of a building: " + building);
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
