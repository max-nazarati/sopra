using System;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Entities;
using KernelPanic.Entities.Units;
using KernelPanic.Events;
using KernelPanic.Players;
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
        private TimeSpan mTimeTillFirstWave = TimeSpan.FromSeconds(10);

        [JsonProperty]
        private int mLastIndex;

        /// <summary>
        /// All the waves where at least one player still has units.
        /// </summary>
        [JsonProperty]
        private readonly List<Wave> mAliveWaves = new List<Wave>();

        /// <summary>
        /// The latest wave, this is the one where both players still have units.
        /// </summary>
        private Wave CurrentWave => mAliveWaves.Count == 0 ? null : mAliveWaves[mAliveWaves.Count - 1];

        [JsonConstructor]
        public WaveManager(PlayerIndexed<Player> players)
        {
            mTroupes = new PlayerIndexed<List<Troupe>>(new List<Troupe>(), new List<Troupe>());
            Players = players;
        }

        private void Activate()
        {
            var wave = new Wave(++mLastIndex, mTroupes);
            mAliveWaves.Add(wave);

            // We have to clone the units before they are modified by Spawn.
            var unitsCopy = mTroupes.Map(troupes => new List<Troupe>(troupes.Select(t => t.Clone())));

            void Spawn(StaticDistinction distinction)
            {
                var troupes = mTroupes.Select(distinction);
                var player = Players.Select(distinction);
                var spawner = player.AttackingLane.UnitSpawner;
                
                void SpawnChild(Troupe troupe)
                {
                    player.ApplyUpgrades(troupe);
                    troupe.Wave = new WaveReference(wave.Index, SpawnChild);
                    troupes.Add(troupe);
                    spawner.Register(troupe, false);
                }
                
                foreach (var troupe in troupes)
                {
                    player.ApplyUpgrades(troupe);
                    troupe.Wave = new WaveReference(wave.Index, SpawnChild);
                    spawner.Register(troupe);
                }
            }
            
            Spawn(new StaticDistinction(true));
            Spawn(new StaticDistinction(false));
            mTroupes = unitsCopy;
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
            // 1. Remove all units from the waves that are either dead or have reached the base.
            //    This awards experience points.
            foreach (var wave in mAliveWaves)
                wave.RemoveDead(Players);

            // 2. Remember the current wave, which might get removed from mAliveWaves in 3.
            var current = CurrentWave;

            // 3. Remove all fully defeated waves.
            mAliveWaves.RemoveAll(wave => wave.FullyDefeated);

            // 4. Activate the next wave. Doing this after 3. might save us some allocations.
            if (current?.AtLeastPartiallyDefeated ?? false)
            {
                Activate();
            }
            else if (current == null)
            {
                // Decrease the time till the first wave.
                mTimeTillFirstWave -= gameTime.ElapsedGameTime;
                if (mTimeTillFirstWave <= TimeSpan.Zero)
                {
                    Activate();
                    EventCenter.Default.Send(Event.SetupEnded());
                }
            }
        }
    }
}
