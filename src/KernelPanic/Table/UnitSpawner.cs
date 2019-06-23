using System;
using KernelPanic.Entities;

namespace KernelPanic.Table
{
    internal sealed class UnitSpawner
    {
        private readonly Action<Unit> mSpawnAction;
        private readonly Grid mGrid;

        public UnitSpawner(Grid grid, Action<Unit> spawnAction)
        {
            mGrid = grid;
            mSpawnAction = spawnAction;
        }

        internal void Register(Unit unit)
        {
            var spawnTile =
                mGrid.LaneSide == Lane.Side.Left
                    ? new TileIndex(Grid.LaneWidthInTiles / 2, mGrid.LaneRectangle.Width - 1, 1)
                    : new TileIndex(mGrid.LaneRectangle.Height - Grid.LaneWidthInTiles / 2, 0, 1);
            unit.Sprite.Position = mGrid.GetTile(spawnTile).Position;
            mSpawnAction(unit);
        }
    }
}