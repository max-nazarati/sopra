using System;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Entities;
using KernelPanic.Entities.Units;
using Microsoft.Xna.Framework;

namespace KernelPanic.Table
{
    internal sealed class UnitSpawner
    {
        private struct SpawnQueue<T> where T : Entity
        {
            private readonly Queue<T> mQueue;
            private readonly Vector2 mInitialPosition;

            internal SpawnQueue(Vector2 spawnPoint)
            {
                mQueue = new Queue<T>();
                mInitialPosition = spawnPoint;
            }

            internal bool Empty => mQueue.Count == 0;

            internal void Add(T unit)
            {
                unit.Sprite.Position = mInitialPosition;
                mQueue.Enqueue(unit);
            }

            internal void Spawn(EntityGraph entityGraph)
            {
                if (mQueue.Count > 0)
                    entityGraph.Add(mQueue.Dequeue());
            }
        }

        private static Vector2 SpawnPoint(int row, int subTileCount, Grid grid)
        {
            var tile = grid.LaneSide == Lane.Side.Left
                ? new TileIndex(row, (grid.LaneRectangle.Width - 1) * subTileCount, subTileCount)
                : new TileIndex((grid.LaneRectangle.Height - 1) * subTileCount - row, 0, subTileCount);
            return grid.GetTile(tile).Position;
        }

        private readonly EntityGraph mEntityGraph;
        private readonly Vector2[] mHeroSpawns;
        private SpawnQueue<Bug> mBugs;
        private SpawnQueue<Virus> mViruses;
        private SpawnQueue<Trojan> mTrojans;
        private SpawnQueue<Nokia> mNokias;
        private SpawnQueue<Thunderbird> mThunderbirds;

        // TODO: We probably want to have more logic instead, maybe something like “is the tile free”.
        private readonly CooldownComponent mSpawnCooldown =
            new CooldownComponent(TimeSpan.FromSeconds(0.5), false) {Enabled = false};

        /// <summary>
        /// Returns <c>true</c> if there are no more troupes to spawn.
        /// </summary>
        internal bool Ready =>
            mBugs.Empty && mViruses.Empty && mTrojans.Empty && mNokias.Empty && mThunderbirds.Empty;

        public UnitSpawner(Grid grid, EntityGraph entityGraph)
        {
            mEntityGraph = entityGraph;

            mBugs = new SpawnQueue<Bug>(SpawnPoint(3, 2, grid));
            mViruses = new SpawnQueue<Virus>(SpawnPoint(5, 2, grid));
            mTrojans = new SpawnQueue<Trojan>(SpawnPoint(6, 1, grid));
            mNokias = new SpawnQueue<Nokia>(SpawnPoint(8, 1, grid));
            mThunderbirds = new SpawnQueue<Thunderbird>(SpawnPoint(4, 1, grid));

            mHeroSpawns = new[]
            {
                SpawnPoint(5, 1, grid),
                SpawnPoint(7, 1, grid),
                SpawnPoint(3, 1, grid),
                SpawnPoint(9, 1, grid),
                SpawnPoint(0, 1, grid)
            };

            mSpawnCooldown.CooledDown += Spawn;
        }

        private void Spawn(CooldownComponent component)
        {
            mBugs.Spawn(mEntityGraph);
            mViruses.Spawn(mEntityGraph);
            mTrojans.Spawn(mEntityGraph);
            mNokias.Spawn(mEntityGraph);
            mThunderbirds.Spawn(mEntityGraph);
            component.Reset();
        }

        /// <summary>
        /// Registers a unit for spawning.
        /// </summary>
        /// <param name="unit">The unit to spawn.</param>
        /// <param name="atBase">If <c>true</c> the units position is modified to be at the lanes base.</param>
        internal void Register(Unit unit, bool atBase = true)
        {
            // TODO: Handle spawning of units which don't want to start at the base.
            switch (unit)
            {
                case Hero hero:
                {
                    hero.Sprite.Position = mHeroSpawns.First(point => !mEntityGraph.HasEntityAt(point));
                    mEntityGraph.Add(hero);
                    return;
                }

                case Bug bug:
                    mBugs.Add(bug);
                    break;
                case Virus virus:
                    mViruses.Add(virus);
                    break;
                case Trojan trojan:
                    mTrojans.Add(trojan);
                    break;
                case Nokia nokia:
                    mNokias.Add(nokia);
                    break;
                case Thunderbird thunderbird:
                    mThunderbirds.Add(thunderbird);
                    break;
            }

            mSpawnCooldown.Enabled = true;
        }

        internal void Update(GameTime gameTime)
        {
            mSpawnCooldown.Update(gameTime);
        }
    }
}