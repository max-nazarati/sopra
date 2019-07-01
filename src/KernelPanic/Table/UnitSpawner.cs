using System;
using KernelPanic.Entities;

namespace KernelPanic.Table
{
    internal sealed class UnitSpawner
    {
        private readonly Action<Entity> mSpawnAction;
        private readonly Grid mGrid;

        public UnitSpawner(Grid grid, Action<Entity> spawnAction)
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

        private void RegisterBuilding(Building building, int x, int y)
        {
            var buildingPosition =
                mGrid.LaneSide != Lane.Side.Left
                    ? new TileIndex(Grid.LaneWidthInTiles / 2, mGrid.LaneRectangle.Width - 1, 1)
                    : new TileIndex(mGrid.LaneRectangle.Height - Grid.LaneWidthInTiles / 2, 0, 1);
            building.Sprite.Position = mGrid.GetTile(buildingPosition).Position;
            mSpawnAction(building);

        }
        
        internal void Register(Entity clone, int? x=null, int? y=null)
        {
            if (clone is Unit unit)
            {
                Register(unit);
            }

            if (clone is Building building)
            {
                if (x is int xInt && y is int yInt)
                {
                    RegisterBuilding(building, xInt, yInt);
                }
                else
                {
                    // throw new NotSupportedException("Cant build a tower without position");
                }
            }
        }
    }
}