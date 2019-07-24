using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using KernelPanic.Entities;
using KernelPanic.Entities.Units;
using KernelPanic.Events;
using KernelPanic.Players;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace KernelPanic.Waves
{
    internal sealed class WaveManager
    {
        [JsonProperty]
        private PlayerIndexed<List<Troupe>> mTroupes;

        [JsonProperty]
        internal PlayerIndexed<Player> Players { get; }

        [JsonProperty]
        internal TimeSpan mTimeTillFirstWave = TimeSpan.FromSeconds(10);

        [JsonProperty]
        internal int LastIndex { get; private set; }

        internal int mByHumanDefeatedWaves, mByComputerDefeatedWaves;

        /// <summary>
        /// All the waves where at least one player still has units.
        /// </summary>
        [JsonProperty]
        private readonly List<Wave> mAliveWaves = new List<Wave>();
        private readonly List<Wave> mByHumanDefeatedWavesList = new List<Wave>();
        private readonly List<Wave> mByComputerDefeatedWavesList = new List<Wave>();

        private readonly PlayerIndexed<List<(Wave, Lazy<Troupe>, TileIndex)>> mDelayedSpawns =
            new PlayerIndexed<List<(Wave, Lazy<Troupe>, TileIndex)>>(
                new List<(Wave, Lazy<Troupe>, TileIndex)>(),
                new List<(Wave, Lazy<Troupe>, TileIndex)>());

        /// <summary>
        /// The latest wave, this is the one where both players still have units.
        /// </summary>
        private Wave CurrentWave => mAliveWaves.Count == 0 ? null : mAliveWaves[mAliveWaves.Count - 1];

        private readonly CooldownComponent mNextWaveTimer = new CooldownComponent(TimeSpan.FromSeconds(1), false);

        [JsonConstructor]
        public WaveManager(PlayerIndexed<Player> players)
        {
            mTroupes = new PlayerIndexed<List<Troupe>>(new List<Troupe>(), new List<Troupe>());
            Players = players;
            mNextWaveTimer.CooledDown += SpawnWave;
            mByHumanDefeatedWaves = 0;
            mByComputerDefeatedWaves = 0;
        }

        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            foreach (var wave in mAliveWaves)
            {
                void RestoreWaveReferences(IPlayerDistinction playerDistinction)
                {
                    foreach (var troupe in wave.Troupes.Select(playerDistinction))
                    {
                        AssignWaveReference(troupe, wave, playerDistinction);
                    }
                }
        
                RestoreWaveReferences(new StaticDistinction(true));
                RestoreWaveReferences(new StaticDistinction(false));
            }
        }

        internal int CurrentUnitCount<T>(IPlayerDistinction playerDistinction)
        {
            return mTroupes.Select(playerDistinction).Count(t => t.GetType() == typeof(T));
        }

        private void Activate()
        {
            if (mTroupes.A.Count == 0 && mTroupes.B.Count == 0)
                return;

            mAliveWaves.Add(new Wave(++LastIndex, mTroupes));
            mTroupes = mTroupes.Map(troupes => new List<Troupe>(troupes.Select(t => t.Clone())));
            mNextWaveTimer.Enabled = true;
        }

        private void SpawnWave(CooldownComponent component)
        {
            var wave = CurrentWave;
            void DistributeSpawns(StaticDistinction distinction)
            {
                foreach (var troupe in wave.Troupes.Select(distinction))
                {
                    Spawn(troupe, distinction, wave, null);
                }
            }

            DistributeSpawns(new StaticDistinction(true));
            DistributeSpawns(new StaticDistinction(false));
        }

        private void CompleteDelayedSpawns()
        {
            void DistributeSpawns(StaticDistinction distinction)
            {
                var delayed = mDelayedSpawns.Select(distinction);
                foreach (var (wave, lazyTroupe, tileIndex) in delayed)
                {
                    Spawn(lazyTroupe.Value, distinction, wave, tileIndex);
                }

                delayed.Clear();
            }
            
            DistributeSpawns(new StaticDistinction(true));
            DistributeSpawns(new StaticDistinction(false));
        }

        private void Spawn(Troupe troupe, IPlayerDistinction playerDistinction, Wave wave, TileIndex? tile)
        {
            var player = Players.Select(playerDistinction);
            player.ApplyUpgrades(troupe);
            AssignWaveReference(troupe, wave, playerDistinction);

            if (tile is TileIndex spawnTile)
            {
                wave.Troupes.Select(playerDistinction).Add(troupe);
                player.AttackingLane.UnitSpawner.Register(troupe, spawnTile);
            }
            else
            {
                player.AttackingLane.UnitSpawner.Register(troupe);
            }
        }

        private void AssignWaveReference(Troupe troupe, Wave wave, IPlayerDistinction playerDistinction)
        {
            var delayedQueue = mDelayedSpawns.Select(playerDistinction);
            troupe.Wave = new WaveReference(wave.Index,
                (lazyTroupe, lazyTile) => delayedQueue.Add((wave, lazyTroupe, lazyTile)));
        }

        internal void Add(IPlayerDistinction player, Unit unit)
        {
            var buyer = Players.Select(player);
            EventCenter.Default.Send(Event.BoughtUnit(buyer, unit));
            
            var clone = unit.Clone();
            if (clone is Troupe troupe)
            {
                mTroupes.Select(player).Add(troupe);
                return;
            }

            buyer.ApplyUpgrades(clone);
            buyer.AttackingLane.UnitSpawner.Register(clone);
        }

        internal void Update(GameTime gameTime)
        {
            CompleteDelayedSpawns();

            mNextWaveTimer.Update(gameTime);

            // 1. Remove all units from the waves that are either dead or have reached the base.
            //    This awards experience points.
            foreach (var wave in mAliveWaves)
                wave.RemoveDead(Players);

            foreach (var wave in mAliveWaves.Where(wave => wave.FullyDefeatedByHuman && !mByHumanDefeatedWavesList.Contains(wave)))
            {
                if (wave.OriginalTroupeCountB == 0) continue;
                mByHumanDefeatedWavesList.Add(wave);
                mByHumanDefeatedWaves++;
            }
            
            foreach (var wave in mAliveWaves.Where(wave => wave.FullyDefeatedByComputer && !mByComputerDefeatedWavesList.Contains(wave)))
            {
                if (wave.OriginalTroupeCountA == 0) continue;
                mByComputerDefeatedWavesList.Add(wave);
                mByComputerDefeatedWaves++;
            }

            // 2. Remember the current wave, which might get removed from mAliveWaves in 3.
            var current = CurrentWave;

            // 3. Remove all fully defeated waves.
            mAliveWaves.RemoveAll(wave => wave.FullyDefeated);

            // 4. Activate the next wave. Doing this after 3. might save us some allocations.
            // 4a) If there is no current wave, start the next one directly if the time till first wave has passed.
            if (current == null)
            {
                if (mTimeTillFirstWave <= TimeSpan.Zero)
                {
                    Activate();
                    EventCenter.Default.Send(Event.SetupEnded());
                }
                else
                {
                    mTimeTillFirstWave -= gameTime.ElapsedGameTime;
                }
                return;
            }

            // 4b) If the wave is not at least partially defeated don't start the next wave.
            if (!current.AtLeastPartiallyDefeated)
                return;
            
            // 4c) If it wasn't unbalanced, we start the next wave.
            if (!current.Unbalanced)
            {
                Activate();
                return;
            }
            
            // 4d) It was unbalanced—if all units from the wave are spawned we'll start the next one.
            if (Players.A.AttackingLane.UnitSpawner.Ready && Players.A.AttackingLane.UnitSpawner.QueuedUnits.Count() > 0 
                && Players.B.AttackingLane.UnitSpawner.Ready)
            {
                Activate();
            }
        }
    }
}
