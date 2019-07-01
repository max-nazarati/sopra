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

        /// <summary>
        /// Registers a unit for spawning.
        /// </summary>
        /// <param name="unit">The unit to spawn.</param>
        /// <param name="atBase">If <c>true</c> the units position is modified to be at the lanes base.</param>
        internal void Register(Unit unit, bool atBase = true)
        {
            if (atBase)
            {
                var tile =
                    mGrid.LaneSide == Lane.Side.Left
                        ? new TileIndex(Grid.LaneWidthInTiles / 2, mGrid.LaneRectangle.Width - 1, 1)
                        : new TileIndex(mGrid.LaneRectangle.Height - Grid.LaneWidthInTiles / 2, 0, 1);
                unit.Sprite.Position = mGrid.GetTile(tile).Position;
            }

            mSpawnAction(unit);
        }

        // maybe this will get somehow transformed into a parent class Spawner
        internal void Register(Entity clone, int? x=null, int? y=null)
        {
            if (clone is Unit unit)
            {
                Register(unit);
            }
            
            /*
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
            */
        }
    }
}