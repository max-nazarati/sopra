using System;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Entities;
using KernelPanic.Players;
using Microsoft.Xna.Framework;

namespace KernelPanic.Waves
{
    internal sealed class WaveManager
    {
        private PlayerOwned<List<Unit>> mUnits;
        internal PlayerOwned<Player> Players { get; }

        private TimeSpan mTimeTillFirstWave = TimeSpan.FromSeconds(30);

        private int mLastIndex;

        /// <summary>
        /// All the waves where at least one player still has units.
        /// </summary>
        private readonly List<Wave> mAliveWaves = new List<Wave>();

        /// <summary>
        /// The latest wave, this is the one where both players still have units.
        /// </summary>
        private Wave CurrentWave => mAliveWaves.Count == 0 ? null : mAliveWaves[mAliveWaves.Count - 1];

        public WaveManager(PlayerOwned<Player> players)
        {
            mUnits = new PlayerOwned<List<Unit>>(new List<Unit>(), new List<Unit>());
            Players = players;
        }

        private void Activate()
        {
            var wave = new Wave(++mLastIndex, mUnits);
            mAliveWaves.Add(wave);

            // We have to clone the units before they are modified by Spawn.
            var unitsCopy = mUnits.Map(units => new List<Unit>(units.Select(u => u.Clone())));

            void Spawn(StaticDistinction distinction)
            {
                var units = mUnits.Select(distinction);
                var player = Players.Select(distinction);
                var spawner = player.AttackingLane.UnitSpawner;
                
                void SpawnChild(Unit unit)
                {
                    player.ApplyUpgrades(unit);
                    unit.Wave = new WaveReference(wave.Index, SpawnChild);
                    units.Add(unit);
                    spawner.Register(unit, false);
                }
                
                foreach (var unit in units)
                {
                    player.ApplyUpgrades(unit);
                    unit.Wave = new WaveReference(wave.Index, SpawnChild);
                    spawner.Register(unit);
                }
            }
            
            Spawn(new StaticDistinction(true));
            Spawn(new StaticDistinction(false));
            mUnits = unitsCopy;
        }

        internal void Add(IPlayerDistinction player, Unit unit)
        {
            mUnits.Select(player).Add(unit);
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
                    Activate();
            }
        }
    }
}
