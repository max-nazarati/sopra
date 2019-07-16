using System;
using KernelPanic.Entities.Units;
using KernelPanic.Table;

namespace KernelPanic.Waves
{
    internal struct WaveReference
    {
        private int Index { get; }
        internal Action<Troupe, TileIndex> SpawnChild { get; }

        internal bool IsValid => Index > 0;

        internal WaveReference(int index, Action<Troupe, TileIndex> spawnChild)
        {
            Index = index;
            SpawnChild = spawnChild;
        }
    }
}
