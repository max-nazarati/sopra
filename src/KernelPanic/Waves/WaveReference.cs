using System;
using KernelPanic.Entities;

namespace KernelPanic.Waves
{
    internal struct WaveReference
    {
        internal int Index { get; }
        internal Action<Troupe> SpawnChild { get; }

        internal bool IsValid => Index > 0;

        internal WaveReference(int index, Action<Troupe> spawnChild)
        {
            Index = index;
            SpawnChild = spawnChild;
        }
    }
}
