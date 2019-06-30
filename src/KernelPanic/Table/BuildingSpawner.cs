using System;
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

        internal void Register(Building unit)
        {
            var spawnTile =
                mGrid.LaneSide == Lane.Side.Left
                    ? new TileIndex(1 + Grid.LaneWidthInTiles / 2, mGrid.LaneRectangle.Width - 1, 1)
                    : new TileIndex(mGrid.LaneRectangle.Height - Grid.LaneWidthInTiles / 2, 0, 1);
            unit.Sprite.Position = mGrid.GetTile(spawnTile).Position;
            mSpawnAction(unit);
            mHeatMap.Block(Grid.CoordinatePositionFromScreen(unit.Sprite.Position));
        }
    }
}
