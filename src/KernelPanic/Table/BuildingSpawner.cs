using System;
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

        internal void Register(Building building, TileIndex tile)
        {
            building.State = BuildingState.Inactive;
            building.Sprite.Position = mGrid.GetTile(tile).Position;
            mHeatMap.Block(tile.ToPoint());
            Spawn(building);
        }

        private void Spawn(Building building)
        {
            building.State = BuildingState.Active;
            mSpawnAction(building);
        }
    }
}
