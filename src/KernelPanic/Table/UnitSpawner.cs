using System;
using System.Collections.Generic;
using KernelPanic.Entities;
using KernelPanic.Entities.Units;
using Microsoft.Xna.Framework;

namespace KernelPanic.Table
{
    internal sealed class UnitSpawner
    {
        private readonly Grid mGrid;
        private readonly Action<Unit> mSpawnAction;
        private readonly Queue<Troupe> mUnits = new Queue<Troupe>();

        // TODO: We probably want to have more logic instead, maybe something like “is the tile free”.
        private readonly CooldownComponent mSpawnCooldown = new CooldownComponent(TimeSpan.FromSeconds(0.5));

        public UnitSpawner(Grid grid, Action<Unit> spawnAction)
        {
            mGrid = grid;
            mSpawnAction = spawnAction;
            mSpawnCooldown.CooledDown += component =>
            {
                if (mUnits.Count == 0)
                    return;

                component.Reset();
                var unit = mUnits.Dequeue();
                spawnAction(unit);
            };
        }

        /// <summary>
        /// Calculates the position where to spawn units.
        /// </summary>
        private Vector2 BasePosition(Unit unit)
        {
            int headstart = 0;
            if (unit is Bug)
            {
                headstart = -4;
            }
            else if (unit is Virus)
            {
                headstart = -2;
            }
            else if (unit is Trojan)
            {
                headstart = 2;
            }
            else if (unit is Nokia)
            {
                headstart = 4;
            }
            var tile =
                mGrid.LaneSide == Lane.Side.Left
                    ? new TileIndex(headstart + Grid.LaneWidthInTiles / 2, mGrid.LaneRectangle.Width - 1, 1)
                    : new TileIndex(-headstart + mGrid.LaneRectangle.Height - Grid.LaneWidthInTiles / 2, 0, 1);
            return mGrid.GetTile(tile).Position;
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
                unit.Sprite.Position = BasePosition(unit);
            }

            if (!(unit is Troupe troupe))
            {
                // Non-troupes are spawned instantly.
                mSpawnAction(unit);
                return;
            }

            // Enqueue the unit and re-enable the timer, either it is already enabled or it has reached zero and there
            // was no unit to dequeue. In the latter case re-enabling invokes the callback again during the next
            // update cycle and now there is a unit to dequeue.
            mUnits.Enqueue(troupe);
            mSpawnCooldown.Enabled = true;
        }

        internal void Update(GameTime gameTime)
        {
            mSpawnCooldown.Update(gameTime);
        }
    }
}