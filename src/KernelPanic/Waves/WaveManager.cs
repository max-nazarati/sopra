using System.Collections.Generic;
using KernelPanic.Entities;
using KernelPanic.Players;

namespace KernelPanic.Waves
{
    internal sealed class WaveManager
    {
        private PlayerOwned<List<Unit>> mUnits;
        private readonly PlayerOwned<Player> mPlayers;
        
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
            mPlayers = players;
        }

        private void Activate()
        {
            var wave = new Wave(++mLastIndex, mUnits);
            mAliveWaves.Add(wave);

            void Spawn(StaticDistinction distinction)
            {
                var units = mUnits.Select(distinction);
                var spawner = mPlayers.Select(distinction).AttackingLane.UnitSpawner;

                void SpawnChild(Unit unit)
                {
                    unit.Wave = new WaveReference(wave.Index, SpawnChild);
                    units.Add(unit);
                    spawner.Register(unit, false);
                }
                
                foreach (var unit in units)
                {
                    unit.Wave = new WaveReference(wave.Index, SpawnChild);
                    spawner.Register(unit);
                }
            }
            
            Spawn(new StaticDistinction(true));
            Spawn(new StaticDistinction(false));
            mUnits.A = new List<Unit>();
            mUnits.B = new List<Unit>();
        }

        internal void Add(IPlayerDistinction player, Unit unit)
        {
            mUnits.Select(player).Add(unit);
        }

        internal void Update(bool forceActivate = false)
        {
            // 1. Remove all units from the waves that are either dead or have reached the base.
            //    This awards experience points.
            foreach (var wave in mAliveWaves)
                wave.RemoveDead(mPlayers);

            // 2. Remember if we want to activate the next wave, as the current wave might be removed in the next step.
            //    Doing this after 3. might save us some allocations.
            var activateNext = forceActivate || CurrentWave is Wave currentWave && currentWave.AtLeastPartiallyDefeated;
            
            // 3. Remove all fully defeated waves.
            mAliveWaves.RemoveAll(wave => wave.FullyDefeated);

            // 4. Activate the next wave.
            if (activateNext)
                Activate();
        }
    }
}
