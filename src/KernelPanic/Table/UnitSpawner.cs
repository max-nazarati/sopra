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
            private readonly Vector2[] mSpawnPoints;
            private int mSpawnIndex;
            private TimeSpan mTimeTillNextSpawn;

            internal SpawnQueue(params Vector2[] spawnPoints)
            {
                mSpawnIndex = 0;
                mQueue = new Queue<T>();
                mSpawnPoints = spawnPoints;
                mTimeTillNextSpawn = TimeSpan.Zero;
            }

            internal bool Empty => mQueue.Count == 0;

            internal void Add(T unit)
            {
                unit.Sprite.Position = mSpawnPoints[mSpawnIndex];
                mSpawnIndex = (mSpawnIndex + 1) % mSpawnPoints.Length;
                mQueue.Enqueue(unit);
            }

            internal void Spawn(GameTime gameTime, EntityGraph entityGraph)
            {
                if (Empty)
                    return;

                if (mTimeTillNextSpawn > TimeSpan.Zero)
                {
                    mTimeTillNextSpawn -= gameTime.ElapsedGameTime;
                    return;
                }

                SpawnNow(entityGraph);
                mTimeTillNextSpawn = SpawnDelay;
            }

            internal bool SpawnNow(EntityGraph entityGraph)
            {
                if (Empty)
                    return false;

                var next = mQueue.Peek();
                if (entityGraph.HasIntersectingEntity(next, o => o is Troupe t && next.CollidesWith(t)))
                    return false;

                entityGraph.Add(mQueue.Dequeue());
                return true;
            }
            
            private static TimeSpan SpawnDelay => TimeSpan.FromSeconds(0.5);

            internal IEnumerable<Troupe> QueuedUnits => mQueue;
        }

        private static Vector2 SpawnPoint(Grid grid, int row, int subTileCount = 1)
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

        public UnitSpawner(Grid grid, EntityGraph entityGraph, IReadOnlyCollection<Troupe> restored)
        {
            mEntityGraph = entityGraph;
            mGrid = grid;

            mBugs = new SpawnQueue<Bug>(SpawnPoint(grid, 3, 2));
            mViruses = new SpawnQueue<Virus>(SpawnPoint(grid, 5, 2));
            mTrojans = new SpawnQueue<Trojan>(SpawnPoint(grid, 6));
            mNokias = new SpawnQueue<Nokia>(SpawnPoint(grid, 8));
            mThunderbirds = new SpawnQueue<Thunderbird>(
                SpawnPoint(grid, 1),
                SpawnPoint(grid, 3),
                SpawnPoint(grid, 6),
                SpawnPoint(grid, 8));

            mHeroSpawns = new[]
            {
                SpawnPoint(grid, 5),
                SpawnPoint(grid, 7),
                SpawnPoint(grid, 3),
                SpawnPoint(grid, 9),
                SpawnPoint(grid, 0)
            };

            mSpawnCooldown.CooledDown += component =>
            {
                var spawned = false;
                foreach (var queue in mAdditionalSpawns.Values)
                {
                    spawned |= queue.SpawnNow(mEntityGraph);
                }
                
                if (spawned)
                    component.Reset();
            };
            
            if (restored == null)
                return;

            // We restore all units from the base and ignore the additional spawns.
            foreach (var troupe in restored)
            {
                Register(troupe);
            }
        }

        /// <summary>
        /// Registers a unit for spawning.
        /// </summary>
        /// <param name="troupe">The unit to spawn.</param>
        /// <param name="spawnTile">An alternative position compared to the base.</param>
        internal void Register(Troupe troupe, TileIndex spawnTile)
        {
            if (!mAdditionalSpawns.TryGetValue(spawnTile, out var queue))
            {
                queue = new SpawnQueue<Troupe>(mGrid.GetTile(spawnTile).Position);
                mAdditionalSpawns[spawnTile] = queue;
            }

            queue.Add(troupe);
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
            mBugs.Spawn(gameTime, mEntityGraph);
            mViruses.Spawn(gameTime, mEntityGraph);
            mTrojans.Spawn(gameTime, mEntityGraph);
            mNokias.Spawn(gameTime, mEntityGraph);
            mThunderbirds.Spawn(gameTime, mEntityGraph);
            mSpawnCooldown.Update(gameTime);
        }

        internal IEnumerable<Troupe> QueuedUnits =>
            mBugs.QueuedUnits
                .Concat(mViruses.QueuedUnits)
                .Concat(mTrojans.QueuedUnits)
                .Concat(mNokias.QueuedUnits)
                .Concat(mThunderbirds.QueuedUnits)
                .Concat(mAdditionalSpawns.Values.SelectMany(queue => queue.QueuedUnits));
    }
}