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
        private struct SpawnQueue<T> where T : Troupe
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
                unit.SetInitialPosition(mInitialPosition);
                mQueue.Enqueue(unit);
            }

            internal bool Spawn(EntityGraph entityGraph)
            {
                if (mQueue.Count == 0)
                    return false;

                var next = mQueue.Peek();
                if (entityGraph.HasIntersectingEntity(next, o => o is Troupe t && next.CollidesWith(t)))
                    return false;

                entityGraph.Add(mQueue.Dequeue());
                return true;
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
        private readonly Grid mGrid;
        private readonly Vector2[] mHeroSpawns;
        private SpawnQueue<Bug> mBugs;
        private SpawnQueue<Virus> mViruses;
        private SpawnQueue<Trojan> mTrojans;
        private SpawnQueue<Nokia> mNokias;
        private SpawnQueue<Thunderbird> mThunderbirds;
        private readonly Dictionary<TileIndex, SpawnQueue<Troupe>> mAdditionalSpawns =
            new Dictionary<TileIndex, SpawnQueue<Troupe>>();

        // Only units spawned somewhere in the lane use the cooldown.
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
            mGrid = grid;

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

            mSpawnCooldown.CooledDown += component =>
            {
                var spawned = false;
                foreach (var queue in mAdditionalSpawns.Values)
                {
                    spawned |= queue.Spawn(mEntityGraph);
                }
                
                if (spawned)
                    component.Reset();
            };
        }

        /// <summary>
        /// Registers a unit for spawning.
        /// </summary>
        /// <param name="troupe">The unit to spawn.</param>
        /// <param name="spawnTile">An alternative position compared to the base.</param>
        internal void Register(Troupe troupe, TileIndex spawnTile)
        {
            if (mAdditionalSpawns.TryGetValue(spawnTile, out var queue))
            {
                queue.Add(troupe);
                return;
            }
            
            queue = new SpawnQueue<Troupe>(mGrid.GetTile(spawnTile).Position);
            queue.Add(troupe);
            mAdditionalSpawns[spawnTile] = queue;
            mSpawnCooldown.Enabled = true;
        }

        /// <summary>
        /// Registers a unit for spawning.
        /// </summary>
        /// <param name="unit">The unit to spawn.</param>
        internal void Register(Unit unit)
        {
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
        }

        internal void Update(GameTime gameTime)
        {
            mBugs.Spawn(mEntityGraph);
            mViruses.Spawn(mEntityGraph);
            mTrojans.Spawn(mEntityGraph);
            mNokias.Spawn(mEntityGraph);
            mThunderbirds.Spawn(mEntityGraph);
            mSpawnCooldown.Update(gameTime);
        }
    }
}