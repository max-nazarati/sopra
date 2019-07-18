using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly CooldownComponent mNextWaveTimer = new CooldownComponent(TimeSpan.FromSeconds(1), false);

        [JsonConstructor]
        public WaveManager(PlayerIndexed<Player> players)
        {
            mTroupes = new PlayerIndexed<List<Troupe>>(new List<Troupe>(), new List<Troupe>());
            Players = players;
            mNextWaveTimer.CooledDown += SpawnWave;
        }

        private void Activate()
        {
            if (mTroupes.A.Count == 0 && mTroupes.B.Count == 0)
                return;

            mAliveWaves.Add(new Wave(++mLastIndex, mTroupes));
            mTroupes = mTroupes.Map(troupes => new List<Troupe>(troupes.Select(t => t.Clone())));
            mNextWaveTimer.Enabled = true;
        }

        private void SpawnWave(CooldownComponent component)
        {
            var wave = CurrentWave;

            void Spawn(StaticDistinction distinction)
            {
                var troupes = wave.Troupes.Select(distinction);
                var player = Players.Select(distinction);
                var spawner = player.AttackingLane.UnitSpawner;
                
                void SpawnChild(Troupe troupe, TileIndex tile)
                {
                    player.ApplyUpgrades(troupe);
                    troupe.Wave = new WaveReference(wave.Index, SpawnChild);
                    troupes.Add(troupe);
                    spawner.Register(troupe, tile);
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
        }

        internal void Add(IPlayerDistinction player, Unit unit)
        {
            var buyer = Players.Select(player);
            if (unit is Troupe || buyer.ValidHeroPurchase(unit))
            {

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
            else
            {
                buyer.Bitcoins += unit.Price;
            }
        }

        internal void Update(GameTime gameTime)
        {
            mNextWaveTimer.Update(gameTime);

            // 1. Remove all units from the waves that are either dead or have reached the base.
            //    This awards experience points.
            foreach (var wave in mAliveWaves)
                wave.RemoveDead(Players);

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
            if (Players.A.AttackingLane.UnitSpawner.Ready && Players.B.AttackingLane.UnitSpawner.Ready)
            {
                Activate();
            }
        }
    }
}
